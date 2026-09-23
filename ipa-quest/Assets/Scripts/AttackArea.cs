using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    [SerializeField] int damage = 3;
    private Collider2D attackCollider;

    private void Start()
    {
        // Get the Collider2D component
        attackCollider = GetComponent<Collider2D>();
        
    }

    private void Update()
    {
        // Enable or disable the collider based on the player's attacking state
        //attackCollider.enabled = PlayerController.Instance.IsAttacking;
        //attackArea.GetComponent<BoxCollider2D>().enabled = attacking;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        PlayerAttack playerAttack = GameObject.FindWithTag("Player").GetComponent<PlayerAttack>();
        if(collider.GetComponent<Health>() != null && playerAttack.IsAttacking)
        {
            Health health = collider.GetComponent<Health>();
            health.Damage(damage);

            //Debug.Log("Hit object: " + collider); // Log the name of the collided object
            PlayerController.Instance.Mana += PlayerController.Instance.manaGain;

            // Check if the collided object is an enemy
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Play the hurtSound and spawn the orangeBlood particle effect
                AudioSource audioSource = GetComponentInParent<AudioSource>();
                audioSource.PlayOneShot(enemy.hurtSound);
                GameObject _orangeBlood = Instantiate(enemy.orangeBlood, transform.position, Quaternion.identity);
                Destroy(_orangeBlood, 5.5f);
            }
        }
    }
}