using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneLoader : MonoBehaviour
{
    public string levelToLoad;
    public FadeManager fadeManager; // référence directe au FadeManager de la scène

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && fadeManager != null)
        {
            fadeManager.LoadSceneWithFade(levelToLoad);
        }
    }
}
