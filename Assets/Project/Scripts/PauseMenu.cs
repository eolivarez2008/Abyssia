using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [Header("Panel Pause UI")]
    public GameObject pauseUI;
    private bool isPaused = false;

    [Header("Menu Scene")]
    public string menuSceneName = "Menu";

    [Header("Settings Canvas")]
    [SerializeField] private Canvas settingsCanvas;

    private CanvasGroup pauseCanvasGroup;

    void Awake()
    {
        if (pauseUI != null)
        {
            pauseUI.SetActive(false);
            pauseCanvasGroup = pauseUI.GetComponent<CanvasGroup>();

            if (pauseCanvasGroup == null)
                pauseCanvasGroup = pauseUI.AddComponent<CanvasGroup>();

            pauseCanvasGroup.alpha = 1f;
            pauseCanvasGroup.interactable = true;
            pauseCanvasGroup.blocksRaycasts = true;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Resume()
    {
        if (pauseUI != null)
            pauseUI.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
    }

    void Pause()
    {
        if (pauseUI != null)
        {
            pauseUI.SetActive(true);
            Canvas canvas = pauseUI.GetComponentInParent<Canvas>();
            if (canvas != null)
                canvas.sortingOrder = 100;
        }

        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Settings()
    {
        if (settingsCanvas != null)
        {
            settingsCanvas.gameObject.SetActive(true);
            settingsCanvas.sortingOrder = 100;
        }
    }

    /// <summary>
    /// Retour au menu avec LoadingManager
    /// </summary>
    public void LoadMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (LoadingManager.instance != null)
        {
            LoadingManager.instance.LoadScene(menuSceneName);
        }
        else
        {
            Debug.LogWarning("LoadingManager introuvable, chargement direct !");
            UnityEngine.SceneManagement.SceneManager.LoadScene(menuSceneName);
        }
    }

    public void CloseSettings()
    {
        if (settingsCanvas != null)
            settingsCanvas.gameObject.SetActive(false);
    }
}