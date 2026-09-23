using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // needed to reference text
using TMPro;

public class ItemCollector : MonoBehaviour
{
    private int symbols = 0;

    [SerializeField] private TextMeshProUGUI symbolsText;

    AudioManager audioManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("collided");
        if (collision.gameObject.CompareTag("Symbols"))
        {
            Debug.Log("collided inside");
            Symbol symbol = collision.gameObject.GetComponent<Symbol>();
            if (symbol != null)
            {
                audioManager.PlaySFX(symbol.symbolNumber);
                Destroy(collision.gameObject);
                symbols++;
                symbolsText.text = "Symbols: " + symbols.ToString(); // update item counter
            }
        }
    }
    public int GetSymbolsCollected()
    {
        return symbols;
    }
    
    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
}