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

    private Canvas pauseCanvas;
    private int originalSortingOrder;

    void Awake()
    {
        if (pauseUI != null)
        {
            pauseUI.SetActive(false);
            
            pauseCanvas = pauseUI.GetComponent<Canvas>();
            if (pauseCanvas == null)
            {
                pauseCanvas = pauseUI.AddComponent<Canvas>();
                pauseUI.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            }
            
            pauseCanvas.overrideSorting = true;
            originalSortingOrder = pauseCanvas.sortingOrder;
            pauseCanvas.sortingOrder = 999;
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
        AudioManager.instance.PlayButtonClick();
    }

    void Pause()
    {
        if (pauseUI != null)
        {
            pauseUI.SetActive(true);
            
            if (pauseCanvas != null)
            {
                pauseCanvas.overrideSorting = true;
                pauseCanvas.sortingOrder = 999;
            }
        }

        AudioManager.instance.PlayButtonClick();
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Settings()
    {
        if (settingsCanvas != null)
        {
            settingsCanvas.gameObject.SetActive(true);
            settingsCanvas.overrideSorting = true;
            settingsCanvas.sortingOrder = 1000;
        }
        
        AudioManager.instance.PlayButtonClick();
    }

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
            UnityEngine.SceneManagement.SceneManager.LoadScene(menuSceneName);
        }

        AudioManager.instance.PlayButtonClick();
    }

    public void CloseSettings()
    {
        if (settingsCanvas != null)
            settingsCanvas.gameObject.SetActive(false);

        AudioManager.instance.PlayButtonClick();
    }
}