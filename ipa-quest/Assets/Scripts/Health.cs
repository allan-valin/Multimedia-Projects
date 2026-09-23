using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] public int health = 100;

    public int maxHealth = 100;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            // Damage(10);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            // Heal(10);
        }
    }
    
    public int HealthValue
    {
        get { return health; }
        set { health = Mathf.Clamp(value, 0, maxHealth); }
    }
    public int MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = value; }
    }
    public void SetHealth(int maxHealth, int health)
    {
        this.maxHealth = maxHealth;
        this.health = health;
    }

    public void Damage(int amount)
    {
        if(amount < 0)
        {
            throw new System.ArgumentOutOfRangeException("Cannot have negative Damage");
        }
        //Debug.Log("Health before damage: " + this.health); // Log the health before damage

        this.health -= amount;
        Debug.Log("Health after damage: " + this.health); // Log the health after damage

        if(health <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (amount < 0)
        {
            throw new System.ArgumentOutOfRangeException("Cannot have negative healing");
        }

        bool wouldBeOverMaxHealth = health + amount > maxHealth;

        if (wouldBeOverMaxHealth)
        {
            this.health = maxHealth;
        }
        else
        {
            this.health += amount;
        }
    }

    public void Die()
    {
        // Check if the Health component is attached to an Enemy object
        Enemy enemy = GetComponent<Enemy>();
        HealthDrop drop = GetComponent<HealthDrop>();
        if (enemy != null)
        {
            // If it is, call the Death method on the Enemy instance
            enemy.rb.gravityScale = 12.0f;
            enemy.Death(0.5f);
            drop.DropHeart();
        }
        else
        {
            // Existing code for PlayerController
            PlayerController.Instance.Death();
        }
    }
    
    
}