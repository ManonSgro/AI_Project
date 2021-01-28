using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobSearchForTarget : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Get senseCollider
        SenseCollider senseCollider = animator.GetComponent<Blob>().senseCollider;

        if (senseCollider.hasSeenTarget)
        {
            Debug.Log("Fruit found.");
            float maxRange = 5;
            RaycastHit hit;
            if (Physics.Raycast(animator.transform.position, (senseCollider.lastSeenTargetPos - animator.transform.position), out hit, maxRange))
            {
                if (hit.transform.position == senseCollider.lastSeenTargetPos)
                {
                    Debug.Log("Fruit reachable !");
                    animator.SetBool("hasPath", true);
                    //SetTargetDestination(senseCollider.lastSeenTargetPos);
                }
                else
                {
                    Debug.Log("There is an obstacle...");
                    Debug.DrawLine(animator.transform.position, senseCollider.lastSeenTargetPos, Color.red, 1.0f);
                    animator.SetBool("hasRandomPath", true);
                }
            }
        }
        else
        {
            Debug.Log("No fruit found.");
            animator.SetBool("hasRandomPath", true);
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
     //   
    //}

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
