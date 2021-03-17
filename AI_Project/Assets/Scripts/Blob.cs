using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;

public enum BlobState { None, Hungry, Fertile, Dead }

public class BlobGroup
{
    public List<Blob> members;

    public BlobGroup(int size, Blob itself)
    {
        members = new List<Blob>(size+1);
        members.Add(itself);
    }

    public BlobGroup MergeGroups(BlobGroup otherGroup)
    {
        members.AddRange(otherGroup.members);
        members = members.Distinct().ToList();
        return this;
    }
}

public class Blob : MonoBehaviour
{
    public string firstname = "Etienne";

    [SerializeField]
    public float energy = 3;
    [SerializeField]
    public NavMeshAgent agent;
    Transform worldTransform;
    [SerializeField]
    SenseCollider senseCollider;
    [SerializeField]
    GameObject gfx;

    public BlobState state = BlobState.None;
    public bool child = true;

    // Genetic
    [SerializeField]
    public bool firstGen = false;
    GameObject gameManager;

    public char[] genes;
    private static int size = 40;

    Vector3 savedPos;
    public float fitness;
    public float gene_speed = 3.5f;
    public float gene_size = 1;
    [SerializeField]
    public float gene_energyNeeds = 70f;
    public float gene_groupSize = 0f;
    //public List<Blob> group;
    public BlobGroup group;
    public float gene_share = 0f;
    float energyLoss = 0f;

    [SerializeField]
    GameObject smoke;
    [SerializeField]
    GameObject hearts;

    // for groups
    Vector3 oldPoint = Vector3.zero;
    bool hasRandomPath = false;

    public IEnumerator MakingBaby()
    {
        agent.speed = 0f;
        yield return new WaitForSeconds(5f);
        agent.speed = gene_speed > 0 ? gene_speed : 1f;
    }

    public void Crossover(Blob parent1, Blob parent2)
    {
        char[] tempGenes = new char[size];

        for (int i = 0; i < tempGenes.Length; i++)
        {
            if (UnityEngine.Random.Range(0f, 1f) < 0.5f)
            {
                tempGenes[i] = parent1.genes[i];
            }
            else
            {
                tempGenes[i] = parent2.genes[i];
            }
        }
        genes = tempGenes;
    }

