using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobWalkToTarget : StateMachineBehaviour
{
    Blob blob;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Get blob
        blob = animator.GetComponent<Blob>();
        //blob.agent.destination = blob.senseCollider.lastSeenTargetPos;
        //blob.SetTargetDestination(blob.senseCollider.lastSeenTargetPos);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(blob.hasToStop && Vector3.Distance(animator.transform.position, blob.agent.destination) <= blob.senseCollider.transform.localScale.x / 2)
        {
            Debug.Log("I have to stop.");
            animator.SetBool("hasPath", false);
            animator.SetBool("hasRandomPath", false);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
