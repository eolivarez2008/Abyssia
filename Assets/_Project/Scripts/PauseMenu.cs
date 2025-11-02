using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseUI;
    private bool isPaused = false;

    [Header("FadeManager de la scène")]
    public FadeManager fadeManager; // Drag & drop du FadeManager de la scène

    public string menuSceneName = "Menu"; // Nom exact de la scène MenuPrincipal

    void Start()
    {
        Time.timeScale = 1f;
        pauseUI.SetActive(false);
        isPaused = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    void Pause()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    // --- Bouton quitter vers le menu avec fade ---
    public void LoadMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (fadeManager != null)
            fadeManager.LoadSceneWithFade(menuSceneName);
        else
        {
            Debug.LogWarning("PauseMenu: FadeManager non assigné, chargement direct !");
            UnityEngine.SceneManagement.SceneManager.LoadScene(menuSceneName);
        }
    }
}
