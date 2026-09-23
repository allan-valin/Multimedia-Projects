using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Savepoint : MonoBehaviour
{
    public bool interacted;
    private Animator anim;
    private bool interactionCheck;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.Play("Flag_Down");
    }

    // Update is called once per frame
    void Update()
    {
        if(interactionCheck)
        {
            interacted = true;
            //anim.Play("Flag");
            anim.SetTrigger("Active");

            SaveData.Instance.savepointSceneName = SceneManager.GetActiveScene().name;
            SaveData.Instance.savepointPos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
            SaveData.Instance.StoreSavepoint();
            SaveData.Instance.SavePlayerData();
            Debug.Log("saved");
        }
        else
        {
            interacted = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && Input.GetButtonDown("Interact"))
        {
            interactionCheck = true;
        }
        else
        {
            interactionCheck = false;
        }
    }

    /*private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") )//&& Input.GetButtonDown("Interact"))
        {
            interacted = false;
        }
    }*/
}
