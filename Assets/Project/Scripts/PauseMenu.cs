using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [Header("Panel Pause UI")]
    public GameObject pauseUI;                 // Le panel avec boutons
    private bool isPaused = false;

    [Header("FadeManager de la scène")]
    public FadeManager fadeManager;            // Référence au FadeManager de la scène
    public string menuSceneName = "Menu";      // Nom exact de la scène MenuPrincipal

    [SerializeField] private Canvas settingsCanvas;

    private CanvasGroup pauseCanvasGroup;

    void Awake()
    {
        if (pauseUI != null)
        {
            pauseUI.SetActive(false);          // Masqué au démarrage
            pauseCanvasGroup = pauseUI.GetComponent<CanvasGroup>();

            // Si pas de CanvasGroup, on en ajoute un pour contrôle alpha & interaction
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
            // S’assure que le panel est au premier plan
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
            settingsCanvas.sortingOrder = 100; // Plus grand = devant les autres Canvas
        }
    }

    // --- Bouton quitter vers le menu avec fade ---
    public void LoadMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (fadeManager != null)
            fadeManager.LoadSceneWithFade(menuSceneName);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(menuSceneName);
    }

    // Désactive le canvas des paramètres
    public void CloseSettings()
    {
        if (settingsCanvas != null)
            settingsCanvas.gameObject.SetActive(false);
    }
}
