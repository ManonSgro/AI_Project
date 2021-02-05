using UnityEngine;
using UnityEngine.AI;

public class EatZone : MonoBehaviour
{
    [SerializeField]
    SenseCollider senseCollider;
    [SerializeField]
    NavMeshAgent agent;
    [SerializeField]
    Blob blob;

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
            Debug.Log("Eat fruit.");
            /*
            // Trigger exit
            other.transform.Translate(Vector3.up * 10000);
            //yield WaitForFixedUpdate();
            Destroy(other.gameObject);
            agent.SetDestination(agent.transform.position);
            senseCollider.Reset();
            */

            // Get energy
            agent.gameObject.GetComponent<Blob>().energy += other.GetComponent<Fruit>().calories;

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

            // Lose energy
            agent.gameObject.GetComponent<Blob>().energy -= 50;
            other.gameObject.GetComponent<Blob>().energy -= 50;

            senseCollider.Reset();
        }
    }
}
