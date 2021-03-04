using UnityEngine;
using UnityEngine.AI;

public class ContactZone : MonoBehaviour
{
    [SerializeField]
    SenseCollider senseCollider;
    [SerializeField]
    NavMeshAgent agent;
    [SerializeField]
    Blob blob;
    [SerializeField]
    GameObject prefab;

    // Genetic
    public float mutationRate = 0.01f;

    private void Start()
    {
        /*
        senseCollider = GameObject.Find("SenseCollider").GetComponent<SenseCollider>();
        agent = GameObject.Find("Blob").GetComponent<NavMeshAgent>();
        blob = GameObject.Find("Blob").GetComponent<Blob>();
        */
    }
    private void OnTriggerEnter(Collider other)
    {
        if (blob._state == BlobState.Hungry && other.CompareTag("Edible"))
        {
            //Debug.Log("Eat fruit.");
            /*
            // Trigger exit
            other.transform.Translate(Vector3.up * 10000);
            //yield WaitForFixedUpdate();
            Destroy(other.gameObject);
            agent.SetDestination(agent.transform.position);
            senseCollider.Reset();
            */

            // Get energy
            blob.energy += other.GetComponent<Fruit>().calories;

            // Trigger exit
            other.transform.Translate(Vector3.up * 10000);
            //yield WaitForFixedUpdate();
            Destroy(other.gameObject);

            senseCollider.Reset();

            //agent.GetComponent<Animator>().SetBool("hasPath", false);
            //agent.GetComponent<Animator>().SetBool("hasRandomPath", false);

        }else if (blob._state == BlobState.Fertile && other.CompareTag("Blob") && other.GetComponent<Blob>().fertile)
        {
            Debug.Log("Making babies.");

            // Get parents
            Blob parent1 = blob;
            Blob parent2 = other.gameObject.GetComponent<Blob>();

            // Lose energy
            parent1.energy -= 50;
            parent2.energy -= 50;

            // Add new blob
            GameObject child = Instantiate(prefab, parent1.transform.position, Quaternion.identity);

            // Calculate genes
            child.GetComponent<Blob>().Crossover(parent1, parent2);
            //child.GetComponent<Blob>() = Blob.Crossover(parent1, parent2);

            child.GetComponent<Blob>().Mutate(mutationRate);

            child.GetComponent<Blob>().ChangeAppearance();

            child.GetComponent<Blob>().energy = 20;

            senseCollider.Reset();
        }
    }
}
