using UnityEngine;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour
{
    [SerializeField] private SceneField sceneToLoad;
    //[SerializeField] private Transform playerStartPoint; // The point where the player will start in the new scene

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Save the start point position to a static variable or a singleton before loading the new scene
            //GameManager.Instance.playerStartPoint = playerStartPoint.position;
            SceneManager.LoadScene(sceneToLoad.SceneName);
        }
    }
}