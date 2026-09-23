using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private GameObject attackArea = default;
    public bool attacking = false;
    private float timeToAttack = 0.7f;
    private float timer = 0f;

    private Animator anim;
    private AudioSource audioSource;

    // Reference to PlayerController
    private PlayerController playerController;

    // Time since last attack
    private float timeSinceAttack;
    // Minimum time between attacks
    [SerializeField] private float timeBetweenAttack = 0.7f;
    

    // Start is called before the first frame update
    void Start()
    {
        attackArea = transform.GetChild(0).gameObject;
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if(attacking)
        {
            timer += Time.deltaTime;
            //Debug.Log("Timer: " + timer);


            if(timer >= timeToAttack)
            {
                timer = 0;
                attacking = false;
                attackArea.SetActive(attacking);
                //attackArea.GetComponent<BoxCollider2D>().enabled = attacking;
                //Debug.Log("Attack finished");


            }
            //Debug.Log("Attacking: " + attacking);
        }

        // Update time since last attack
        timeSinceAttack += Time.deltaTime;
    }
    
    public void Attack()
    {
        // Only attack if enough time has passed since the last attack
        if (timeSinceAttack >= timeBetweenAttack)
        {
            //Debug.Log("Attacking");
            attacking = true;
            //attackArea.SetActive(attacking);
            //attackArea.GetComponent<BoxCollider2D>().enabled = attacking;
            attackArea.SetActive(attacking);

            // Set the animation trigger and play the sound
            anim.SetTrigger("Attacking");
            audioSource.PlayOneShot(playerController.dashAndAttackSound);

            // Reset timer and attacking state
            timeSinceAttack = 0f;
            //Debug.Log("Reset timeSinceAttack to " + timeSinceAttack);
            //attacking = false;
        }
    }
    
    public bool IsAttacking
    {
        get { return attacking; }
    }
    
    
}