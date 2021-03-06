using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System;

public enum BlobState { None, Hungry, Fertile, Dead }

public class Blob : MonoBehaviour
{
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
    private static int size = 24;

    [SerializeField]
    Vector3 dest;
    Vector3 savedPos;
    public float fitness;
    public float gene_speed = 3.5f;
    public float gene_size = 1;
    [SerializeField]
    public float gene_energyNeeds = 70f;

    [SerializeField]
    GameObject smoke;

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
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + UnityEngine.Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    private void Awake()
    {
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
    }

    void LoseEnergy()
    {
        energy -= 1*((gene_speed/10f+gene_size/50f)/2f);
        UpdateState();
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

    void Die()
    {
        GameObject smokePuff = Instantiate(smoke, transform.position, transform.rotation) as GameObject;
        ParticleSystem parts = smokePuff.GetComponent<ParticleSystem>();
        float totalDuration = parts.duration;
        Destroy(smokePuff, totalDuration);

        gameManager.GetComponent<Abilities>().population.Remove(this);
        Destroy(gameObject);
    }

    public void SetRandomDestination()
    {
        float randomX = UnityEngine.Random.Range(-worldTransform.localScale.x / 2, worldTransform.localScale.x / 2);
        float randomZ = UnityEngine.Random.Range(-worldTransform.localScale.z / 2, worldTransform.localScale.z / 2);
        Vector3 point;
        if (RandomPoint(new Vector3(randomX, transform.position.y, randomZ), 30f, out point))
        {
            try
            {
                agent.SetDestination(point);
                dest = agent.destination;//debug
            }
            catch (Exception e)
            {
                Debug.Log("Error : "+e.Message);
            }
        }
    }

    public void SetTargetDestination(Vector3 target)
    {
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
        dest = agent.destination;
    }

    void UpdateState()
    {        
        if (energy < 50f) state = BlobState.Hungry;
        if (energy >= 50f && !child) state = BlobState.Fertile;
        if (energy <= 0) state = BlobState.Dead;
    }

    public void ChangeAppearance()
    {
        // change material
        float red = Convert.ToInt32(string.Join("", genes.Take(8)), 2) / 255f;
        float green = Convert.ToInt32(string.Join("", genes.Skip(8).Take(8)), 2) / 255f;
        float blue = Convert.ToInt32(string.Join("", genes.Skip(16).Take(8)), 2) / 255f;

        gfx.GetComponent<Renderer>().material.color = new Color(red, green, blue);

        // change speed
        gene_speed = red * 10f;
        agent.speed = gene_speed>0?gene_speed:1f;

        // change size
        gene_size = green * 50f;
        senseCollider.GetComponent<SphereCollider>().radius = gene_size;

        // change energy needs
        gene_energyNeeds = 100f - (blue * 100f);
        float size = 0.1f + gene_energyNeeds / 100f * .5f;
        transform.localScale = new Vector3(size, size, size);

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

/*
public class Blob : MonoBehaviour
{
    // Genetic
    public char[] genes;
    private float fitness;
    private static int size = 24;
    [SerializeField]
    GameObject gameManager;
    float gene_speed = 3.5f;
    float gene_size = 1;
    float gene_energyNeeds = 70f;

    // Sensors
    public SenseCollider senseCollider;
    public bool hasToStop = false;
    public NavMeshAgent agent;
    [SerializeField]
    Transform worldTransform;
    public bool hasPath = false;
    public bool hasRandomPath = false;

    // Logic
    public string name;
    public int energy = 20;
    public BlobState _state;
    BlobState tmpState;
    public bool fertile = false;

    #region Genetic

    public void Crossover(Blob parent1, Blob parent2)
    {
        Debug.Log("Start Crossover");
        char[] tempGenes = new char[size];

        for (int i = 0; i < tempGenes.Length; i++)
        {
            if (UnityEngine.Random.Range(0, 1) < 0.5)
            {
                tempGenes[i] = parent1.genes[i];
            }
            else
            {
                tempGenes[i] = parent2.genes[i];
            }
        }
        genes = tempGenes;
        Debug.Log("Finish Crossover");
    }

    public void Mutate(float mutationRate)
    {
        for (int i = 0; i < genes.Length; i++)
        {
            if (UnityEngine.Random.Range(0, 1) < mutationRate)
            {
                genes[i] = GameObject.FindObjectOfType<Abilities>().GetRandomGene();
            }
        }
    }
    #endregion

    #region Monobehaviour
    void Start()
    {
        // Genetic
        this.genes = new char[size];
        for (int i = 0; i < genes.Length; i++)
        {
            genes[i] = gameManager.GetComponent<Abilities>().GetRandomGene();

        }
        Debug.Log("Genome blob : " + new string(genes));
        this.fitness = gameManager.GetComponent<Abilities>().calculateFitness(genes);

        // Logic & Sensors
        _state = BlobState.None;

        InvokeRepeating("LoseEnergy", 0.0f, 1f);
    }

    void UpdateState()
    {
        if (energy < gene_energyNeeds) _state = BlobState.Hungry;
        if (energy >= gene_energyNeeds) _state = BlobState.Fertile;
        if (energy <= 0) _state = BlobState.Dead;
    }

    void LoseEnergy()
    {
        tmpState = _state;
        --energy;
        UpdateState();
        if (tmpState != _state)
        {
            switch (_state)
            {
                case BlobState.Hungry:
                    //SearchFood();
                    fertile = false;
                    senseCollider.ChangeTagToSearchFor("Edible");
                    break;
                case BlobState.Fertile:
                    //SearchPartner();
                    fertile = true;
                    senseCollider.ChangeTagToSearchFor("Blob");
                    break;
                case BlobState.Dead:
                    Die();
                    break;
                default:
                    senseCollider.ChangeTagToSearchFor("None");
                    break;
            }
        }        
    }

    void UpdateDestination()
    {
        if (!agent.hasPath || Vector3.Distance(agent.transform.position, agent.destination) < .1f || agent.isStopped) // has reach destination
        {
            Debug.Log("Is stopped.");
            senseCollider.Reset();
            if (senseCollider.hasSeenTarget)
            {
                SetTargetDestination(senseCollider.lastSeenTargetPos);
            }
            else
            {
                SetRandomDestination();
            }
        }
        else if (hasToStop && Vector3.Distance(transform.position, agent.destination) <= senseCollider.transform.localScale.x / 2)
        {
            Debug.Log(name + " : I have to stop.");
            agent.SetDestination(transform.position);
        }
    }
    private void Update()
    {
        UpdateDestination();
    }
    public void SetRandomDestination()
    {
        Debug.Log("Set random dest");
        float randomX = UnityEngine.Random.Range(-worldTransform.localScale.x / 2, worldTransform.localScale.x / 2);
        float randomZ = UnityEngine.Random.Range(-worldTransform.localScale.z / 2, worldTransform.localScale.z / 2);
        agent.SetDestination(new Vector3(randomX, transform.position.y, randomZ));
        hasRandomPath = true;

        //Debug.Log(name + ": Random path.");
    }
    public void SetTargetDestination(Vector3 dest)
    {
        Debug.Log("Set target dest");
        //Debug.DrawLine(new Vector3(transform.position.x, 1f, transform.position.z), senseCollider.lastSeenTargetPos, Color.red, 20.0f);
        float maxRange = 5;
        RaycastHit hit;
        if (Physics.Raycast(agent.transform.position, (senseCollider.lastSeenTargetPos - transform.position), out hit, maxRange))
        {
            if (hit.transform.position == senseCollider.lastSeenTargetPos)
            {
                //Debug.Log(name + ": Fruit reachable.");
                hasRandomPath = false;
                //agent.SetDestination(new Vector3(dest.x, 0, dest.z));
                agent.SetDestination(dest);
                Debug.DrawLine(agent.transform.position, dest, Color.red, 1.0f);
                //Debug.Log(name + ": Target path.");
            }
            else
            {
                Debug.Log(name + ": There is an obstacle...");
            }
        }
    }

    void Die()
    {
        Debug.Log("Die");
        Destroy(gameObject);
    }

    public void ChangeAppearance()
    {
        Debug.Log("Change appearance");
        // change material
        float red = Convert.ToInt32(string.Join("", genes.Take(8)), 2)/255f;
        float green = Convert.ToInt32(string.Join("", genes.Skip(8).Take(8)), 2) / 255f;
        float blue = Convert.ToInt32(string.Join("", genes.Skip(16).Take(8)), 2) / 255f;

        GetComponent<Renderer>().material.color = new Color(red, green, blue);

        // change speed
        gene_speed = red * 10f;
        agent.speed = gene_speed;

        // change size
        gene_size = green * 10f;
        senseCollider.GetComponent<SphereCollider>().radius = gene_size;

        // change energy needs
        gene_energyNeeds = blue * 100f;
    }

    #endregion
}
*/
