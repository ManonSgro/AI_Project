using UnityEngine;
using UnityEngine.AI;

public class EatZone : MonoBehaviour
{
    SenseCollider senseCollider;
    NavMeshAgent agent;

    private void Start()
    {
        senseCollider = GameObject.Find("SenseCollider").GetComponent<SenseCollider>();
        agent = GameObject.Find("Blob").GetComponent<NavMeshAgent>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Edible"))
        {
            Debug.Log("Eat fruit.");
            // Trigger exit
            other.transform.Translate(Vector3.up * 10000);
            //yield WaitForFixedUpdate();
            Destroy(other.gameObject);
            agent.SetDestination(agent.transform.position);
            senseCollider.Reset();
        }
    }
}
