using System.Collections;
using UnityEngine;

public class BossController2 : Enemy
{
	#region General Variables
    public enum BossState
    {
        Idle,
        Run,
        Jump,
        Dive,
        Lunge,
        Cast,
        BendDown,
        Bounce1,
        Bounce2,
        Stunned,
        Parry,
        Slash
    }

    private BossState currentState;
    private Animator animator;
    //private Rigidbody2D rb;

    public Health bossHealth; // Add this line
    
    public static BossController2 Instance;
    [SerializeField] GameObject slashEffect;
	
    [Header("Ground Check Settings:")]
    [SerializeField] public Transform groundCheckPoint; //point at which ground check happens
    [SerializeField] private float groundCheckY = 0.2f; //how far down from ground chekc point is Grounded() checked
    [SerializeField] private float groundCheckX = 0.5f; //how far horizontally from ground chekc point to the edge of the player is
    [SerializeField] private LayerMask whatIsGround; //sets the ground layer
    [Space(5)]
	
    [Header("Attack Settings:")]
    [SerializeField] public Transform SideAttackTransform; //the middle of the side attack area
    [SerializeField] public Vector2 SideAttackArea; //how large the area of side attack is
    [SerializeField] public Transform UpAttackTransform; //the middle of the up attack area
    [SerializeField] public Vector2 UpAttackArea; //how large the area of side attack is
    [SerializeField] public Transform DownAttackTransform; //the middle of the down attack area
    [SerializeField] public Vector2 DownAttackArea; //how large the area of down attack is

    public float attackRange;
    public float attackTimer;
    float bloodCountDown;
    float bloodTimer;
		
    [Space(5)]
	
    [HideInInspector] public bool facingRight = true;
	
    int hitCounter;
    bool stunned, canStun;
    bool alive;
	
    [HideInInspector] public float runSpeed;
	
    public GameObject impactParticle;
    #endregion
    
    #region Unity Methods
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    protected override void Start()
    {
	    base.Start();
//        animator = GetComponent<Animator>();
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
        {
	        //Debug.LogError("No Animator component found on this game object.");
	        return;
        }
        rb = GetComponent<Rigidbody2D>();
        currentState = BossState.Idle;
        
        ChangeState(EnemyStates.Boss_Stage1);
        //ChangeState(EnemyStates.Boss_Stage2);
        //ChangeState(EnemyStates.Boss_Stage3);
        //ChangeState(EnemyStates.Boss_Stage4);
        alive = true;
        attacking = false;
        
        bossHealth = GetComponent<Health>(); // Add this line
        //StartCoroutine(ChangeBossStage());
    }

