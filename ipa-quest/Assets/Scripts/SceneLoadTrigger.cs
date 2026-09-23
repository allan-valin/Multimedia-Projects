using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadTrigger : MonoBehaviour
{
    private GameObject _player;

    [SerializeField] private SceneField[] _scenesToLoad;
    [SerializeField] private SceneField[] _scenesToUnload;
    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == _player)
        {
            StartCoroutine(LoadScenes());
            StartCoroutine(UnloadScenes());
        }
    }
    
    private IEnumerator LoadScenes()
    {
        // Get the PlayerController component
        PlayerController playerController = _player.GetComponent<PlayerController>();

        // Disable player's control before scene transition
        if (playerController != null)
        {
            //playerController.DisableControl();
        }

        for(int i = 0; i < _scenesToLoad.Length; i++)
        {
            bool isSceneLoaded = false;
            for(int j = 0; j < SceneManager.sceneCount; j++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(j);
                if(loadedScene.name == _scenesToLoad[i].SceneName)
                {
                    isSceneLoaded = true;
                    break;
                }
            }

            if(!isSceneLoaded)
            {
                yield return SceneManager.LoadSceneAsync(_scenesToLoad[i], LoadSceneMode.Additive);
            }
        }

        // Re-enable player's control after scene transition
        if (playerController != null)
        {
            //playerController.EnableControl();
        }
    }

    private IEnumerator UnloadScenes()
    {
        for(int i = 0; i < _scenesToUnload.Length; i++)
        {
            for(int j = 0; j < SceneManager.sceneCount; j++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(j);
                if(loadedScene.name == _scenesToUnload[i].SceneName)
                {
                    yield return SceneManager.UnloadSceneAsync(_scenesToUnload[i]);
                    break;
                }
            }
        }
    }
}
