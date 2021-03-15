using UnityEngine;
using UnityEngine.AI;

public class SenseCollider : MonoBehaviour
{
    [SerializeField]
    string tagToSearchFor = "None";
    [SerializeField]
    Blob blob;
    [SerializeField]
    public Animation anim;

    public bool canAnim = false;

    private void Start()
    {
        //InvokeRepeating("Anim", 0f, 2f);
        canAnim = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Abilities>().sensorsError;
    }
    private void Anim()
    {
        anim.Play("senseCollider");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagToSearchFor) && Vector3.Distance(other.transform.position, blob.agent.transform.position) < Vector3.Distance(blob.agent.transform.position, blob.agent.destination))
        {
            blob.SetTargetDestination(other.transform.position);
        }
    }

    public void ChangeTagToSearchFor(string tag)
    {
        tagToSearchFor = tag;
        if (canAnim)
        {
            Anim();
        }
    }

    /*
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
        Debug.Log("Trigger enter : "+other.tag);
        if (other.CompareTag(tagToSearchFor))
        {
            //Debug.Log(blob.name+": Target found."+(blob.fertile?other.GetComponent<Blob>().name:""));
            ++nbOfTargets;
            lastSeenTargetPos = other.transform.position;
            if (blob.hasRandomPath || !blob.GetComponent<NavMeshAgent>().hasPath || Vector3.Distance(blob.GetComponent<NavMeshAgent>().destination, blob.transform.position) > Vector3.Distance(lastSeenTargetPos, blob.transform.position))
            {
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
        Debug.Log("Trigger exit : "+other.tag);
        if (other.CompareTag(tagToSearchFor))
        {
            if (nbOfTargets > 0)
            {
                --nbOfTargets;
                
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
        Debug.Log("Reset");
        hasSeenTarget = false;
        nbOfTargets = 0;

        // reset lastSeenPos
        anim.Play();
        
;    }

    public void ChangeTagToSearchFor(string tag)
    {
        Debug.Log("ChangeTagToSearchFor");
        tagToSearchFor = tag;
        Reset();
    }
*/
}
