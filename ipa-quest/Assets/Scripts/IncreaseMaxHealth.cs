using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseMaxHealth : MonoBehaviour
{
    [SerializeField] GameObject particles;
    [SerializeField] GameObject canvasUI;

    [SerializeField] HeartPieces heartPieces;
    
    bool used;
    

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerController.Instance.playerHealth.MaxHealth >= PlayerController.Instance.maxTotalHealth)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !used)
        {
            used = true;
            StartCoroutine(ShowUI());
            
        }
    }

    IEnumerator ShowUI()
    {
        GameObject _particles = Instantiate(particles, transform.position, Quaternion.identity);
        Destroy(_particles, 0.5f);
        yield return new WaitForSecondsRealtime(0.1f);

        //canvasUI.SetActive(true);
        //heartPieces.initialFillAmount = PlayerController.Instance.heartPieces * 0.25f;
        PlayerController.Instance.heartPieces++;
        //heartPieces.targetFillAmount = PlayerController.Instance.heartPieces * 0.25f;

        //StartCoroutine(heartPieces.LerpFill());
        if(PlayerController.Instance.heartPieces == 4)
        {
            PlayerController.Instance.playerHealth.MaxHealth++;
            PlayerController.Instance.onHealthChangedCallback();
            PlayerController.Instance.heartPieces = 0;
        }



        yield return new WaitForSecondsRealtime(0.1f);
        
        SaveData.Instance.SavePlayerData();
        //canvasUI.SetActive(false);
        Destroy(gameObject);
    }
}
