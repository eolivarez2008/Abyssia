using UnityEngine;

public class sceneLoader : MonoBehaviour
{
    public string levelToLoad;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (LoadingManager.instance != null)
            {
                LoadingManager.instance.LoadScene(levelToLoad);
            }
            else
            {
                // Fallback si LoadingManager n'existe pas
                Debug.LogWarning("LoadingManager introuvable, chargement direct !");
                UnityEngine.SceneManagement.SceneManager.LoadScene(levelToLoad);
            }
        }
    }
}