    protected override void Update()
    {
        base.Update();
        
        UpdateEnemyStates();
		
        if(bossHealth.HealthValue <= 0 && alive)
        {
	        Death(0);
        }
		
        if (!attacking)
        {
	        attackCountdown -= Time.deltaTime;
	        //Debug.Log("attackCountdown: " + attackCountdown);
	        if (attackCountdown <= 0)
	        {
		        AttackHandler();
		        attackCountdown = attackTimer; // Reset the countdown to the attack timer
	        }
        }
		
        if(stunned)
        {
	        rb.velocity = Vector2.zero;
        }
		
        bloodCountDown -= Time.deltaTime;
        if(bloodCountDown <= 0 && (currentEnemyState != EnemyStates.Boss_Stage1 && 
                                   currentEnemyState != EnemyStates.Boss_Stage2))
        {
	        GameObject _orangeBlood = Instantiate(orangeBlood, groundCheckPoint.position, Quaternion.identity);
	        Destroy(_orangeBlood, 4f);
	        bloodCountDown = bloodTimer;
        }
        
        // Check the state of each boolean and trigger
        if (animator.GetBool("Run"))
        {
	        //Debug.Log("Run state is being called.");
	        currentState = BossState.Run;
        }
        else if (animator.GetBool("Stunned"))
        {
	        //Debug.Log("Stunned state is being called.");
	        currentState = BossState.Stunned;
        }
        else if (animator.GetBool("Lunge"))
        {
	        //Debug.Log("Lunge state is being called.");
	        currentState = BossState.Lunge;
        }
        else if (animator.GetBool("Jump"))
        {
	        //Debug.Log("Jump state is being called.");
	        currentState = BossState.Jump;
        }
        else if (animator.GetBool("Cast"))
        {
	        //Debug.Log("Cast state is being called.");
	        currentState = BossState.Cast;
        }
        else if (animator.GetBool("Dive"))
        {
	        //Debug.Log("Dive state is being called.");
	        currentState = BossState.Dive;
        }
        else if (animator.GetBool("Parry"))
        {
	        //Debug.Log("Parry state is being called.");
	        currentState = BossState.Parry;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Slash"))
        {
	        //Debug.Log("Slash state is being called.");
	        currentState = BossState.Slash;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("BendDown"))
        {
	        //Debug.Log("BendDown state is being called.");
	        currentState = BossState.BendDown;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Bounce1"))
        {
	        //Debug.Log("Bounce1 state is being called.");
	        currentState = BossState.Bounce1;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Bounce2"))
        {
	        //Debug.Log("Bounce2 state is being called.");
	        currentState = BossState.Bounce2;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
        {
	        //Debug.Log("Idle state is being called.");
	        currentState = BossState.Idle;
        }
        else
        {
	        //Debug.Log("Idle state is being called.");
	        currentState = BossState.Idle;
        }
        
        switch (currentState)
        {
	        case BossState.Idle:
		        StartCoroutine(Idle());
		        break;
	        case BossState.Run:
		        rb.velocity = new Vector2(runSpeed, rb.velocity.y);
		        StartCoroutine(Run());
		        break;
	        case BossState.Jump:
		        Debug.Log("Jump state is being called.");
		        StartCoroutine(Jump());
		        break;
	        case BossState.Dive:
		        Debug.Log("Dive state is being called.");
		        StartCoroutine(Dive());
		        break;
	        case BossState.Lunge:
		        Debug.Log("Lunge state is being called.");
		        StartCoroutine(Lunge());
		        break;
	        case BossState.Cast:
		        Debug.Log("Cast state is being called.");
		        StartCoroutine(Cast());
		        break;
	        case BossState.BendDown:
		        Debug.Log("BendDown state is being called.");
		        StartCoroutine(BendDown());
		        break;
	        case BossState.Bounce1:
		        Debug.Log("Bounce1 state is being called.");
		        StartCoroutine(Bounce1());
		        break;
	        case BossState.Bounce2:
		        Debug.Log("Bounce2 state is being called.");
		        StartCoroutine(Bounce2());
		        break;
        }
        
        #region health to state
        if(bossHealth.HealthValue > 80)
        {
	        //Debug.Log("Stage 1");
	        ChangeState(EnemyStates.Boss_Stage1);
        }
        else if(bossHealth.HealthValue <= 80 && bossHealth.HealthValue > 50)
        {
	        //Debug.Log("Stage 2");
	        ChangeState(EnemyStates.Boss_Stage2);
        }
        else if(bossHealth.HealthValue <= 50 && bossHealth.HealthValue > 25)
        {
	        //Debug.Log("Stage 3");
	        ChangeState(EnemyStates.Boss_Stage3);
        }
        else if(bossHealth.HealthValue <= 25)
        {
	        //Debug.Log("Stage 4");
	        ChangeState(EnemyStates.Boss_Stage4);
        }
        if(bossHealth.HealthValue <= 0)
        {
	        Death(0);
        }
        #endregion
		
        
    }
    #endregion
    
    #region Methods
    // test function
    /*
    private IEnumerator ChangeBossStage()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f); // Wait for 10 seconds

            // Change the boss stage based on the current stage
            switch (GetCurrentEnemyState)
            {
                case EnemyStates.Boss_Stage1:
                    ChangeState(EnemyStates.Boss_Stage2);
                    break;
                case EnemyStates.Boss_Stage2:
                    ChangeState(EnemyStates.Boss_Stage3);
                    break;
                case EnemyStates.Boss_Stage3:
                    ChangeState(EnemyStates.Boss_Stage4);
                    break;
                case EnemyStates.Boss_Stage4:
                    ChangeState(EnemyStates.Boss_Stage1);
                    break;
            }
        }
    }*/

    public void ChangeState(BossState newState)
    {
        if (currentState != newState)
        {
            StopAllCoroutines(); // Stop the current state coroutine
            currentState = newState;
            StartCoroutine(newState.ToString()); // Start the new state coroutine
        }
    }
    
    public bool Grounded()
    {
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround) 
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround) 
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround))
        {
	        //Debug.Log("Grounded");

	        return true;
        }
        else
        {
            return false;
        }
    }
	
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(SideAttackTransform.position, SideAttackArea);
        Gizmos.DrawWireCube(UpAttackTransform.position, UpAttackArea);
        Gizmos.DrawWireCube(DownAttackTransform.position, DownAttackArea);
    }
	
    public void Flip()
    {
        // Get the local scale of the boss object
        Vector3 scale = transform.localScale;

        // If the player is to the left of the boss and the boss is facing right
        if (PlayerController.Instance.transform.position.x < transform.position.x && scale.x > 0)
        {
            // Flip the boss by negating the x scale
            scale.x = -scale.x;
            // Update the local scale
            transform.localScale = scale;
            facingRight = false; // Update facingRight
        }
        // If the player is to the right of the boss and the boss is facing left
        else if (PlayerController.Instance.transform.position.x > transform.position.x && scale.x < 0)
        {
            // Flip the boss by negating the x scale
            scale.x = -scale.x;
            // Update the local scale
            transform.localScale = scale;
            facingRight = true; // Update facingRight
        }
    }
    
    protected override void UpdateEnemyStates()
    {
        if(PlayerController.Instance != null)
        {
            switch(GetCurrentEnemyState)
            {
                case EnemyStates.Boss_Stage1:
                    canStun = true;
                    attackTimer = 6; // higher numbers = slower attack speed
                    runSpeed = speed;
                    break;
				
                case EnemyStates.Boss_Stage2:
                    canStun = true;
                    attackTimer = 5;
                    break;
				
                case EnemyStates.Boss_Stage3:
                    canStun = false;
                    attackTimer = 8;
                    bloodTimer = 5f;
                    break;
				
                case EnemyStates.Boss_Stage4:
                    canStun = false;
                    attackTimer = 10;
                    runSpeed = speed / 2;
                    bloodTimer = 1.5f;
                    break;
            }
        }
    }

    /*public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
	    if (!stunned)
	    {
		    if (!parrying)
		    {
			    if (canStun)
			    {
				    hitCounter++;
				    if (hitCounter >= 3) // number of hits to stun
				    {
					    ResetAllAttacks();
					    StartCoroutine(Stunned());
				    }
			    }

			    base.EnemyHit(_damageDone, _hitDirection, _hitForce);

			    if (currentEnemyState != EnemyStates.Boss_Stage4)
			    {
				    ResetAllAttacks(); // cancel attacks to avoid bugs
				    StartCoroutine(Parry());
			    }

		    }
		    else
		    {
			    StopCoroutine(Parry());
			    ResetAllAttacks();
			    StartCoroutine(Slash()); // riposte
		    }
	    }
	    else
	    {
		    StopCoroutine(Stunned());
		    animator.SetBool("Stunned", false);
		    stunned = false;
	    }
	    
	    #region health to state
	    if(bossHealth.HealthValue > 80)
	    {
		    Debug.Log("Stage 1");
		    ChangeState(EnemyStates.Boss_Stage1);
	    }
	    else if(bossHealth.HealthValue <= 80 && bossHealth.HealthValue > 50)
	    {
		    Debug.Log("Stage 2");
		    ChangeState(EnemyStates.Boss_Stage2);
	    }
	    else if(bossHealth.HealthValue <= 50 && bossHealth.HealthValue > 25)
	    {
		    Debug.Log("Stage 3");
		    ChangeState(EnemyStates.Boss_Stage3);
	    }
	    else if(bossHealth.HealthValue <= 25)
	    {
		    Debug.Log("Stage 4");
		    ChangeState(EnemyStates.Boss_Stage4);
	    }
	    if(bossHealth.HealthValue <= 0)
	    {
		    Death(0);
	    }
	    #endregion
    }*/

    IEnumerator Stunned()
	{
		stunned = true;
		hitCounter = 0;
		animator.SetBool("Stunned", true);
		
		yield return new WaitForSecondsRealtime(6f);
        
		animator.SetBool("Stunned", false);
		stunned = false;
	}
	
	public override void Death(float _destroyTime)
	{
		ResetAllAttacks();
		alive = false;
		rb.velocity = new Vector2(rb.velocity.x, -25);
		animator.SetTrigger("Die");
		bossHealth.Die();
		bloodTimer = 0.8f;
		DestroyAfterDeath();
	}
	
	public void DestroyAfterDeath()
	{
		Destroy(gameObject);
	}
	#endregion
    
    #region Combat Variables
    [HideInInspector] public bool attacking;
    [HideInInspector] public float attackCountdown;
    [HideInInspector] public bool damagedPlayer = false;
    [HideInInspector] public bool parrying;
	
    [HideInInspector] public Vector2 moveToPosition;
    [HideInInspector] public bool diveAttack;
    public GameObject divingCollider;
    public GameObject pillar;
	
    [HideInInspector] public bool barrageAttack;
    public GameObject barrageFireball;
    [HideInInspector] public bool outbreakAttack;
	
    [HideInInspector] public bool bounceAttack;
    [HideInInspector] public float rotationDirectionToTarget;
    [HideInInspector] public int bounceCount;
	
    #endregion
    
    #region Control
	
	public void AttackHandler()
	{
		//Debug.Log(" Inside AttackHandler .");
		if(currentEnemyState == EnemyStates.Boss_Stage1)
		{
			Debug.Log(" Inside Boss_Stage1 .");
			if(Vector2.Distance(PlayerController.Instance.transform.position, rb.position) <= attackRange)
			{
				//Debug.Log(" Calling TripleSlash .");
				StartCoroutine(TripleSlash());
			}
			else
			{
				//Debug.Log(" Calling Lunge");
				StartCoroutine(Lunge());
			}
		}
		
		if(currentEnemyState == EnemyStates.Boss_Stage2)
		{
			Debug.Log(" Inside Boss_Stage2 .");
			if(Vector2.Distance(PlayerController.Instance.transform.position, rb.position) <= attackRange)
			{
				StartCoroutine(TripleSlash());
			}
			else
			{
				int _attackChosen = Random.Range(1, 3);
				switch(_attackChosen)
				{
					case 1:
						StartCoroutine(Lunge());
						break;
					
					case 2:
						DiveAttackJump();
						break;
					
					case 3:
						BarrageBendDown();
						break;
				}
			}
		}
		
		if(currentEnemyState == EnemyStates.Boss_Stage3)
		{
			Debug.Log(" Inside Boss_Stage3 .");
			
			int _attackChosen = Random.Range(1, 4);
			
			if(_attackChosen == 1)
			{
                OutbreakBendDown();
            }

            if (_attackChosen == 2)
            {
                DiveAttackJump(); // bug
            }
            
			if (_attackChosen == 3)
            {
                BarrageBendDown();
            }
            
			if (_attackChosen == 4)
            {
                BounceAttack();
            }
			
        }
		
		if(currentEnemyState == EnemyStates.Boss_Stage4)
		{
			Debug.Log(" Inside Boss_Stage4 .");
			if(Vector2.Distance(PlayerController.Instance.transform.position, rb.position) <= attackRange)
			{
				StartCoroutine(Slash());
			}
			else
			{
				BounceAttack();
			}
		}
	}
	
	public void ResetAllAttacks()
	{
		attacking = false;
		
		StopCoroutine(TripleSlash());
		StopCoroutine(Lunge());
		StopCoroutine(Parry());
		StopCoroutine(Slash());
		
		diveAttack = false;
		barrageAttack = false;
		outbreakAttack = false;
		bounceAttack = false;
	}
	
	#endregion
    
    #region Stage 1
	
    IEnumerator TripleSlash()
    {
	    //Debug.Log("TripleSlash started. Animator is null: " + (animator == null));

	    if (animator == null)
	    {
		    Debug.LogError("No Animator component found on this game object.");
		    yield break;
	    }
        attacking = true;
        rb.velocity = Vector2.zero;
		
        animator.SetTrigger("Slash");
        SlashAngle();
        yield return new WaitForSecondsRealtime(0.3f);
        yield return new WaitForSecondsRealtime(0.3f);
        animator.ResetTrigger("Slash");
		
        animator.SetTrigger("Slash");
        SlashAngle();
        yield return new WaitForSecondsRealtime(0.5f);
        animator.ResetTrigger("Slash");
		
        animator.SetTrigger("Slash");
        SlashAngle();
        yield return new WaitForSecondsRealtime(0.2f);
        animator.ResetTrigger("Slash");
		
        ResetAllAttacks();
    }
	
    void SlashAngle()
    {
        // side attack
        if(PlayerController.Instance.transform.position.x > transform.position.x ||
           PlayerController.Instance.transform.position.x < transform.position.x)
        {
            Instantiate(slashEffect, SideAttackTransform);
        }
        // up attack
        else if(PlayerController.Instance.transform.position.y > transform.position.y)
        {
            Instantiate(slashEffect, UpAttackTransform);
        }
		
        // down attack
        else if(PlayerController.Instance.transform.position.y < transform.position.y)
        {
            Instantiate(slashEffect, DownAttackTransform);
        }
    }
	
    void SlashEffectAtAngle(GameObject _slashEffect, int _effectAngle, Transform _attackTransform)
    {
        _slashEffect = Instantiate(_slashEffect, _attackTransform);
        _slashEffect.transform.eulerAngles = new Vector3(0, 0, _effectAngle);
        _slashEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
    }
    
    //original code, used again above
    /*
    IEnumerator Lunge()
	{
	    Flip(); // Flip the boss if necessary
	    attacking = true;

	    animator.SetBool("Lunge", true);

	    Vector2 startPosition = rb.position;

	    Vector2 directionToPlayer = (PlayerController.Instance.transform.position - transform.position).normalized;

	    Vector2 targetPosition = new Vector2(startPosition.x + (facingRight ? 15f : -15f), startPosition.y);

	    float animationDuration = animator.GetCurrentAnimatorStateInfo(0).length;

	    float elapsedTime = 0f;
        
	    bool damageDealt = false;
        
	    while (elapsedTime < animationDuration)
	    {
	        float t = elapsedTime / animationDuration;
            
	        rb.MovePosition(Vector2.Lerp(startPosition, targetPosition, t));
            
	        if (!damageDealt && t >= 0.5f) // Adjust the timing as needed
	        {
	            DealDamageToPlayer();
	            damageDealt = true;
	        }
            
	        elapsedTime += Time.deltaTime;
            
	        yield return null;
	    }
        
	    animator.SetBool("Lunge", false);
        
	    attacking = false;
        
	    while (true)
	    {
	        rb.MovePosition(targetPosition);
            
	        yield return null;
	    }
	}
    
    

	void DealDamageToPlayer()
	{
	    // Check if the player is in range
	    if (Vector2.Distance(transform.position, PlayerController.Instance.transform.position) <= attackRange)
	    {
	        // Deal damage to the player
	        PlayerController.Instance.TakeDamage(damage);
	    }
	}*/



	IEnumerator Parry()
	{
		attacking = true;
		rb.velocity = Vector2.zero;
		
		animator.SetBool("Parry", true);
		yield return new WaitForSecondsRealtime(0.8f);
		animator.SetBool("Parry", false);
		
		parrying = false;
		ResetAllAttacks();
	}
	
	IEnumerator Slash()
	{
		attacking = true;
		rb.velocity = Vector2.zero;
		
		animator.SetTrigger("Slash");
		SlashAngle();
		yield return new WaitForSecondsRealtime(0.3f);
		animator.ResetTrigger("Slash");
		
		ResetAllAttacks();
	}
	
	#endregion
	
	#region Stage 2
	
	
	void DiveAttackJump()
	{
		attacking = true;
		moveToPosition = new Vector2(PlayerController.Instance.transform.position.x, rb.position.y + 10);
		diveAttack = true;
		animator.SetBool("Jump", true);
	}
	
	
	
	
	private void OnTriggerEnter2D(Collider2D _other)
	{
		if (_other.CompareTag("Player") && ((_other.transform.position.x < transform.position.x && facingRight) ||
		                                    (_other.transform.position.x > transform.position.x && !facingRight)))
		{
			PlayerController.Instance.TakeDamage(damage);
		}
	}

	
	
	public void DivingPillars()
	{
		Vector2 _impactPoint = groundCheckPoint.position;
		float _spawnDistance = 5;
		
		for(int i = 0; i < 10; i++)
		{
			Vector2 _pillarSpawnPointRight = _impactPoint + new Vector2(_spawnDistance, 0);
			Vector2 _pillarSpawnPointLeft = _impactPoint - new Vector2(_spawnDistance, 0);
			Instantiate(pillar, _pillarSpawnPointRight, Quaternion.Euler(0, 0, -90));
			Instantiate(pillar, _pillarSpawnPointLeft, Quaternion.Euler(0, 0, -90));
			
			_spawnDistance += 5;
		}
		ResetAllAttacks();
	}
	
	void BarrageBendDown()
	{
		attacking = true;
		rb.velocity = Vector2.zero;
		moveToPosition = new Vector2(transform.position.x, rb.position.y + 5);

		barrageAttack = true;
		animator.SetTrigger("BendDown");
	}
	
	public IEnumerator Barrage()
	{
		//rb.velocity = Vector2.zero;
		rb.velocity = new Vector2(rb.velocity.x, 0);
		
		float _currentAngle = 30f;
		for(int i = 0; i < 10; i++)
		{
			GameObject _projectile = Instantiate(barrageFireball, transform.position, Quaternion.Euler(0, 0, _currentAngle));
			
			if(facingRight)
			{
				_projectile.transform.eulerAngles = new Vector3(_projectile.transform.eulerAngles.x, 0, _currentAngle);
			}
			else
			{
				_projectile.transform.eulerAngles = new Vector3(_projectile.transform.eulerAngles.x, 180, _currentAngle);
			}
			
			_currentAngle += 5f;
			
			yield return new WaitForSecondsRealtime(0.4f);
		}
		yield return new WaitForSecondsRealtime(0.1f);
		animator.SetBool("Cast", false);
		ResetAllAttacks();
	}
	
	#endregion
    
	#region Stage 3
	
	void OutbreakBendDown()
	{
		attacking = true;
		rb.velocity = Vector2.zero;
		moveToPosition = new Vector2(transform.position.x, rb.position.y + 5);
		outbreakAttack = true;
		animator.SetTrigger("BendDown");
	}
	
	public IEnumerator Outbreak()
	{
        yield return new WaitForSecondsRealtime(1f);
        animator.SetBool("Cast", true);
		
		//rb.velocity = Vector2.zero;
		rb.velocity = new Vector2(rb.velocity.x, 0);

		for(int i = 0; i < 30; i++)
		{
			Instantiate(barrageFireball, transform.position, Quaternion.Euler(0, 0, Random.Range(110, 130))); // downwards
			Instantiate(barrageFireball, transform.position, Quaternion.Euler(0, 0, Random.Range(50, 70))); // diagonally right
			Instantiate(barrageFireball, transform.position, Quaternion.Euler(0, 0, Random.Range(260, 280))); // diagonally left
			
			yield return new WaitForSecondsRealtime(0.2f);
        }
		yield return new WaitForSecondsRealtime(0.1f);
        
        rb.constraints = RigidbodyConstraints2D.None;
		rb.constraints = RigidbodyConstraints2D.FreezeRotation;
		rb.velocity = new Vector2(rb.velocity.x, -10);
		yield return new WaitForSecondsRealtime(0.1f);
		animator.SetBool("Cast", false);
		ResetAllAttacks();
	}
	
	public void BounceAttack()
	{
		attacking = true;
		bounceCount = Random.Range(2, 5);
		BounceBendDown();
	}
	int _bounces = 0;
	
	public void CheckBounce()
	{
		if(_bounces < bounceCount - 1)
		{
			_bounces++;
			BounceBendDown();
		}
		else
		{
			_bounces = 0;
			animator.Play("Boss_Run");
		}
	}
	
	public void BounceBendDown()
	{
		rb.velocity = Vector2.zero;
		moveToPosition = new Vector2(PlayerController.Instance.transform.position.x, rb.position.y + 10);
		bounceAttack = true;
		animator.SetTrigger("BendDown");
	}
	
	public void CalculateTargetAngle()
	{
		Vector3 _directionToTarget = (PlayerController.Instance.transform.position - transform.position).normalized;
		float _angleOfTarget = Mathf.Atan2(_directionToTarget.y, _directionToTarget.x) * Mathf.Rad2Deg;
		rotationDirectionToTarget = _angleOfTarget;
	}
	
	#endregion
	
    #region Boss States
    
    
    IEnumerator Idle()
    {
        
        while (currentState == BossState.Idle)
        {
	        //attackCountdown -= Time.deltaTime;
            rb.velocity = Vector2.zero;
            RunToPlayer(animator);

            if (attackCountdown <= 0)
            {
	            //Debug.Log("AttackHandler is being called inside Idle().");
                AttackHandler();
                attackCountdown = Random.Range(attackTimer - 1, attackTimer + 1);
                //Debug.Log("attackCountdown: " + attackCountdown);
            }
            yield return null;
        }
        
    }
	
    /*void RunToPlayer(Animator animator)
    {
        if (Vector2.Distance(PlayerController.Instance.transform.position, rb.position) >= attackRange)
        {
            animator.SetBool("Run", true);
        }
        else
        {
            return;
        }
    }*/
    void RunToPlayer(Animator animator)
    {
	    if (Vector2.Distance(PlayerController.Instance.transform.position, rb.position) >= attackRange)
	    {
		    // Determine the direction to the player
		    float directionToPlayer = PlayerController.Instance.transform.position.x - transform.position.x;

		    // Flip the boss's sprite based on the direction to the player
		    if (directionToPlayer > 0 && !facingRight)
		    {
			    Flip();
		    }
		    else if (directionToPlayer < 0 && facingRight)
		    {
			    Flip();
		    }

		    animator.SetBool("Run", true);
	    }
	    else
	    {
		    return;
	    }
    }

    IEnumerator Run()
    {
        animator.SetBool("Run", true);
        while (currentState == BossState.Run)
        {
	        //attackCountdown -= Time.deltaTime;
            TargetPlayerPosition(animator);

            if (attackCountdown <= 0)
            {
	            //Debug.Log("AttackHandler is being called inside Run().");
                AttackHandler();
                attackCountdown = Random.Range(attackTimer - 1, attackTimer + 1);
            }
            // Reset the velocity
            
            //Debug.Log("Run speed: " + runSpeed);

            yield return null;
        }
        animator.SetBool("Run", false);
    }
    
    void TargetPlayerPosition(Animator animator)
    {
        if (Grounded())
        {
            Flip();
            Vector2 _target = new Vector2(PlayerController.Instance.transform.position.x, rb.position.y);
            Vector2 _newPos = Vector2.MoveTowards(rb.position, _target, runSpeed * Time.fixedDeltaTime);
            runSpeed = speed;
            rb.MovePosition(_newPos);
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, -25);
        }

        if (Vector2.Distance(PlayerController.Instance.transform.position, rb.position) <= attackRange)
        {
            animator.SetBool("Run", false);
        }
    }

    IEnumerator Jump()
    {
        animator.SetBool("Jump", true);
        while (currentState == BossState.Jump)
        {
            DiveAttack();
            animator.SetBool("Jump", false); // Set Jump to false after the jump action is completed
            yield return null;
        }
        animator.SetBool("Jump", false);
    }
    
    void DiveAttack()
    {
        if (diveAttack)
        {
            Flip();
            
            Vector2 _newPos = Vector2.MoveTowards(rb.position, moveToPosition, speed * Time.fixedDeltaTime);

            rb.MovePosition(_newPos);

            float _distance = Vector2.Distance(rb.position, _newPos);
            //Debug.Log("Distance: " + _distance);
            if (_distance < 1f)
            {
                Dive();
                //Debug.Log("Boss is reaching the target position. Current position: " + rb.position + ", Target position: " + moveToPosition);
            }
            
        }
        
        
    }

    IEnumerator Dive()
    {
        animator.SetBool("Dive", true);
        while (currentState == BossState.Dive)
        {
            //Debug.Log("DiveAttack is being called.");
            divingCollider.SetActive(true);
            //Debug.Log("Diving collider is set to true.");
        
            //Debug.Log("Boss is diving.");
            if (Grounded())
            {
                //Debug.Log("Boss is grounded.");
                divingCollider.SetActive(false);
                //Debug.Log("Diving collider is set to false.");

                if (!callOnce)
                {
                    //Debug.Log("DiveAttack is being called once.");
                    GameObject _impactParticle = Instantiate(impactParticle,
                        groundCheckPoint.position, Quaternion.identity);
                    Destroy(_impactParticle, 4f);
                    DivingPillars();
                    animator.SetBool("Dive", false);
                    ResetAllAttacks();
                    callOnce = true;
                }
            }
            else
            {
                animator.SetBool("Dive", false);
            }

            // Set DiveAttack to false after the dive attack action is completed
            diveAttack = false;
            yield return null;
        }
        animator.SetBool("Dive", false);
        callOnce = false;
    }

    /*private IEnumerator Lunge()
    {
        animator.SetBool("Lunge", true);
        while (currentState == BossState.Lunge)
        {
            TargetPlayerPosition(animator);
            
            rb.gravityScale = 0;
            int _dir = facingRight ? 1 : -1;
            rb.velocity = new Vector2(_dir * (speed * 5), 0f);

            if (Vector2.Distance(PlayerController.Instance.transform.position, rb.position) <=
                attackRange && !damagedPlayer)
            {
                PlayerController.Instance.TakeDamage(damage);
                damagedPlayer = true;
            }
            yield return null;
        }
        animator.SetBool("Lunge", false);
    }*/
    
    //mixture of both Lunge()
    IEnumerator Lunge()
    {
	    //Debug.Log("Lunge started. Animator is null: " + (animator == null));

	    if (animator == null)
	    {
		    Debug.LogError("No Animator component found on this game object.");
		    yield break;
	    }
        Flip(); // Flip the boss if necessary
        attacking = true;
		//Debug.Log("attacking: " + attacking);
        // Set the "Lunge" animation trigger
        animator.SetBool("Lunge", true);
		//Debug.Log("Lunge animation trigger is set to true.");
        // Get the starting position of the boss
        Vector2 startPosition = rb.position;

        // Calculate the direction to the player
        Vector2 directionToPlayer = (PlayerController.Instance.transform.position - transform.position).normalized;

        // Calculate the target position by moving 15 units towards the player on the x-axis
        Vector2 targetPosition = new Vector2(startPosition.x + (facingRight ? 15f : -15f), startPosition.y);

        // Get the duration of the animation
        float animationDuration = animator.GetCurrentAnimatorStateInfo(0).length;

        // Define the time elapsed
        float elapsedTime = 0f;

        // Define if the damage has been dealt
        bool damageDealt = false;

        // Move the boss gradually towards the target position
        while (elapsedTime < animationDuration)
        {
            // Calculate the interpolation factor
            float t = elapsedTime / animationDuration;

            // Interpolate the boss position between the start and target positions
            rb.MovePosition(Vector2.Lerp(startPosition, targetPosition, t));

            // Check if it's time to deal damage
            if (!damageDealt && t >= 0.5f) // Adjust the timing as needed
            {
                DealDamageToPlayer();
                damageDealt = true;
            }

            // Update the elapsed time
            elapsedTime += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Reset the "Lunge" animation trigger
        animator.SetBool("Lunge", false);

        // Reset attacking state
        attacking = false;

        // Freeze the boss's position until the coroutine completes
        /*while (true)
        {
            // Ensure the boss stays at the target position
            rb.MovePosition(targetPosition);

            // Wait for the next frame
            yield return null;
        }*/
        // Freeze the boss's position for a certain duration after the lunge attack
        float freezeDuration = 1f; // Adjust this value as needed
        float freezeStartTime = Time.time;

        while (Time.time - freezeStartTime < freezeDuration)
        {
	        rb.MovePosition(targetPosition);
	        yield return null;
        }
    }

    void DealDamageToPlayer()
    {
        // Check if the player is in range
        if (Vector2.Distance(transform.position, PlayerController.Instance.transform.position) <= attackRange)
        {
            // Deal damage to the player
            PlayerController.Instance.TakeDamage(damage);
        }
    }

    IEnumerator Cast()
    {
        animator.SetBool("Cast", true);
        while (currentState == BossState.Cast)
        {
            // Cast state logic...
            yield return null;
        }
        animator.SetBool("Cast", false);
    }

    IEnumerator BendDown()
    {
        animator.SetBool("BendDown", true);
        while (currentState == BossState.BendDown)
        {
            OutbreakAttack();
            
            yield return null;
        }
        animator.ResetTrigger("BendDown");

    }
    
    void OutbreakAttack()
    {
        if (outbreakAttack)
        {
            Vector2 _newPos = Vector2.MoveTowards(rb.position, moveToPosition,
                speed * 1.5f * Time.fixedDeltaTime);
            rb.MovePosition(_newPos);

            float _distance = Vector2.Distance(rb.position, _newPos);
            if (_distance < 0.1f)
            {
                rb.constraints = RigidbodyConstraints2D.FreezePosition;
            }
        }
    }

    IEnumerator Bounce1()
    {
        animator.SetBool("Bounce1", true);
        while (currentState == BossState.Bounce1)
        {
            if (bounceAttack)
            {
                Vector2 _newPos = Vector2.MoveTowards(rb.position, moveToPosition,
                    speed * Random.Range(2, 4) * Time.fixedDeltaTime);
                rb.MovePosition(_newPos);

                float _distance = Vector2.Distance(rb.position, _newPos);
                if (_distance < 0.1f)
                {
                    CalculateTargetAngle();
                    animator.SetTrigger("Bounce2");
                }
            }
            yield return null;
        }
        animator.ResetTrigger("Bounce1");
    }

    bool callOnce;
    
    IEnumerator Bounce2()
    {
        animator.SetBool("Bounce2", true);
        while (currentState == BossState.Bounce2)
        {
            Vector2 _forceDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * rotationDirectionToTarget),
                Mathf.Sin(Mathf.Deg2Rad * rotationDirectionToTarget));
            rb.AddForce(_forceDirection * 3, ForceMode2D.Impulse);

            divingCollider.SetActive(true);

            if (Grounded())
            {
                divingCollider.SetActive(false);
                if (!callOnce)
                {
                    GameObject _impactParticle = Instantiate(impactParticle,
                        groundCheckPoint.position, Quaternion.identity);
                    Destroy(_impactParticle, 4f);

                    ResetAllAttacks();
                    CheckBounce();
                    callOnce = true;
                }

                animator.SetTrigger("Grounded");
            }
            yield return null;
        }
        animator.ResetTrigger("Bounce2");
        animator.ResetTrigger("Grounded");
        callOnce = false;
    }
    #endregion

    
}