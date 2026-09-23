using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] protected float recoilLength;
    [SerializeField] protected float recoilFactor;
    [SerializeField] protected bool isRecoiling = false;

    [SerializeField] public float speed;

    [SerializeField] public float damage;
    [SerializeField] public GameObject orangeBlood;

    [SerializeField] public AudioClip hurtSound;

    protected float recoilTimer;
    [HideInInspector] public Rigidbody2D rb;
    protected SpriteRenderer sr;
    public Animator anim;
    protected AudioSource audioSource;

    protected bool hasTakenDamage = false;
    protected EnemyStates currentEnemyState;
    protected enum EnemyStates
    {
        // Crawler
        Crawler_Idle,
        Crawler_Flip,

        // Bat
        Bat_Idle,
        Bat_Chase,
        Bat_Stunned,
        Bat_Death,

        // Charger
        Charger_Idle,
        Charger_Surprised,
        Charger_Charge,

        // Boss
        Boss_Stage1,
        Boss_Stage2,
        Boss_Stage3,
        Boss_Stage4,
    }

    protected virtual EnemyStates GetCurrentEnemyState
    {
        get { return currentEnemyState; }
        set
        {
            if(currentEnemyState != value)
            {
                currentEnemyState = value;

                ChangeCurrentAnimation();
            }
        }
    }

    // Start is called before the first frame update
    
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }
    // Update is called once per frame
    protected virtual void Update()
    {
        if (GameManager.Instance.gameIsPaused) return;
        
        hasTakenDamage = false;

        
        if (isRecoiling)
        {
            if (recoilTimer < recoilLength)
            {
                recoilTimer += Time.deltaTime;
            }
            else
            {
                isRecoiling = false;
                recoilTimer = 0;
            }
        }
        else
        {
            UpdateEnemyStates();
        }
    }

    
    public virtual void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        if (hasTakenDamage) return;
        
        // Check if the collided object is the player's attack collider
        if (this.gameObject.GetComponent<BoxCollider2D>() != null && this.gameObject.GetComponent<BoxCollider2D>().isTrigger)
        {
            return; // If it is, do not apply damage
        }

        health -= _damageDone;
        //if (!isRecoiling)
        //{
            audioSource.PlayOneShot(hurtSound);
            GameObject _orangeBlood = Instantiate(orangeBlood, transform.position, Quaternion.identity);
            Destroy(_orangeBlood, 5.5f);
            //rb.velocity = _hitForce * recoilFactor * _hitDirection;
            rb.AddForce(_hitDirection * _hitForce, ForceMode2D.Impulse);
            isRecoiling = true;

            
        //}
        hasTakenDamage = true;
   
    }
    
    
    protected virtual void OnCollisionStay2D(Collision2D _other)
    {
        if (_other.gameObject.CompareTag("Player") && !PlayerController.Instance.pState.invincible && health > 0 
            && _other.gameObject.layer != LayerMask.NameToLayer("PlayerAttack") && !PlayerController.Instance.pState.attacking)//&& !_other.gameObject.CompareTag("PlayerAttack"))
        {
            Attack();

            if(PlayerController.Instance.pState.alive)
            {
                PlayerController.Instance.HitStopTime(0, 5, 0.5f);
            }

            
        }

        if (_other.gameObject.CompareTag("Enemy"))
        {
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        }
        
    }

    public virtual void Death(float _destroyTime)
    {
        Destroy(gameObject, _destroyTime);
    }

    protected virtual void UpdateEnemyStates() {}

    protected virtual void ChangeCurrentAnimation() { }
    protected void ChangeState(EnemyStates _newState)
    {
        GetCurrentEnemyState = _newState;
    }
    protected virtual void Attack()
    {
        if (PlayerController.Instance.pState.attacking) return;
        
        PlayerController.Instance.TakeDamage(damage);
        //PlayerController.Instance.HitStopTime(0, 5, 0.5f);
    }

}
