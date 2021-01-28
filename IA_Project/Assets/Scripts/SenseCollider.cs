using UnityEngine;
using UnityEngine.AI;

public class SenseCollider : MonoBehaviour
{
    [SerializeField]
    int nbOfTargets = 0;
    public bool hasSeenTarget = false;
    public Vector3 lastSeenTargetPos = new Vector3(0, 0, 0);
    Blob blob;
    Animation anim;

    private void Start()
    {
        blob = transform.parent.GetComponent<Blob>();
        anim = GetComponent<Animation>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Edible"))
        {
            Debug.Log("Fruit seen.");
            ++nbOfTargets;
            //if(Vector3.Distance(other.transform.position, transform.position)<Vector3.Distance(lastSeenTargetPos, transform.position))
            //{
            lastSeenTargetPos = other.transform.position;
            //}
            if (blob.hasRandomPath || !blob.GetComponent<NavMeshAgent>().hasPath || Vector3.Distance(blob.GetComponent<NavMeshAgent>().destination, blob.transform.position)>Vector3.Distance(lastSeenTargetPos, blob.transform.position))
            {
                Debug.Log("New path.");
                blob.SetTargetDestination(lastSeenTargetPos);
            }
            else
            {
                Debug.Log("Continue path.");
            }
        }
        if (nbOfTargets > 0 && !hasSeenTarget)
        {
            hasSeenTarget = true;
        }
        else if (nbOfTargets <= 0 && hasSeenTarget)
        {
            hasSeenTarget = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Edible"))
        {
            if (nbOfTargets > 0)
            {
                --nbOfTargets;
                if(other.transform.position == lastSeenTargetPos)
                {
                    blob.hasToStop = true;
                    Reset();
                }
            }
        }
        if (nbOfTargets > 0 && !hasSeenTarget)
        {
            hasSeenTarget = true;
        }
        else if (nbOfTargets <= 0 && hasSeenTarget)
        {
            hasSeenTarget = false;
        }
    }

    public void Reset()
    {
        hasSeenTarget = false;
        nbOfTargets = 0;

        // reset lastSeenPos
        /*
        Vector3 tmp = transform.localScale;
        transform.localScale = new Vector3(0, 0, 0);
        transform.localScale = tmp;
        */
        anim.Play();

    }
}
