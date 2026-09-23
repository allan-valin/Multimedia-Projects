using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Jump : StateMachineBehaviour
{
    Rigidbody2D rb;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponentInParent<Rigidbody2D>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        DiveAttack();
        animator.SetBool("Jump", false); // Set Jump to false after the jump action is completed
    }

    void DiveAttack()
    {
        if (BossController.Instance.diveAttack)
        {
            BossController.Instance.Flip();

            // Move the boss towards moveToPosition
            //Vector2 _newPos = Vector2.MoveTowards(rb.position, BossController.Instance.moveToPosition,BossController.Instance.speed * 3 * Time.fixedDeltaTime);
            
            // from bounce attack
            //BossController.Instance.moveToPosition = new Vector2(PlayerController.Instance.transform.position.x, rb.position.y + 10);
            //Vector2 _newPos = new Vector2(PlayerController.Instance.transform.position.x, rb.position.y + 10);
            Vector2 _newPos = Vector2.MoveTowards(rb.position, BossController.Instance.moveToPosition, BossController.Instance.speed * Time.fixedDeltaTime);

            rb.MovePosition(_newPos);

            float _distance = Vector2.Distance(rb.position, _newPos);
            //Debug.Log("Distance: " + _distance);
            if (_distance < 1f)
            {
                
                BossController.Instance.Dive();
                //Debug.Log("Boss is reaching the target position. Current position: " + rb.position + ", Target position: " + BossController.Instance.moveToPosition);
                

            }
            else
            {
                Debug.Log("Boss is not reaching the target position. Current position: " + rb.position + ", Target position: " + BossController.Instance.moveToPosition);
            }
        }
        else
        {
            Debug.Log("DiveAttack is not being called because BossController.Instance.diveAttack is false.");
        }
        
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        

    }
}