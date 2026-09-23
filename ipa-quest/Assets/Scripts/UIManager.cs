using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public SceneFader sceneFader;

    [SerializeField] GameObject deathScreen;
    public GameObject inventory;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);

        
    }

    private void Start()
    {
        sceneFader = GetComponentInChildren<SceneFader>();
    }

    public IEnumerator ActivateDeathScreen()
    {
        yield return new WaitForSecondsRealtime(0.8f);
        StartCoroutine(sceneFader.Fade(SceneFader.FadeDirection.In));
        yield return new WaitForSecondsRealtime(0.8f);
        deathScreen.SetActive(true);

    }

    public IEnumerator DeactivateDeathScreen()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        deathScreen.SetActive(false);
        StartCoroutine(sceneFader.Fade(SceneFader.FadeDirection.Out));
    }

}