using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string levelToLoad;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(LoadSceneWithFade(levelToLoad));
        }
    }

    IEnumerator LoadSceneWithFade(string sceneName)
    {
        if (FadeManager.Instance != null)
            yield return FadeManager.Instance.FadeOut();

        yield return SceneManager.LoadSceneAsync(sceneName);

        // Déplacer le joueur sur le spawnpoint de la nouvelle scène
        yield return null; // attendre 1 frame
        GameObject player = GameObject.FindWithTag("Player");
        GameObject spawn = GameObject.FindWithTag("SpawnPoint");

        if (player != null && spawn != null)
            player.transform.position = spawn.transform.position;
    }
}
