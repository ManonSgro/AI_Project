using UnityEngine;
using UnityEngine.AI;

public class Blob : MonoBehaviour
{
    public SenseCollider senseCollider;
    public bool hasToStop = false;
    NavMeshAgent agent;
    public bool hasRandomPath = false;

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
        //worldTransform = GameObject.Find("Terrain").transform;
        //InvokeRepeating("BlobLoop", 0.0f, loopTime);
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

    void SetRandomDestination()
    {
        float randomX = Random.Range(-worldTransform.localScale.x / 2, worldTransform.localScale.x / 2);
        float randomZ = Random.Range(-worldTransform.localScale.z / 2, worldTransform.localScale.z / 2);
        agent.SetDestination(new Vector3(randomX, transform.position.y, randomZ));
        hasRandomPath = true;

        Debug.Log("Random path.");
    }
    */
    public void SetTargetDestination(Vector3 dest)
    {
        hasRandomPath = false;
        //agent.SetDestination(new Vector3(dest.x, 0, dest.z));
        agent.SetDestination(dest);
        Debug.DrawLine(transform.position, dest, Color.red, 1.0f);
        Debug.Log("Target path.");
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
