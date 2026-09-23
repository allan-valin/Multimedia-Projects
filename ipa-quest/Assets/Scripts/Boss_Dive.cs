using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Dive : StateMachineBehaviour
{
    Rigidbody2D rb;
    bool callOnce;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponentInParent<Rigidbody2D>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("DiveAttack is being called.");
        BossController.Instance.divingCollider.SetActive(true);
        Debug.Log("Diving collider is set to true.");
        
        Debug.Log("Boss is diving.");
        if (BossController.Instance.Grounded())
        {
            Debug.Log("Boss is grounded.");
            BossController.Instance.divingCollider.SetActive(false);
            Debug.Log("Diving collider is set to false.");

            if (!callOnce)
            {
                Debug.Log("DiveAttack is being called once.");
                GameObject _impactParticle = Instantiate(BossController.Instance.impactParticle,
                    BossController.Instance.groundCheckPoint.position, Quaternion.identity);
                Destroy(_impactParticle, 4f);
                BossController.Instance.DivingPillars();
                animator.SetBool("Dive", false);
                BossController.Instance.ResetAllAttacks();
                callOnce = true;
            }
        }
        else
        {
            animator.SetBool("Dive", false);
        }

        // Set DiveAttack to false after the dive attack action is completed
        BossController.Instance.diveAttack = false;
    }
    
    

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        callOnce = false;
    }
}