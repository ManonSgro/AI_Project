using UnityEngine;
using UnityEngine.AI;

public class Blob : MonoBehaviour
{
    public SenseCollider senseCollider;
    public bool hasToStop = false;
    public NavMeshAgent agent;
    Transform worldTransform;
    public bool hasPath = false;
    public bool hasRandomPath = false;

    public string name;

    /*
    const float loopTime = 5f;
    NavMeshAgent agent;
    Transform worldTransform;
    SenseCollider senseCollider;
    public bool hasRandomPath = false;
    public bool hasToStop = false;
    */
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        senseCollider = transform.Find("SenseCollider").GetComponent<SenseCollider>();
        worldTransform = GameObject.Find("Terrain").transform;
        //InvokeRepeating("BlobLoop", 0.0f, loopTime);
    }

    private void Update()
    {
        if (!agent.hasPath ||Vector3.Distance(agent.transform.position, agent.destination)<1f) // has reach destination
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
            Debug.Log(name +" : I have to stop.");
            agent.SetDestination(transform.position);
        }
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
}
