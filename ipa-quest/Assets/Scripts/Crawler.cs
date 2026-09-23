using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crawler : Enemy
{
    float timer;
    [SerializeField] private float flipWaitTime;
    [SerializeField] private float ledgeCheckX;
    [SerializeField] private float ledgeCheckY;
    [SerializeField] private LayerMask whatIsGround;

    //Vector3 _ledgeCheckStart;
    //Vector2 _wallCheckDir;
    protected override void Start()
    {
        base.Start();
        rb.gravityScale = 12f;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        //ChangeState(EnemyStates.Crawler_Idle);
        
        /*if (!PlayerController.Instance.pState.alive)
        {
            ChangeState(EnemyStates.Crawler_Idle);
        }
        
        if (!isRecoiling)
        {
            transform.position = Vector2.MoveTowards
                (transform.position, new Vector2(PlayerController.Instance.transform.position.x, transform.position.y),
                speed * Time.deltaTime);
        }*/
        
    }
  

    protected override void UpdateEnemyStates()
    {
        if (health <= 0)
        {
            Death(0.05f);
        }
        Vector3 _ledgeCheckStart = transform.localScale.x > 0 ? new Vector3(ledgeCheckX, 0) : new Vector3(-ledgeCheckX, 0);
        Vector2 _wallCheckDir = transform.localScale.x > 0 ? transform.right : -transform.right;
        switch (GetCurrentEnemyState)
        {
            
            case EnemyStates.Crawler_Idle:

                
                if (!Physics2D.Raycast(transform.position + _ledgeCheckStart, Vector2.down, ledgeCheckY, whatIsGround) ||
                    Physics2D.Raycast(transform.position, _wallCheckDir, ledgeCheckX, whatIsGround))
                {
                    ChangeState(EnemyStates.Crawler_Flip);
                }

                if (transform.localScale.x > 0) rb.velocity = new Vector2(speed, rb.velocity.y);
                else rb.velocity = new Vector2(-speed, rb.velocity.y);
            
                break;
            
            case EnemyStates.Crawler_Flip:
                timer += Time.deltaTime;

                if(timer > flipWaitTime)
                {
                    timer = 0;
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                    ChangeState(EnemyStates.Crawler_Idle);
                }

                break;
        }
    }
    /*
    private void OnDrawGizmosSelected()
    {
        _ledgeCheckStart = transform.localScale.x > 0 ? new Vector3(ledgeCheckX, 0) : new Vector3(-ledgeCheckX, 0);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position + _ledgeCheckStart, Vector2.down * ledgeCheckY);
    }
*/
}
