using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealHP : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            PlayerController.Instance.playerHealth.HealthValue++;
            Destroy(gameObject);
        }
    }
}
