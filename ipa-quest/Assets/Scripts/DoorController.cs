using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public int symbolsRequired = 10; // Adjust the number of symbols required to unlock the door
    private bool isUnlocked = false;
    private bool bossBattleMusicStarted = false;

    private ItemCollector itemCollector; // Reference to the ItemCollector script
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component
    private BoxCollider2D boxCollider; // Reference to the BoxCollider2D component

    private void Start()
    {
        // Get the SpriteRenderer and BoxCollider2D components
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        // Get the ItemCollector component from the player object
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            itemCollector = player.GetComponent<ItemCollector>();
        }
    }

    private void Update()
    {
        // Check if the required number of symbols have been collected
        if (!isUnlocked && itemCollector != null && itemCollector.GetSymbolsCollected() >= symbolsRequired)
        {
            isUnlocked = true;
            spriteRenderer.enabled = false;
            boxCollider.isTrigger = true;
            Debug.Log("Door unlocked!");
        }
        
        // Check if the player has collected 85 symbols and the boss battle music hasn't started yet
        if (!bossBattleMusicStarted && itemCollector != null && itemCollector.GetSymbolsCollected() >= 85)
        {
            bossBattleMusicStarted = true;
            AudioManager.instance.PlayBossBattleMusic();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If the door is unlocked and the colliding object is the player, trigger the scene transition
        if (isUnlocked && collision.gameObject.CompareTag("Player"))
        {
            // Assuming the SceneTransition script is attached to the same object as the DoorController script
            SceneTransition sceneTransition = GetComponent<SceneTransition>();
            if (sceneTransition != null)
            {
                sceneTransition.HandleSceneTransition(collision.transform);
            }
        }
    }
}