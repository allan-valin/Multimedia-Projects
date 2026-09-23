using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{
    
    
    
    [Header("Horizontal Movement Settings:")]
    [SerializeField] private float walkSpeed = 1; //sets the players movement speed on the ground
    [Space(5)]


    [Header("Vertical Movement Settings")]
    [SerializeField] private float jumpForce = 45f; //sets how hight the player can jump
    private int jumpBufferCounter = 0; //stores the jump button input
    [SerializeField] private int jumpBufferFrames; //sets the max amount of frames the jump buffer input is stored
    private float coyoteTimeCounter = 0; //stores the Grounded() bool
    [SerializeField] private float coyoteTime; ////sets the max amount of frames the Grounded() bool is stored
    private int airJumpCounter = 0; //keeps track of how many times the player has jumped in the air
    [SerializeField] private int maxAirJumps; //the max no. of air jumps
    private float gravity; //stores the gravity scale at start
    [Space(5)]

    [Header("Wall Jump Settings")]
    [SerializeField] private float wallSlidingSpeed = 2f;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallJumpingDuration;
    [SerializeField] private Vector2 wallJumpingPower;
    float wallJumpingDirection;
    bool isWallSliding;
    bool isWallJumping;
    [Space(5)]

    [Header("Ground Check Settings:")]
    [SerializeField] private Transform groundCheckPoint; //point at which ground check happens
    [SerializeField] private float groundCheckY = 0.2f; //how far down from ground chekc point is Grounded() checked
    [SerializeField] private float groundCheckX = 0.5f; //how far horizontally from ground chekc point to the edge of the player is
    [SerializeField] private LayerMask whatIsGround; //sets the ground layer
    [Space(5)]


    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed; //speed of the dash
    [SerializeField] private float dashTime; //amount of time spent dashing
    [SerializeField] private float dashCooldown; //amount of time between dashes
    [SerializeField] GameObject dashEffect;
    private bool canDash = true, dashed;
    [Space(5)]


    [Header("Attack Settings:")]
    [SerializeField] private Transform SideAttackTransform; //the middle of the side attack area
    [SerializeField] private Vector2 SideAttackArea; //how large the area of side attack is
    [SerializeField] private Transform UpAttackTransform; //the middle of the up attack area
    [SerializeField] private Vector2 UpAttackArea; //how large the area of side attack is
    [SerializeField] private Transform DownAttackTransform; //the middle of the down attack area
    [SerializeField] private Vector2 DownAttackArea; //how large the area of down attack is
    [SerializeField] private LayerMask attackableLayer; //the layer the player can attack and recoil off of
    private float timeSinceAttack;
    [SerializeField] private float timeBetweenAttack = 1f; //the time between attacks
    [SerializeField] private float damage; //the damage the player does to an enemy
    [SerializeField] private GameObject slashEffect; //the effect of the slashs
    [Space(5)]

    // time restoration
    bool restoreTime;
    float restoreTimeSpeed;
    [Space(5)]


    [Header("Recoil Settings:")]
    [SerializeField] private int recoilXSteps = 5; //how many FixedUpdates() the player recoils horizontally for
    [SerializeField] private int recoilYSteps = 5; //how many FixedUpdates() the player recoils vertically for
    [SerializeField] private float recoilXSpeed = 100; //the speed of horizontal recoil
    [SerializeField] private float recoilYSpeed = 100; //the speed of vertical recoil
    private int stepsXRecoiled, stepsYRecoiled; //the no. of steps recoiled horizontally and verticall
    [Space(5)]


    [Header("Health Settings")]
    public Health playerHealth;
    //public int health;
    //public int maxHealth;
    public int maxTotalHealth = 10;
    public int heartPieces = 4; // how many pieces to get a heart
    [SerializeField] GameObject bloodSpurt;
    [SerializeField] float hitFlashSpeed;
    public delegate void OnHealthChangedDelegate();
    [HideInInspector] public OnHealthChangedDelegate onHealthChangedCallback;
    float healTimer;
    [SerializeField] float timeToHeal;
    [Space(5)]


    [Header("Mana Settings")]
    [SerializeField] UnityEngine.UI.Image manaStorage;
    [SerializeField] float mana;
    [SerializeField] float manaDrainSpeed;
    [SerializeField] public float manaGain;
    [Space(5)]


    [Header("Spell Settings")]
    //spell stats
    [SerializeField] float manaSpellCost = 0.3f;
    [SerializeField] float timeBetweenCast = 0.5f;
    [SerializeField] float spellDamage; //upspellexplosion and downspellfireball
    [SerializeField] float downSpellForce; // desolate dive only
    //spell cast objects
    [SerializeField] GameObject sideSpellFireball;
    [SerializeField] GameObject upSpellExplosion;
    [SerializeField] GameObject downSpellFireball;
    float timeSinceCast;
    float castOrHealTimer;
    [Space(5)]

    [Header("Audio")]
    [SerializeField] AudioClip landingSound;
    [SerializeField] AudioClip jumpSound;
    [SerializeField] public AudioClip dashAndAttackSound;
    [SerializeField] AudioClip spellcastSound;
    [SerializeField] AudioClip hurtSound;
    private bool landingSoundPlayed;
    [Space(5)]

    [HideInInspector] public PlayerStateList pState;
    private Animator anim;
    public Rigidbody2D rb;
    private SpriteRenderer sr;
    private AudioSource audioSource;

    //Input Variables
    private float xAxis, yAxis;
    private bool attack = false;
    private bool canFlash = true;
    bool openInventory;

    public static PlayerController Instance;
    
    private GameObject attackAreaSide = default;
    private GameObject attackAreaUp = default;
    private GameObject attackAreaDown = default;
    
    private PlayerAttack playerAttack;

    [Header("Unlocks")]
    public bool unlockedWallJump;
    public bool unlockedDash;
    public bool unlockedVarJump;
    public bool unlockedSideCast;
    public bool unlockedUpCast;
    public bool unlockedDownCast;

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
        DontDestroyOnLoad(gameObject);
        
    }


    // Start is called before the first frame update
    void Start()
    {
        pState = GetComponent<PlayerStateList>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        // Get the Health component
        playerHealth = GetComponent<Health>();
        
        // attack areas
        //attackAreaSide = transform.GetChild(0).gameObject;
        //attackAreaUp = transform.GetChild(1).gameObject;
        //attackAreaDown = transform.GetChild(2).gameObject;
        
        // attack areas
        //attackAreaSide = transform.Find("SideAttackTransform").gameObject;
        //attackAreaUp = transform.Find("UpAttackTransform").gameObject;
        //attackAreaDown = transform.Find("DownAttackTransform").gameObject;
        
        playerAttack = GetComponent<PlayerAttack>();

        //SaveData.Instance.LoadPlayerData();
        gravity = rb.gravityScale;

        Mana = mana;
        manaStorage.fillAmount = Mana;
        Health = playerHealth.MaxHealth;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(SideAttackTransform.position, SideAttackArea);
        Gizmos.DrawWireCube(UpAttackTransform.position, UpAttackArea);
        Gizmos.DrawWireCube(DownAttackTransform.position, DownAttackArea);
    }

    
    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameIsPaused) return;

        if (pState.cutscene) return;
        if(pState.alive)
        {
            GetInputs();
            ToggleInventory();
        }
        
        UpdateJumpVariables();
        RestoreTimeScale();

        if (pState.dashing) return;
        if (pState.alive)
        {
            if(!isWallJumping)
            {
                Flip();
                Move();
                Jump();
            }

            if (unlockedWallJump)
            {
                WallSlide();
                WallJump();
            }
            
            if(unlockedDash)
            {
                StartDash();
            }
            
            //Attack();
            if(attack) playerAttack.Attack();
            Heal();
            CastSpell();
        }
        FlashWhileInvincible();

    }
    private void OnTriggerEnter2D(Collider2D _other) //for up and down cast spell
    {
        if (_other.GetComponent<Enemy>() != null && pState.casting)
        {
            _other.GetComponent<Enemy>().EnemyHit(spellDamage, (_other.transform.position - transform.position).normalized, -recoilYSpeed);
        }
    }

    private void FixedUpdate()
    {
        if (pState.cutscene) return;

        if (pState.dashing) return;
        Recoil();
    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
        attack = Input.GetButtonDown("Attack");
        //openInventory = Input.GetButton("Inventory");
        if (Input.GetButtonDown("Inventory")) openInventory = !openInventory;

        if (Input.GetButton("Cast/Heal"))
        {
            castOrHealTimer += Time.deltaTime;
        }
        else
        {
            castOrHealTimer = 0;
        }
    }

    //public bool IsAttacking { get { return attack; } }
    public bool IsAttacking { get; set; }
    
    void ToggleInventory()
    {
        if (openInventory)
        {
            UIManager.Instance.inventory.SetActive(true);
        }
        else
        {
            UIManager.Instance.inventory.SetActive(false) ;
        }
    }
    // movement methods
    void Flip()
    {
        if (xAxis < 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
            pState.lookingRight = false;
        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
            pState.lookingRight = true;
        }
    }

    private void Move()
    {
        rb.velocity = new Vector2(walkSpeed * xAxis, rb.velocity.y);
        anim.SetBool("Walking", rb.velocity.x != 0 && Grounded());
    }

    void StartDash()
    {
        if (Input.GetButtonDown("Dash") && canDash && !dashed)
        {
            StartCoroutine(Dash());
            dashed = true;
        }

        if (Grounded())
        {
            dashed = false;
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        pState.dashing = true;
        anim.SetTrigger("Dashing");
        audioSource.PlayOneShot(dashAndAttackSound);
        rb.gravityScale = 0;
        int _dir = pState.lookingRight ? 1 : -1;
        rb.velocity = new Vector2(_dir * dashSpeed, 0);
        if (Grounded()) Instantiate(dashEffect, transform);
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState.dashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    public IEnumerator WalkIntoNewScene(Vector2 _exitDir, float _delay)
    {
        //If exit direction is upwards
        if (_exitDir.y > 0)
        {
            rb.velocity = jumpForce * _exitDir;
        }

        //If exit direction requires horizontal movement
        if (_exitDir.x != 0)
        {
            xAxis = _exitDir.x > 0 ? 1 : -1;

            Move();
        }

        Flip();
        yield return new WaitForSeconds(_delay);
        pState.cutscene = false;
    }
    
    // attacking methods
    void Attack()
    {
        timeSinceAttack += Time.deltaTime;
        if (attack && timeSinceAttack >= timeBetweenAttack)
        {
            int _recoilLeftOrRight = pState.lookingRight ? 1 : -1;
            timeSinceAttack = 0;
            anim.SetTrigger("Attacking");
            audioSource.PlayOneShot(dashAndAttackSound);

            // Set attacking to true at the start of an attack
            
            
            if (yAxis == 0 || yAxis < 0 && Grounded())
            {
                //Hit(SideAttackTransform, SideAttackArea, ref pState.recoilingX, Vector2.right * _recoilLeftOrRight, recoilXSpeed, "side");
                attackAreaSide.SetActive(true);
                Instantiate(slashEffect, SideAttackTransform);
            }
            else if (yAxis > 0)
            {
                //Hit(UpAttackTransform, UpAttackArea, ref pState.recoilingY, Vector2.up, recoilYSpeed, "up");
                attackAreaUp.SetActive(true);
                SlashEffectAtAngle(slashEffect, 80, UpAttackTransform);
            }
            else if (yAxis < 0 && !Grounded())
            {
                //Hit(DownAttackTransform, DownAttackArea, ref pState.recoilingY, Vector2.down, recoilYSpeed, "down");
                attackAreaDown.SetActive(true);
                SlashEffectAtAngle(slashEffect, -90, DownAttackTransform);
            }
            attackAreaSide.SetActive(false);
            attackAreaUp.SetActive(false);
            attackAreaDown.SetActive(false);
            
        }


    }
    /*void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilBool, Vector2 _recoilDir, float _recoilStrength)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);

        if (objectsToHit.Length > 0)
        {
            _recoilBool = true;
        }
        for (int i = 0; i < objectsToHit.Length; i++)
        {
            if (objectsToHit[i].GetComponent<Enemy>() != null)
            {
                objectsToHit[i].GetComponent<Enemy>().EnemyHit(damage, _recoilDir, _recoilStrength);

                if (objectsToHit[i].CompareTag("Enemy"))
                {
                    Mana += manaGain;
                }
            }
        }
    }*/
    
    /*void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilBool, Vector2 _recoilDir, float _recoilStrength)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);

        if (objectsToHit.Length > 0)
        {
            _recoilBool = true;
        }
        foreach (var obj in objectsToHit)
        {
            if (obj.CompareTag("Enemy")) // Check if the collided object is an enemy
            {
                obj.GetComponent<Enemy>().EnemyHit(damage, _recoilDir, _recoilStrength);
                Mana += manaGain;
            }
        }
    }*/
    /*void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilBool, Vector2 _recoilDir, float _recoilStrength)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);

        if (objectsToHit.Length > 0)
        {
            _recoilBool = true;
        }

        foreach (var obj in objectsToHit)
        {
            Debug.Log("Hit object: " + obj.name); // Log the name of the collided object

            if (obj.CompareTag("Enemy")) // Check if the collided object is an enemy
            {
                obj.GetComponent<Enemy>().EnemyHit(damage, _recoilDir, _recoilStrength);
                Mana += manaGain;
            }
        }

        // Apply recoil only if there are objects hit and the player is not grounded
        if (objectsToHit.Length > 0 && !Grounded())
        {
            rb.velocity += _recoilDir * _recoilStrength * Time.deltaTime;
        }
    }*/
    
    void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilBool, Vector2 _recoilDir, float _recoilStrength, string attackDirection)
    {
        // Determine which transform to use based on attackDirection
        switch (attackDirection)
        {
            case "side":
                _attackTransform = SideAttackTransform;
                break;
            case "up":
                _attackTransform = UpAttackTransform;
                break;
            case "down":
                _attackTransform = DownAttackTransform;
                break;
        }

        // Create a new BoxCollider2D and set it as a trigger
        //BoxCollider2D attackCollider = _attackTransform.gameObject.AddComponent<BoxCollider2D>();
        //attackCollider.isTrigger = true;
        //attackCollider.size = _attackArea;
        //attackCollider.tag = "PlayerAttack";
        //attackCollider.gameObject.layer = LayerMask.NameToLayer("PlayerAttack");

        // Get all objects within the attack area
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);

        if (objectsToHit.Length > 0)
        {
            _recoilBool = true;
        }

        foreach (var obj in objectsToHit)
        {
            Debug.Log("Hit object: " + obj.name); // Log the name of the collided object

            // Check if the collided object is an enemy and not the player
            if (obj.CompareTag("Enemy") && obj.gameObject != this.gameObject)
            {
                obj.GetComponent<Enemy>().EnemyHit(damage, _recoilDir, _recoilStrength);
                Mana += manaGain;
            }
        }

        // Apply recoil only if there are objects hit and the player is not grounded
        if (objectsToHit.Length > 0 && !Grounded())
        {
            rb.velocity += _recoilDir * _recoilStrength * Time.deltaTime;
        }

        // Destroy the collider after the attack
        //Destroy(attackCollider);
    }

    
    
    


    void SlashEffectAtAngle(GameObject _slashEffect, int _effectAngle, Transform _attackTransform)
    {
        _slashEffect = Instantiate(_slashEffect, _attackTransform);
        _slashEffect.transform.eulerAngles = new Vector3(0, 0, _effectAngle);
        _slashEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
    }
    // life and death methods
    void Recoil()
    {
        if (pState.recoilingX)
        {
            if (pState.lookingRight)
            {
                rb.velocity = new Vector2(-recoilXSpeed, 0);
            }
            else
            {
                rb.velocity = new Vector2(recoilXSpeed, 0);
            }
        }

        if (pState.recoilingY)
        {
            rb.gravityScale = 0;
            if (yAxis < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, recoilYSpeed);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, -recoilYSpeed);
            }
            airJumpCounter = 0;
        }
        else
        {
            rb.gravityScale = gravity;
        }

        //stop recoil
        if (pState.recoilingX && stepsXRecoiled < recoilXSteps)
        {
            stepsXRecoiled++;
        }
        else
        {
            StopRecoilX();
        }
        if (pState.recoilingY && stepsYRecoiled < recoilYSteps)
        {
            stepsYRecoiled++;
        }
        else
        {
            StopRecoilY();
        }

        if (Grounded())
        {
            StopRecoilY();
        }
    }
    void StopRecoilX()
    {
        stepsXRecoiled = 0;
        pState.recoilingX = false;
    }
    void StopRecoilY()
    {
        stepsYRecoiled = 0;
        pState.recoilingY = false;
    }
    public void TakeDamage(float _damage)
    {
        if (pState.attacking) return;
        audioSource.PlayOneShot(hurtSound);

        if (pState.alive)
        {
            //Health -= Mathf.RoundToInt(_damage);
            //if(Health <= 0)
            playerHealth.HealthValue -= Mathf.RoundToInt(_damage);
            if(playerHealth.HealthValue <= 0)
            {
                //Health = 0;
                playerHealth.HealthValue = 0;
                StartCoroutine(Death());
            }
            else
            {
                StartCoroutine(StopTakingDamage());
            }
        }
        
        
    }
    IEnumerator StopTakingDamage()
    {
        pState.invincible = true;
        GameObject _bloodSpurtParticles = Instantiate(bloodSpurt, transform.position, Quaternion.identity);
        Destroy(_bloodSpurtParticles, 1.5f);
        anim.SetTrigger("TakeDamage");
        yield return new WaitForSeconds(1f);
        pState.invincible = false;
    }

    IEnumerator Flash()
    {
        sr.enabled = !sr.enabled;
        canFlash = false;
        yield return new WaitForSeconds(0.1f);
        canFlash = true;
    }
    void FlashWhileInvincible()
    {
        if (pState.invincible)
        {
            if (Time.timeScale > 0.2 && canFlash)
            {
                StartCoroutine(Flash());
            }
        }
        else
        {
            sr.enabled = true;
        }
    }

    void RestoreTimeScale()
    {
        if (restoreTime)
        {
            if (Time.timeScale < 1)
            {
                Time.timeScale += Time.deltaTime * restoreTimeSpeed;
            }
            else
            {
                Time.timeScale = 1;
                restoreTime = false;
            }
        }
    }

    public void HitStopTime(float _newTimeScale, int _restoreSpeed, float _delay)
    {
        restoreTimeSpeed = _restoreSpeed;
        if (_delay > 0)
        {
            StopCoroutine(StartTimeAgain(_delay));
            StartCoroutine(StartTimeAgain(_delay));
        }
        else
        {
            restoreTime = true;
        }
        Time.timeScale = _newTimeScale;
    }

    IEnumerator StartTimeAgain(float _delay)
    {
        restoreTime = true;
        yield return new WaitForSeconds(_delay);
    }

    public IEnumerator Death()
    {
        pState.alive = false;
        Time.timeScale = 1f;
        GameObject _bloodSpurtParticles = Instantiate(bloodSpurt, transform.position, Quaternion.identity);
        Destroy(_bloodSpurtParticles, 1.5f);
        anim.SetTrigger("Death");
        yield return new WaitForSecondsRealtime(0.9f);

        StartCoroutine(UIManager.Instance.ActivateDeathScreen());

        Respawned();
    }

    public void Respawned()
    {
        if(!pState.alive)
        {
            pState.alive = true;
            //Health = maxHealth;
            playerHealth.HealthValue = playerHealth.MaxHealth;

            anim.Play("Player_Idle");
            mana = 0;
        }
    }

    public int Health
    {
        get { return playerHealth.HealthValue; }
        set { playerHealth.HealthValue = Mathf.Clamp(value, 0, playerHealth.MaxHealth); }
        /*
        //get { return health; }
        get { return playerHealth.HealthValue; }
        set
        {
            //if (health != value)
            if (playerHealth.HealthValue != value)
            {
                //health = Mathf.Clamp(value, 0, maxHealth);
                playerHealth.HealthValue = Mathf.Clamp(value, 0, playerHealth.MaxHealth);
                
                if (onHealthChangedCallback != null)
                {
                    onHealthChangedCallback.Invoke();
                }
            }
        }*/
    }

    // magic methods
    void Heal()
    {
        if (Input.GetButton("Cast/Heal") && castOrHealTimer > 0.05f && Health < playerHealth.MaxHealth && Mana > 0 && !pState.jumping && !pState.dashing)
        {
            pState.healing = true;
            anim.SetBool("Healing", true);

            //healing
            healTimer += Time.deltaTime;
            if (healTimer >= timeToHeal)
            {
                Health++;
                healTimer = 0;
            }

            //drain mana
            Mana -= Time.deltaTime * manaDrainSpeed;
        }
        else
        {
            pState.healing = false;
            anim.SetBool("Healing", false);
            healTimer = 0;
        }
    }

    public float Mana
    {
        get { return mana; }
        set
        {
            //if mana stats change
            if (mana != value)
            {
                mana = Mathf.Clamp(value, 0, 1);
                manaStorage.fillAmount = Mana;
            }
        }
    }

    void CastSpell()
    {
        if (Input.GetButtonUp("Cast/Heal") && castOrHealTimer <= 0.05f && timeSinceCast >= timeBetweenCast && Mana >= manaSpellCost)
        {
            pState.casting = true;
            timeSinceCast = 0;
            StartCoroutine(CastCoroutine());
            
        }
        else
        {
            timeSinceCast += Time.deltaTime;
        }

        if (Grounded())
        {
            //disable downspell if on the ground
            downSpellFireball.SetActive(false);
        }
        //if down spell is active, force player down until grounded
        if (downSpellFireball.activeInHierarchy)
        {
            rb.velocity += downSpellForce * Vector2.down;
        }
    }

    IEnumerator CastCoroutine()
    {
        audioSource.PlayOneShot(spellcastSound);

        anim.SetBool("Casting", true);
        yield return new WaitForSeconds(0.15f); // check on Animator if it matches

        //side cast
        if ((yAxis == 0 || (yAxis < 0 && Grounded())) && unlockedSideCast)
        {
            anim.SetBool("Casting", true);
            yield return new WaitForSeconds(0.15f); // check on Animator if it matches
            GameObject _fireBall = Instantiate(sideSpellFireball, SideAttackTransform.position, Quaternion.identity);

                //flip fireball
            if (pState.lookingRight)
            {
                _fireBall.transform.eulerAngles = Vector3.zero; // if facing right, fireball continues as per normal
                rb.velocity = new Vector2(-recoilXSpeed, rb.velocity.y); // Apply negative recoil
            }
            else
            {
                _fireBall.transform.eulerAngles = new Vector2(_fireBall.transform.eulerAngles.x, 180);
                //if not facing right, rotate the fireball 180 deg
                rb.velocity = new Vector2(recoilXSpeed, rb.velocity.y); // Apply positive recoil
            }
            pState.recoilingX = true;

            Mana -= manaSpellCost;
            yield return new WaitForSeconds(0.35f); // check Animator
        }

        //up cast
        else if (yAxis > 0 && unlockedUpCast)
        {
            anim.SetBool("Casting", true);
            yield return new WaitForSeconds(0.15f); // check on Animator if it matches
            Instantiate(upSpellExplosion, transform);
            rb.velocity = Vector2.zero;

            Mana -= manaSpellCost;
            yield return new WaitForSeconds(0.35f); // check Animator
        }

        //down cast
        else if ((yAxis < 0 && !Grounded()) && unlockedDownCast)
        {
            
            anim.SetBool("Casting", true);
            yield return new WaitForSeconds(0.15f); // check on Animator if it matches
            downSpellFireball.SetActive(true);

            Mana -= manaSpellCost;
            yield return new WaitForSeconds(0.35f); // check Animator
        }

        
        anim.SetBool("Casting", false);
        pState.casting = false;
    }
    // jumping methods
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

    void Jump()
    {
        if (!pState.jumping && jumpBufferCounter > 0 && coyoteTimeCounter > 0)
        {
            audioSource.PlayOneShot(jumpSound);

            rb.velocity = new Vector3(rb.velocity.x, jumpForce);

            pState.jumping = true;
        }
        
        if (!Grounded() && airJumpCounter < maxAirJumps && Input.GetButtonDown("Jump") && unlockedVarJump && !Walled())
        {
            audioSource.PlayOneShot(jumpSound);

            pState.jumping = true;

            airJumpCounter++;
            Debug.Log("Air jump counter: " + airJumpCounter);

            rb.velocity = new Vector3(rb.velocity.x, jumpForce);
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 3)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);

            pState.jumping = false;
        }

        anim.SetBool("Jumping", !Grounded());
    }

    void UpdateJumpVariables()
    {
        if (Grounded())
        {
            if (!landingSoundPlayed)
            {
                audioSource.PlayOneShot(landingSound);
                landingSoundPlayed = true;
            }
            pState.jumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
            landingSoundPlayed = false;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferFrames;
        }
        else
        {
            jumpBufferCounter--;
        }
    }
    // wall jumping methods
    private bool Walled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    /*void WallSlide()
    {
        if(Walled() && !Grounded() && xAxis != 0)
        {
            isWallSliding = true;

            // set player speed to wallsliding speed
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }*/
    void WallSlide()
    {
        if (Walled() && !Grounded() && xAxis != 0)
        {
            isWallSliding = true;

            // Set player speed to wall sliding speed
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));

            // Reset air jump counter
            airJumpCounter = 0;
            Debug.Log("Air jump counter reset");
            
            // Update player orientation based on the wall side
            if (xAxis < 0 && pState.lookingRight)
            {
                Flip();
            }
            else if (xAxis > 0 && !pState.lookingRight)
            {
                Flip();
            }
        }
        else
        {
            isWallSliding = false;
        }
    }

    /*void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = !pState.lookingRight ? 1 : -1;

            CancelInvoke(nameof(StopWallJumping));
        }

        if (Input.GetButtonDown("Jump") && isWallSliding)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);

            dashed = false;
            airJumpCounter = 0;

            if ((pState.lookingRight && transform.eulerAngles.y == 0) || (!pState.lookingRight && transform.eulerAngles.y != 0))
            {
                pState.lookingRight = !pState.lookingRight;
                int _yRotation = pState.lookingRight ? 0 : 180;

                transform.eulerAngles = new Vector2(transform.eulerAngles.x, _yRotation);
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }*/
    void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = !pState.lookingRight ? 1 : -1;

            // Update player orientation based on the wall side
            if ((wallJumpingDirection > 0 && !pState.lookingRight) || (wallJumpingDirection < 0 && pState.lookingRight))
            {
                Flip();
            }

            CancelInvoke(nameof(StopWallJumping));
        }

        if (Input.GetButtonDown("Jump") && isWallSliding)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);

            dashed = false;
            airJumpCounter = 0;

            // Update player orientation based on the wall side
            if ((wallJumpingDirection > 0 && !pState.lookingRight) || (wallJumpingDirection < 0 && pState.lookingRight))
            {
                Flip();
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    void StopWallJumping()
    {
        isWallJumping = false;
    }
}