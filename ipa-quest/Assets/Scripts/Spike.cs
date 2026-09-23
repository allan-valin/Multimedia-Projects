/*using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Spike : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            StartCoroutine(RespawnPoint());
        }
    }

    IEnumerator RespawnPoint()
    {
        PlayerController.Instance.pState.cutscene = true;
        PlayerController.Instance.pState.invincible = true;
        PlayerController.Instance.rb.velocity = Vector2.zero;
        Time.timeScale = 0;
        StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.In));
        PlayerController.Instance.TakeDamage(1);
        yield return new WaitForSecondsRealtime(1);

        if (GameManager.Instance != null)
        {
            PlayerController.Instance.transform.position = GameManager.Instance.platformingRespawnPoint;
        }
        else
        {
            Debug.LogError("GameManager.Instance is null");
        }

        StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.Out));
        yield return new WaitForSecondsRealtime(UIManager.Instance.sceneFader.fadeTime);
        PlayerController.Instance.pState.cutscene = false;
        PlayerController.Instance.pState.invincible = false;
        Time.timeScale = 1;
    }
}
*/

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Spike : MonoBehaviour
{
    public Transform respawnPoint; // The respawn point the player should teleport to
    public Transform sceneStartPoint; // The scene's starting position

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            //StartCoroutine(RespawnPoint());
            PlayerController.Instance.transform.position = respawnPoint.position;
        }
    }

    IEnumerator RespawnPoint()
    {
        PlayerController.Instance.pState.cutscene = true;
        PlayerController.Instance.pState.invincible = true;
        PlayerController.Instance.rb.velocity = Vector2.zero;
        Time.timeScale = 0;
        StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.In));
        PlayerController.Instance.TakeDamage(1);
        yield return new WaitForSecondsRealtime(1);

        if (respawnPoint != null)
        {
            PlayerController.Instance.transform.position = respawnPoint.position;
        }
        else
        {
            PlayerController.Instance.transform.position = sceneStartPoint.position;
        }

        StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.Out));
        yield return new WaitForSecondsRealtime(UIManager.Instance.sceneFader.fadeTime);
        PlayerController.Instance.pState.cutscene = false;
        PlayerController.Instance.pState.invincible = false;
        Time.timeScale = 1;
    }
}