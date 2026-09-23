using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : Enemy
{
	public Health bossHealth; // Add this line
	private BossEvents bossEvents;
	
	public static BossController Instance;
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
		
	[Space(5)]
	
	[HideInInspector] public bool facingRight = true;
	
	int hitCounter;
	bool stunned, canStun;
	bool alive;
	
	[HideInInspector] public float runSpeed;
	
	public GameObject impactParticle;
	
	
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
		sr = GetComponentInChildren<SpriteRenderer>();
		anim = GetComponentInChildren<Animator>();
		ChangeState(EnemyStates.Boss_Stage1);
		//ChangeState(EnemyStates.Boss_Stage2);
		//ChangeState(EnemyStates.Boss_Stage3);
		//ChangeState(EnemyStates.Boss_Stage4);
		alive = true;
		//bossEvents = GetComponent<BossEvents>();
		//StartCoroutine(ChangeBossStage());
	}
	
	public bool Grounded()
    {
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround) 
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround) 
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround))
        {
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
	
	float bloodCountDown;
	float bloodTimer;
	
	protected override void Update()
	{
		base.Update();
		
		UpdateEnemyStates();
		
		if(health <= 0 && alive)
		{
			Death(0);
		}
		
		if (!attacking)
		{
			attackCountdown -= Time.deltaTime;
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
		
		
	}
	
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
	}

	
	/* original
	 public void Flip()
	{
		if(PlayerController.Instance.transform.position.x < transform.position.x && transform.position.localScale.x > 0)
		{
			transform.eulerAngles = new Vector2(transform.eulerAngles.x, 180);
            //transform.localScale = new Vector2(-1, transform.localScale.y);
            //transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            facingRight = false;
		}
		else //if (PlayerController.Instance.transform.position.x > transform.position.x && transform.localScale.x < 0)
        {
            transform.eulerAngles = new Vector2(transform.eulerAngles.x, 0);
            //transform.localScale = new Vector2(1, transform.localScale.y);
            //transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);

            facingRight = true;
		}
	}*/

    /* chat gpt
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
            facingRight = false;
        }
        // If the player is to the right of the boss and the boss is facing left
        else if (PlayerController.Instance.transform.position.x > transform.position.x && scale.x < 0)
        {
            // Flip the boss by negating the x scale
            scale.x = -scale.x;
            // Update the local scale
            transform.localScale = scale;
            facingRight = true;
        }
    }*/
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
				attackTimer = 1; // higher numbers = slower attack speed
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
	
	protected override void OnCollisionStay2D(Collision2D _other)
	{
		base.OnCollisionStay2D(_other);
	}
	
	#region attacking
	#region variables
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
		
		
		if(currentEnemyState == EnemyStates.Boss_Stage1)
		{
			if(Vector2.Distance(PlayerController.Instance.transform.position, rb.position) <= attackRange)
			{
				StartCoroutine(TripleSlash());
			}
			else
			{
				//StartCoroutine(Lunge());
				int _attackChosen = Random.Range(1, 4);
				switch(_attackChosen)
				{
					case 1:
						
						StartCoroutine(Lunge());
						break;
				
					case 2:
						//DiveAttackJump();
						BounceAttack();
						break;
				
					case 3:
						BarrageBendDown();
						break;

					/*case 4:
						
						OutbreakBendDown();
						break;*/
				}
			}
		}
		
		if(currentEnemyState == EnemyStates.Boss_Stage2)
		{
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
			
			
			int _attackChosen = Random.Range(1, 4);
			
			if(_attackChosen == 1)
			{
                OutbreakBendDown();
            }

            if (_attackChosen == 2)
            {
                //DiveAttackJump(); // bug
                StartCoroutine(Lunge());
            }
            
			if (_attackChosen == 3)
            {
                BarrageBendDown();
            }
            
			if (_attackChosen == 4)
            {
                BounceAttack();
            }
			
            /*
             switch(_attackChosen)
			{
				case 1:
					OutbreakBendDown();
					break;
				
				case 2:
					//DiveAttackJump();
					StartCoroutine(Lunge());
					break;
				
				case 3:
					BarrageBendDown();
					break;

				case 4:
					BounceAttack();
					break;
			}*/

        }
		
		if(currentEnemyState == EnemyStates.Boss_Stage4)
		{
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
		attacking = true;
		rb.velocity = Vector2.zero;
		
		anim.SetTrigger("Slash");
		SlashAngle();
		yield return new WaitForSecondsRealtime(0.3f);
        yield return new WaitForSecondsRealtime(0.3f);
        anim.ResetTrigger("Slash");
		
		anim.SetTrigger("Slash");
		SlashAngle();
		yield return new WaitForSecondsRealtime(0.5f);
		anim.ResetTrigger("Slash");
		
		anim.SetTrigger("Slash");
		SlashAngle();
		yield return new WaitForSecondsRealtime(0.2f);
		anim.ResetTrigger("Slash");
		
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
	
	
	/*IEnumerator Lunge()
	{
		Flip(); // Flip the boss if necessary
		attacking = true;
    
		// Set the "Lunge" animation trigger
		anim.SetBool("Lunge", true);
    
		// Wait for the duration of the animation
		yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
    
		// After the animation is finished, reset the "Lunge" trigger
		anim.SetBool("Lunge", false);
		damagedPlayer = false;
		attacking = false;
		
		ResetAllAttacks();
	}*/
	IEnumerator Lunge()
	{
	    Flip(); // Flip the boss if necessary
	    attacking = true;

	    // Set the "Lunge" animation trigger
	    anim.SetBool("Lunge", true);

	    // Get the starting position of the boss
	    Vector2 startPosition = rb.position;

	    // Calculate the direction to the player
	    Vector2 directionToPlayer = (PlayerController.Instance.transform.position - transform.position).normalized;

	    // Calculate the target position by moving 15 units towards the player on the x-axis
	    Vector2 targetPosition = new Vector2(startPosition.x + (facingRight ? 15f : -15f), startPosition.y);

	    // Get the duration of the animation
	    float animationDuration = anim.GetCurrentAnimatorStateInfo(0).length;

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
	    anim.SetBool("Lunge", false);

	    // Reset attacking state
	    attacking = false;

	    
	    // Freeze the boss's position until the coroutine completes
	    //while (true)
	    //{
	        // Ensure the boss stays at the target position
	       // rb.MovePosition(targetPosition);

	        // Wait for the next frame
	      //  yield return null;
	    //}
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



	IEnumerator Parry()
	{
		attacking = true;
		rb.velocity = Vector2.zero;
		
		anim.SetBool("Parry", true);
		yield return new WaitForSecondsRealtime(0.8f);
		anim.SetBool("Parry", false);
		
		parrying = false;
		ResetAllAttacks();
	}
	
	IEnumerator Slash()
	{
		attacking = true;
		rb.velocity = Vector2.zero;
		
		anim.SetTrigger("Slash");
		SlashAngle();
		yield return new WaitForSecondsRealtime(0.3f);
		anim.ResetTrigger("Slash");
		
		ResetAllAttacks();
	}
	
	#endregion
	#region Stage 2
	
	// Define an enum for the boss states
	public enum BossState
	{
		Idle,
		Jump,
		Dive,
		Run
	}
	public BossState currentState;
	
	
	void DiveAttackJump()
	{
		attacking = true;
		moveToPosition = new Vector2(PlayerController.Instance.transform.position.x, rb.position.y + 10);
		diveAttack = true;
		//anim.SetBool("Jump", true);
		Dive();
	}
	
	public void Dive()
	{
		anim.SetBool("Dive", true);
		Debug.Log("DiveAttack is set to true.");
		//anim.SetBool("Jump", false);
		//GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -10), ForceMode2D.Force);
		anim.SetBool("Dive", false);
		//Debug.Log("DiveAttack is set to false.");
	}
	
	/*private void OnTriggerEnter2D(Collider2D _other)
	{
		if(_other.GetComponent<PlayerController>() != null && (diveAttack || bounceAttack))
		{
			_other.GetComponent<PlayerController>().TakeDamage(damage * 2);
			PlayerController.Instance.pState.recoilingX = true;
		}
	}*/
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
		anim.SetTrigger("BendDown");
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
		anim.SetBool("Cast", false);
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
		anim.SetTrigger("BendDown");
	}
	
	public IEnumerator Outbreak()
	{
        yield return new WaitForSecondsRealtime(1f);
        anim.SetBool("Cast", true);
		
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
		anim.SetBool("Cast", false);
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
			anim.Play("Boss_Run");
		}
	}
	
	public void BounceBendDown()
	{
		rb.velocity = Vector2.zero;
		moveToPosition = new Vector2(PlayerController.Instance.transform.position.x, rb.position.y + 10);
		bounceAttack = true;
		anim.SetTrigger("BendDown");
	}
	
	public void CalculateTargetAngle()
	{
		Vector3 _directionToTarget = (PlayerController.Instance.transform.position - transform.position).normalized;
		float _angleOfTarget = Mathf.Atan2(_directionToTarget.y, _directionToTarget.x) * Mathf.Rad2Deg;
		rotationDirectionToTarget = _angleOfTarget;
	}
	
	#endregion
	
	#endregion
	
	public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
	{
		if(!stunned)
		{
			if(!parrying)
			{
				if(canStun)
				{
					hitCounter++;
					if(hitCounter >= 3) // number of hits to stun
					{
						ResetAllAttacks();
						StartCoroutine(Stunned());
					}
				}
				base.EnemyHit(_damageDone, _hitDirection, _hitForce);
				
				if(currentEnemyState != EnemyStates.Boss_Stage4)
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
			anim.SetBool("Stunned", false);
			stunned = false;
		}
		/*
		#region health to state
		if(health > 20)
		{
			ChangeState(EnemyStates.Boss_Stage1);
		}
		if(health <= 15 && health < 10)
		{
			ChangeState(EnemyStates.Boss_Stage2);
		}
		if(health <= 10 && health < 5)
		{
			ChangeState(EnemyStates.Boss_Stage3);
		}
		if(health < 10)
		{
			ChangeState(EnemyStates.Boss_Stage4);
		}
		if(health <= 0)
		{
			Death(0);
		}
		#endregion
		*/
		
		
	}
	
	public IEnumerator Stunned()
	{
		stunned = true;
		hitCounter = 0;
		anim.SetBool("Stunned", true);
		
		yield return new WaitForSecondsRealtime(6f);
        
        anim.SetBool("Stunned", false);
		stunned = false;
	}
	
	public override void Death(float _destroyTime)
	{
		ResetAllAttacks();
		alive = false;
		rb.velocity = new Vector2(rb.velocity.x, -25);
		anim.SetTrigger("Die");
		bloodTimer = 0.8f;
	}
	
	public void DestroyAfterDeath()
	{
		Destroy(gameObject);
	}
}