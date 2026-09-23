using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string transitionedFromScene;

    public Vector2 platformingRespawnPoint;
    public Vector2 respawnPoint;
    [SerializeField] Savepoint savepoint;

    public Vector3 playerStartPoint;
    
    [SerializeField] private FadeUI pauseMenu;
    [SerializeField] private float fadeTime;
    public bool gameIsPaused;

    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        SaveData.Instance.Initialize();

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        SaveScene();

        DontDestroyOnLoad(gameObject);
        savepoint = FindObjectOfType<Savepoint>();
    }

    public void RespawnPlayer()
    {
        SaveData.Instance.LoadSavepoint();

        if(SaveData.Instance.savepointSceneName != null)
        {
            SceneManager.LoadScene(SaveData.Instance.savepointSceneName);
        }
        
        if(SaveData.Instance.savepointPos != null)
        {
            respawnPoint = SaveData.Instance.savepointPos;
        }
        else
        {
            respawnPoint = platformingRespawnPoint;
        }
        /*
        if (savepoint != null)
        {
            if (savepoint.interacted)
            {
                respawnPoint = savepoint.transform.position;
            }
            else
            {
                respawnPoint = platformingRespawnPoint;
            }
        }
        else
        {
            respawnPoint = platformingRespawnPoint;
        }
        */

        PlayerController.Instance.transform.position = respawnPoint;
        StartCoroutine(UIManager.Instance.DeactivateDeathScreen());
        PlayerController.Instance.Respawned();
    }

    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.P)) // for testing 
        {
            SaveData.Instance.SavePlayerData();
        }

        if(Input.GetKeyUp(KeyCode.Escape) && !gameIsPaused) 
        {
            pauseMenu.FadeUIIn(fadeTime);
            Time.timeScale = 0;
            GameManager.Instance.gameIsPaused = true;
        }
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1;
        GameManager.Instance.gameIsPaused = false;
    }
    
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }

    public void SaveScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SaveData.Instance.sceneNames.Add(currentSceneName);
    }

    private void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<StandaloneInputModule>();
        }
    }
}