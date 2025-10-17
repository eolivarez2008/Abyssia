using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInitializer : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Cherche un éventuel panel pause dans la nouvelle scène
        var pauseMenu = Object.FindAnyObjectByType<PauseMenu>(); // nouvelle méthode
        if (pauseMenu != null && pauseMenu.pauseUI != null)
        {
            pauseMenu.pauseUI.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}
