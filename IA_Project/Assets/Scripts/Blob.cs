using UnityEngine;
using UnityEngine.AI;

public enum BlobState { None, Hungry, Fertile, Dead }

public class Blob : MonoBehaviour
{
    // Genetic
    public char[] genes;
    private float fitness;
    private static int size = 24;
    [SerializeField]
    GameObject gameManager;

    // Sensors
    public SenseCollider senseCollider;
    public bool hasToStop = false;
    public NavMeshAgent agent;
    Transform worldTransform;
    public bool hasPath = false;
    public bool hasRandomPath = false;

    // Logic
    public string name;
    public int energy = 20;
    public BlobState _state;
    BlobState tmpState;
    public bool fertile = false;

    /*
    const float loopTime = 5f;
    NavMeshAgent agent;
    Transform worldTransform;
    SenseCollider senseCollider;
    public bool hasRandomPath = false;
    public bool hasToStop = false;
    */

    #region Genetic
    /*
    public Blob()
    {
        this.genes = new char[size];
        for (int i = 0; i < genes.Length; i++)
        {
            genes[i] = gameManager.GetComponent<Abilities>().GetRandomGene();

        }
        Debug.Log("Genome blob : " + new string(genes));
        this.fitness = gameManager.GetComponent<Abilities>().calculateFitness(genes);
    }*/
    
    public Blob(char[] genes)
    {
        this.genes = new char[size];
        this.genes = genes;
        Debug.Log("Genome blob : " + new string(genes));
        this.fitness = gameManager.GetComponent<Abilities>().calculateFitness(genes);
    }
    /*
    public static Blob Crossover(Blob parent1, Blob parent2)
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
        Debug.Log("Finish Crossover");
        return new Blob(tempGenes);
    }
    */

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
        gameManager = GameObject.Find("GameManager");
        Debug.Log(gameManager.GetComponent<Abilities>());

        this.genes = new char[size];
        for (int i = 0; i < genes.Length; i++)
        {
            genes[i] = gameManager.GetComponent<Abilities>().GetRandomGene();

        }
        Debug.Log("Genome blob : " + new string(genes));
        this.fitness = gameManager.GetComponent<Abilities>().calculateFitness(genes);

        // Logic & Sensors
        _state = BlobState.None;
        agent = this.GetComponent<NavMeshAgent>();
        senseCollider = transform.Find("SenseCollider").GetComponent<SenseCollider>();
        worldTransform = GameObject.Find("Terrain").transform;
        //InvokeRepeating("BlobLoop", 0.0f, loopTime);
        InvokeRepeating("LoseEnergy", 0.0f, 1f);
    }

    void UpdateState()
    {
        if (energy < 70) _state = BlobState.Hungry;
        if (energy >= 70) _state = BlobState.Fertile;
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
                    break;
            }
        }        
    }

    void UpdateDestination()
    {
        if (!agent.hasPath || Vector3.Distance(agent.transform.position, agent.destination) < 1f || agent.isStopped) // has reach destination
        {
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
        /*
        if(energy <= 0)
        {
            Die();
        }
        */
    }
    /*
    void BlobLoop()
    {
        if (!agent.hasPath)
        {
            Debug.Log("Search a path.");
            if (senseCollider.hasSeenTarget)
            {
                Debug.Log("Fruit found.");
                float maxRange = 5;
                RaycastHit hit;
                if (Physics.Raycast(transform.position, (senseCollider.lastSeenTargetPos - transform.position), out hit, maxRange))
                {
                    if (hit.transform.position == senseCollider.lastSeenTargetPos)
                    {
                        Debug.Log("Fruit reachable !");
                        SetTargetDestination(senseCollider.lastSeenTargetPos);
                    }
                    else
                    {
                        Debug.Log("There is an obstacle...");
                        Debug.DrawLine(transform.position, senseCollider.lastSeenTargetPos, Color.red, 1.0f);
                        SetRandomDestination();
                    }
                }
            }
            else
            {
                Debug.Log("No fruit found.");
                SetRandomDestination();
            }
            hasToStop = false;

        }
        else if(hasToStop && Vector3.Distance(transform.position, agent.destination) <= senseCollider.transform.localScale.x / 2)
        {
            Debug.Log("I have to stop.");
            agent.SetDestination(transform.position);
        }
    }
    */
    public void SetRandomDestination()
    {
        float randomX = Random.Range(-worldTransform.localScale.x / 2, worldTransform.localScale.x / 2);
        float randomZ = Random.Range(-worldTransform.localScale.z / 2, worldTransform.localScale.z / 2);
        agent.SetDestination(new Vector3(randomX, transform.position.y, randomZ));
        hasRandomPath = true;

        Debug.Log(name + ": Random path.");
    }
    public void SetTargetDestination(Vector3 dest)
    {
        Debug.DrawLine(transform.position, senseCollider.lastSeenTargetPos, Color.red, 1.0f);
        float maxRange = 5;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, (senseCollider.lastSeenTargetPos - transform.position), out hit, maxRange))
        {
            if (hit.transform.position == senseCollider.lastSeenTargetPos)
            {
                Debug.Log(name + ": Fruit reachable.");
                hasRandomPath = false;
                //agent.SetDestination(new Vector3(dest.x, 0, dest.z));
                agent.SetDestination(dest);
                Debug.DrawLine(transform.position, dest, Color.red, 1.0f);
                Debug.Log(name + ": Target path.");
            }
            else
            {
                Debug.Log(name + ": There is an obstacle...");
                /*
                if (!agent.hasPath)
                {
                    Debug.Log(name + ": New random path.");
                    SetRandomDestination();
                }
                */
            }
        }
        /*
        GameObject[] fruits = GameObject.FindGameObjectsWithTag("Edible");
        foreach(GameObject fruit in fruits)
        {
            Destroy(fruit);
        }
        */
    }
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Edible"))
        {
            Destroy(other.gameObject);
        }
    }*/

    void Die()
    {
        Destroy(gameObject);
    }
    #endregion
}
