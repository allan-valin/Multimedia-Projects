using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Bounce2 : StateMachineBehaviour
{
    Rigidbody2D rb;
    bool callOnce;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponentInParent<Rigidbody2D>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector2 _forceDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * BossController.Instance.rotationDirectionToTarget),
            Mathf.Sin(Mathf.Deg2Rad * BossController.Instance.rotationDirectionToTarget));
        rb.AddForce(_forceDirection * 3, ForceMode2D.Impulse);

        BossController.Instance.divingCollider.SetActive(true);

        if (BossController.Instance.Grounded())
        {
            BossController.Instance.divingCollider.SetActive(false);
            if (!callOnce)
            {
                GameObject _impactParticle = Instantiate(BossController.Instance.impactParticle,
                    BossController.Instance.groundCheckPoint.position, Quaternion.identity);
                Destroy(_impactParticle, 4f);

                BossController.Instance.ResetAllAttacks();
                BossController.Instance.CheckBounce();
                callOnce = true;
            }

            animator.SetTrigger("Grounded");
        }

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Bounce2");
        animator.ResetTrigger("Grounded");
        callOnce = false;
    }
}