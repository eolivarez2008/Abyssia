using UnityEngine;

/// <summary>
/// Gère le menu de pause du jeu
/// </summary>
public class PauseMenu : MonoBehaviour
{
    [Header("Panel Pause UI")]
    [Tooltip("GameObject du panneau de pause")]
    public GameObject pauseUI;
    
    [Header("Menu Scene")]
    [Tooltip("Nom de la scène du menu principal")]
    public string menuSceneName = "Menu";

    [Header("Settings Canvas")]
    [Tooltip("Canvas du menu des paramètres")]
    [SerializeField] private Canvas settingsCanvas;

    [Header("Input Settings")]
    [Tooltip("Touche pour ouvrir/fermer la pause")]
    public KeyCode pauseKey = KeyCode.Escape;

    private bool isPaused = false;
    private Canvas pauseCanvas;
    private int originalSortingOrder;

    void Awake()
    {
        if (pauseUI != null)
        {
            // Désactive le panneau au démarrage
            pauseUI.SetActive(false);
            
            // Configure le canvas
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
        else
        {
            Debug.LogWarning("PauseMenu: pauseUI non assigné!");
        }
    }

    void Update()
    {
        // Toggle pause avec Escape (ou touche configurée)
        if (Input.GetKeyDown(pauseKey))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    /// <summary>
    /// Reprend le jeu
    /// </summary>
    public void Resume()
    {
        if (pauseUI != null)
            pauseUI.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
        
        if (AudioManager.instance != null)
            AudioManager.instance.PlayButtonClick();
    }

    /// <summary>
    /// Met le jeu en pause
    /// </summary>
    void Pause()
    {
        if (pauseUI != null)
        {
            pauseUI.SetActive(true);
            
            // S'assure que le canvas est au-dessus de tout
            if (pauseCanvas != null)
            {
                pauseCanvas.overrideSorting = true;
                pauseCanvas.sortingOrder = 999;
            }
        }

        if (AudioManager.instance != null)
            AudioManager.instance.PlayButtonClick();
        
        Time.timeScale = 0f;
        isPaused = true;
    }

    /// <summary>
    /// Ouvre le menu des paramètres
    /// </summary>
    public void Settings()
    {
        if (settingsCanvas != null)
        {
            settingsCanvas.gameObject.SetActive(true);
            settingsCanvas.overrideSorting = true;
            settingsCanvas.sortingOrder = 1000; // Au-dessus de la pause
        }
        
        if (AudioManager.instance != null)
            AudioManager.instance.PlayButtonClick();
    }

    /// <summary>
    /// Retourne au menu principal
    /// </summary>
    public void LoadMenu()
    {
        // Restaure le temps normal
        Time.timeScale = 1f;
        isPaused = false;

        if (LoadingManager.instance != null)
        {
            LoadingManager.instance.LoadScene(menuSceneName);
        }
        else
        {
            // Fallback si LoadingManager n'existe pas
            UnityEngine.SceneManagement.SceneManager.LoadScene(menuSceneName);
        }

        if (AudioManager.instance != null)
            AudioManager.instance.PlayButtonClick();
    }

    /// <summary>
    /// Ferme le menu des paramètres
    /// </summary>
    public void CloseSettings()
    {
        if (settingsCanvas != null)
            settingsCanvas.gameObject.SetActive(false);

        if (AudioManager.instance != null)
            AudioManager.instance.PlayButtonClick();
    }
}