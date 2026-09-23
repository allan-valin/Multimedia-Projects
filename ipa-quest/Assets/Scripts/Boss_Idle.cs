using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Idle : StateMachineBehaviour
{
    public Rigidbody2D rb;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponentInParent<Rigidbody2D>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb.velocity = Vector2.zero;
        RunToPlayer(animator);

        if (BossController.Instance.attackCountdown <= 0)
        {
            BossController.Instance.AttackHandler();
            BossController.Instance.attackCountdown = Random.Range(BossController.Instance.attackTimer - 1, BossController.Instance.attackTimer + 1);
        }
    }

    void RunToPlayer(Animator animator)
    {
        if (Vector2.Distance(PlayerController.Instance.transform.position, rb.position) >= BossController.Instance.attackRange)
        {
            animator.SetBool("Run", true);
        }
        else
        {
            return;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}