    public void Mutate(float mutationRate)
    {
        for (int i = 0; i < genes.Length; i++)
        {
            if (UnityEngine.Random.Range(0f, 1f) < mutationRate)
            {
                genes[i] = gameManager.GetComponent<Abilities>().GetRandomGene();
            }
        }
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPoint = center + UnityEngine.Random.insideUnitSphere * range;
            NavMeshHit hit;
            //var areaMaskFromName = 1 << NavMesh.GetAreaFromName("Walkable");
            var areaMaskFromName = NavMesh.AllAreas;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, areaMaskFromName))
            {
                result = hit.position;
                return true;
            }
            else
            {
                result = Vector3.MoveTowards(transform.position, gameManager.GetComponent<Abilities>().safePos.position, agent.speed*0.1f);
                return true;
            }
        }
        //result = Vector3.zero;
        //return false;
        result = gameManager.GetComponent<Abilities>().safePos.position;
        return false;
    }

    private void Awake()
    {
        transform.parent = GameObject.FindGameObjectWithTag("BlobRoot").transform; // avoid bug

        PlayParticles(transform.position);

        gameManager = GameObject.Find("GameManager");

        gameManager.GetComponent<Abilities>().population.Add(this);

        worldTransform = GameObject.Find("Terrain").transform;
        if (firstGen)
        {
            genes = new char[size];
            for (int i = 0; i < genes.Length; i++)
            {
                genes[i] = gameManager.GetComponent<Abilities>().GetRandomGene();

            }
            ChangeAppearance();
        }

        InvokeRepeating("LoseEnergy", 0f, 1f);

        savedPos = agent.transform.position;
        InvokeRepeating("IsWaitingForTooLong", 0f, 3f);

        List<string> possibleNames = new List<string>() { "Eva", "Johan", "Sophie", "Ruben", "Clélie", "Louisa", "Elisa", "Clara", "Enguerrand", "Laura", "Mathilde", "Zoé", "Jules", "Mélissande", "Thomas", "Andréa", "Barbara", "Guillaume", "Julien", "Victor", "Yoann", "Amandine", "Pierre", "Laurine", "Cyrielle", "Lucie", "Antoine", "Nicolas", "Monica", "Flora", "Brice", "Amélia", "Solène", "Victorine", "Baptiste", "Line", "Océane", "Maéva", "Bastien", "Laurelenn", "Margaux", "Manon", "Pierre", "Margaux", "Roxane", "Gaëlle", "Sarah" };
        firstname = possibleNames[UnityEngine.Random.Range(0, possibleNames.Count - 1)];
    }

    void IsWaitingForTooLong()
    {
        Vector3 newPos = agent.transform.position;
        if(Vector3.Distance(newPos, savedPos) <= 0.5f)
        {
            // is blocked
            SetRandomDestination();
        }
        else
        {
            savedPos = newPos;
        }
    }

    private void Update()
    {
        if (!agent.hasPath || Vector3.Distance(agent.transform.position, agent.destination) < 1f)
        {
            SetRandomDestination();
        }
        /*
        if (hasRandomPath && group.Count > 0)
        {
            if(Vector3.Distance(group[0].transform.position, oldPoint) > 5f)
            {
                oldPoint = group[0].transform.position;

                Vector3 newPoint = SetGroupDestination();
                Vector3 point;
                if (RandomPoint(newPoint, 30f, out point))
                {
                    try
                    {
                        agent.SetDestination(point);
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Error : " + e.Message);
                    }
                }
            }
        }*/
    }

    void LoseEnergy()
    {
        energy -= energyLoss;

        if (UpdateState())
        {
            switch (state)
            {
                case BlobState.Hungry:
                    senseCollider.ChangeTagToSearchFor("Edible");
                    break;
                case BlobState.Fertile:
                    senseCollider.ChangeTagToSearchFor("Blob");
                    break;
                case BlobState.Dead:
                    Die();
                    break;
                default:
                    break;
            }
        } 
        if(state == BlobState.Fertile && senseCollider.canAnim)
        {
            senseCollider.anim.Play("senseCollider");
        }
    }

    void PlayParticles(Vector3 point)
    {
        GameObject smokePuff = Instantiate(smoke, point, smoke.transform.rotation) as GameObject;
        ParticleSystem parts = smokePuff.GetComponent<ParticleSystem>();
        float totalDuration = parts.main.duration + parts.main.startLifetime.constantMax;
        Destroy(smokePuff, totalDuration);
    }

    void Die()
    {
        PlayParticles(transform.position);

        gameManager.GetComponent<Abilities>().population.Remove(this);
        Destroy(gameObject);
    }

    private void OnMouseDown()
    {
        gameManager.GetComponent<Abilities>().blobPanel.UpdateInfos(this);
        gameManager.GetComponent<Abilities>().blobPanel.gameObject.SetActive(true);
        gameManager.GetComponent<Abilities>().vCam.m_Follow = transform;
    }

    Vector3 SetGroupDestination()
    {
        Vector3 randomPoint = transform.position;

        // check for null
        group.members.RemoveAll(item => item == null);

        if (group.members.Count > 0)
        {
            Vector3 average = Vector3.zero;
            int n = 0;
            for(int i=0; i<group.members.Count; ++i)
            {
                if (group.members[i] != null && group.members[i].hasRandomPath)// && Vector3.Distance(group[i].transform.position, transform.position) < 10f)
                {
                    average += group.members[i].transform.position;
                    ++n;
                }
            }
            randomPoint = average/n;
        }
        //return randomPoint;
        return Vector3.MoveTowards(transform.position, randomPoint, agent.speed);
    }

    public void SetRandomDestination()
    {
        hasRandomPath = true;
        /*
        float randomX = UnityEngine.Random.Range(-worldTransform.localScale.x / 2, worldTransform.localScale.x / 2);
        float randomZ = UnityEngine.Random.Range(-worldTransform.localScale.z / 2, worldTransform.localScale.z / 2);
        Vector3 randomPoint = new Vector3(randomX, transform.position.y, randomZ);
        /*
        // check for null
        group.RemoveAll(item => item == null);

        if (group.Count > 0)
        {
            /*
            Vector3 average = Vector3.zero;
            int n = 0;
            for(int i=0; i<group.Count; ++i)
            {
                if (group[i] != null)
                {
                    average += group[i].agent.destination;
                    ++n;
                }
            }
            randomPoint = average/n;
            */
        /*
            float minDist = Vector3.Distance(group[0].transform.position, transform.position);
            int idNeighbor = 0;
            for (int i = 0; i < group.Count; ++i)
            {
                float newDist = Vector3.Distance(group[i].transform.position, transform.position);
                if (minDist > newDist)
                {
                    minDist = newDist;
                    idNeighbor = i;

                }
            }
            randomPoint = group[idNeighbor].agent.destination;
        }
        */
        Vector3 randomPoint;
        if (group.members.Count > 0)
        {
            randomPoint = SetGroupDestination();
            //oldPoint = group[0].transform.position;
        }
        else
        {
            float randomX = UnityEngine.Random.Range(-worldTransform.localScale.x / 2, worldTransform.localScale.x / 2);
            float randomZ = UnityEngine.Random.Range(-worldTransform.localScale.z / 2, worldTransform.localScale.z / 2);
            randomPoint = new Vector3(randomX, transform.position.y, randomZ);
        }

        // find closest point on map
        Vector3 point;
        if (RandomPoint(randomPoint, 30f, out point))
        {
            try
            {
                agent.SetDestination(point);
            }
            catch (Exception e)
            {
                Debug.Log("Error : "+e.Message);
            }
        }
    }

    public void SetTargetDestination(Vector3 target)
    {
        hasRandomPath = false;
        /*
        Debug.DrawRay(new Vector3(agent.transform.position.x, 0f, agent.transform.position.z), (target - agent.transform.position), Color.red, 10.0f);

        RaycastHit hit;
        if (Physics.Raycast(new Vector3(agent.transform.position.x, 0f, agent.transform.position.z), (target - agent.transform.position), out hit))
        {
            if (Vector3.Distance(hit.transform.position, target) <= .5f)
            {
                agent.SetDestination(target);
            }
            else
            {
                Debug.Log(name + ": There is an obstacle...");
            }
        }
        */
        agent.SetDestination(target);
    }

    bool UpdateState()
    {
        if (energy < 50f && state != BlobState.Hungry)
        {
            state = BlobState.Hungry;
            hearts.SetActive(false);
            return true;
        }
        if (energy >= 50f && !child && state != BlobState.Fertile)
        {
            state = BlobState.Fertile;
            hearts.SetActive(true);
            return true;
        }
        if (energy <= 0)
        {
            state = BlobState.Dead;
            hearts.SetActive(false);
            return true;
        }
        return false;
    }

    public void ChangeAppearance()
    {
        // change material
        float red = Convert.ToInt32(string.Join("", genes.Take(8)), 2) / 255f;
        float green = Convert.ToInt32(string.Join("", genes.Skip(8).Take(8)), 2) / 255f;
        float blue = Convert.ToInt32(string.Join("", genes.Skip(16).Take(8)), 2) / 255f;
        float groupSize = gameManager.GetComponent<Abilities>().canFormGroups ? Convert.ToInt32(string.Join("", genes.Skip(24).Take(8)), 2) / 255f : 0f;
        float share = gameManager.GetComponent<Abilities>().canShareEnergy ? Convert.ToInt32(string.Join("", genes.Skip(32).Take(8)), 2) / 255f : 0f;

        gfx.GetComponent<Renderer>().material.color = new Color(red, green, blue);

        // change speed
        gene_speed = red * 10f;
        agent.speed = gene_speed>0?gene_speed:1f;

        // change size
        gene_size = green * 50f;
        senseCollider.GetComponent<SphereCollider>().radius = gene_size;

        
        float nom = 0f;
        float denom = 0f;
        if (gameManager.GetComponent<Abilities>().speedAffectsEnergy)
        {
            nom += gene_speed / 10f;
            denom += 1f;
        }
        if (gameManager.GetComponent<Abilities>().sensorsAffectsEnergy)
        {
            nom += gene_size / 50f;
            denom += 1f;
        }
        if (denom > 0f)
        {
            energyLoss = 1 * (nom / denom);

        }
        else
        {
            energyLoss = 1f;
        }

        // change energy needs
        gene_energyNeeds = 100f - (blue * 100f);
        if (gameManager.GetComponent<Abilities>().energyAffectsBabies)
        {
            float size = 0.2f + gene_energyNeeds / 100f * .5f;
            transform.localScale = new Vector3(size, size, size);
        }
        else
        {
            child = false;
        }

        // change groupSize
        int maxGroupSize = 10;
        gene_groupSize = Mathf.Round(groupSize * maxGroupSize);
        //group = new List<Blob>(Mathf.RoundToInt(gene_groupSize));
        group = new BlobGroup(Mathf.RoundToInt(gene_groupSize), this);

        // change sharing
        gene_share = share;

        // calculate fitness
        fitness = gameManager.GetComponent<Abilities>().calculateFitness(genes);

        /*
        // create custom animation for sensors
        Animation anim = senseCollider.GetComponent<Animation>(); // required
        AnimationCurve curve;

        // create a new AnimationClip
        AnimationClip clip = new AnimationClip();
        clip.legacy = true;
        clip.name = gene_size.ToString();

        // create a curve to move the GameObject and assign to the clip
        Keyframe[] keys;
        keys = new Keyframe[2];
        keys[0] = new Keyframe(0.0f, 0.0f);
        keys[1] = new Keyframe(1.0f, 1);
        curve = new AnimationCurve(keys);
        clip.SetCurve("", typeof(Transform), "localScale.x", curve);
        clip.SetCurve("", typeof(Transform), "localScale.y", curve);
        clip.SetCurve("", typeof(Transform), "localScale.z", curve);

        // now animate the GameObject
        anim.AddClip(clip, clip.name);
        senseCollider.clipName = clip.name;
        */
    }
}
