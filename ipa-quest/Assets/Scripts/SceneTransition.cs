using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private SceneField transitionTo;
//    [SerializeField] private Transform startPoint;
    [SerializeField] private List<Transform> startPoints; // List of start points

    [SerializeField] private Vector2 exitDirection;
    [SerializeField] private float exitTime;

    private void Start()
    {
        InitializeGameObjects();
        HandleTransitionFromPreviousScene();
        StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.Out));
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        Debug.Log("Trigger entered by: " + _other.gameObject.name); // Debugging line

        if (_other.CompareTag("Player"))
        {
            Debug.Log("Scene transition initiated"); // Debugging line

            InitializeGameObjects();
            HandleSceneTransition(_other.transform);
        }
    }

    private void InitializeGameObjects()
    {
        if (GameManager.Instance == null)
        {
            // Initialize GameManager.Instance here
            GameManager.Instance.transitionedFromScene = SceneManager.GetActiveScene().name;
        }

        if (PlayerController.Instance == null || PlayerController.Instance.pState == null)
        {
            // Initialize PlayerController.Instance and PlayerController.Instance.pState here
            PlayerController.Instance.pState.cutscene = true;
        }
    }

    private void HandleTransitionFromPreviousScene()
    {
        if (GameManager.Instance.transitionedFromScene == transitionTo)
        {
            // Find the closest start point to the player
            Transform closestStartPoint = startPoints.OrderBy(t => Vector2.Distance(t.position, PlayerController.Instance.transform.position)).First();

            PlayerController.Instance.transform.position = closestStartPoint.position;
            StartCoroutine(PlayerController.Instance.WalkIntoNewScene(exitDirection, exitTime));
        }
    }

    public void HandleSceneTransition(Transform playerTransform)
    {
        if (startPoints.Count == 0)
        {
            Debug.LogError("No start points have been assigned in the SceneTransition component.");
            return;
        }

        // Find the closest start point to the player
        Transform closestStartPoint = startPoints.OrderBy(t => Vector2.Distance(t.position, playerTransform.position)).First();

        GameManager.Instance.transitionedFromScene = SceneManager.GetActiveScene().name;
        GameManager.Instance.playerStartPoint = closestStartPoint.position;

        PlayerController.Instance.pState.cutscene = true;
        StartCoroutine(LoadSceneAsync(transitionTo));
    }

    private IEnumerator LoadSceneAsync(SceneField scene)
    {
        // The Application.backgroundLoadingPriority could be set to High so the loading can happen as fast as possible
        Application.backgroundLoadingPriority = ThreadPriority.High;

        // Start an asynchronous operation to load the scene that was passed to the LoadNewScene coroutine
        AsyncOperation async = SceneManager.LoadSceneAsync(scene.SceneName, LoadSceneMode.Single);

        // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done
        while (!async.isDone)
        {
            yield return null;
        }
    }
}

/*
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public Transform entrancePoint; // Entrance point of the next scene
    public SceneAsset sceneToLoad; // Scene to load

    private void Awake()
    {
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the sceneLoaded event when the script is destroyed
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Save the player's position
            PlayerPrefs.SetFloat("PlayerX", other.transform.position.x);
            PlayerPrefs.SetFloat("PlayerY", other.transform.position.y);

            LoadScene();
        }
    }

    private void LoadScene()
    {
        if (sceneToLoad != null)
        {
            SceneManager.LoadScene(sceneToLoad.name);
        }
        else
        {
            Debug.LogError("Scene to load is not assigned!");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Call the SpawnPlayerAtEntrancePoint method when a new scene is loaded
        SpawnPlayerAtEntrancePoint();
    }

    private void SpawnPlayerAtEntrancePoint()
    {
        // Check if there's an entrance point and spawn player accordingly
        if (entrancePoint != null)
        {
            Vector2 playerPosition = new Vector2(PlayerPrefs.GetFloat("PlayerX"), PlayerPrefs.GetFloat("PlayerY"));
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = playerPosition;
            }
        }
    }
}*/