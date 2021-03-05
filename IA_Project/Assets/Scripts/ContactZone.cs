using UnityEngine;
using UnityEngine.AI;

public class ContactZone : MonoBehaviour
{
    [SerializeField]
    Blob blob;
    [SerializeField]
    GameObject babyBlob;

    GameObject gameManager;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (blob.state == BlobState.Hungry && other.CompareTag("Edible"))
        {
            if (blob.child)
            {
                blob.transform.localScale = blob.transform.localScale + new Vector3(0.1f, 0.1f, 0.1f);
                if (blob.transform.localScale.x >= 1)
                {
                    blob.child = false;
                }
            }
            blob.energy += other.GetComponent<Fruit>().calories;
            Destroy(other.gameObject);
        }
        if (blob.state == BlobState.Fertile && other.CompareTag("Blob") && other.GetComponent<Blob>().state == BlobState.Fertile)
        {
            gameManager.GetComponent<Abilities>().AddParents(blob, other.GetComponent<Blob>());
        }
    }
    /*
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

    private void OnTriggerEnter(Collider other)
    {
        if (blob._state == BlobState.Hungry && other.CompareTag("Edible"))
        {
            Debug.Log("Eat fruit.");

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
    */
}
