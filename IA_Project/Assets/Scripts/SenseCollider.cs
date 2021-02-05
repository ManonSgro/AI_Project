using UnityEngine;
using UnityEngine.AI;

public class SenseCollider : MonoBehaviour
{
    [SerializeField]
    public int nbOfTargets = 0;
    public bool hasSeenTarget = false;
    public Vector3 lastSeenTargetPos = new Vector3(0, 0, 0);
    Blob blob;
    Animation anim;
    [SerializeField]
    string tagToSearchFor = "None";

    private void Start()
    {
        blob = transform.parent.GetComponent<Blob>();
        anim = GetComponent<Animation>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagToSearchFor))
        {
            Debug.Log(blob.name+": Target found."+(blob.fertile?other.GetComponent<Blob>().name:""));
            ++nbOfTargets;
            lastSeenTargetPos = other.transform.position;
            if (blob.hasRandomPath || !blob.GetComponent<NavMeshAgent>().hasPath || Vector3.Distance(blob.GetComponent<NavMeshAgent>().destination, blob.transform.position) > Vector3.Distance(lastSeenTargetPos, blob.transform.position))
            {
                Debug.Log(blob.name+": New target path.");
                //blob.GetComponent<Animator>().SetBool("hasPath", true);
                //blob.GetComponent<Animator>().SetBool("hasRandomPath", false);
                blob.SetTargetDestination(lastSeenTargetPos);
            }
            else
            {
                Debug.Log(blob.name + ": Continue path.");
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
        if (other.CompareTag(tagToSearchFor))
        {
            if (nbOfTargets > 0)
            {
                --nbOfTargets;
                if (other.transform.position == lastSeenTargetPos)
                {
                    Debug.Log(blob.name + " has to stop soon.");
                    blob.hasToStop = true;
                    //Reset();
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
        /*
        if (nbOfTargets<=0)
        {
            Debug.Log(nbOfTargets);
            blob.GetComponent<Animator>().SetBool("hasRandomPath", true);
            blob.GetComponent<Animator>().SetBool("hasPath", false);
        }
        else
        {
            blob.GetComponent<Animator>().SetBool("hasPath", true);
            blob.GetComponent<Animator>().SetBool("hasRandomPath", false);
        }
        */
    }

    public void ChangeTagToSearchFor(string tag)
    {
        tagToSearchFor = tag;
        Reset();
    }
}
