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
                UnityEngine.SceneManagement.SceneManager.LoadScene(levelToLoad);
            }
        }
    }
}