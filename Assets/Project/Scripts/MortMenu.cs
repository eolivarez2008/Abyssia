using UnityEngine;

/// <summary>
/// Gère le menu affiché à la mort du joueur
/// </summary>
public class MortMenu : MonoBehaviour
{
    [Header("Panel Mort UI")]
    [Tooltip("GameObject du panneau de mort")]
    public GameObject mortUI;
    
    [Tooltip("Nom de la scène du menu principal")]
    public string menuSceneName = "Menu";

    private CanvasGroup mortCanvasGroup;

    void Awake()
    {
        if (mortUI != null)
        {
            // Désactive le panneau au démarrage
            mortUI.SetActive(false);
            
            // Configure ou récupère le CanvasGroup
            mortCanvasGroup = mortUI.GetComponent<CanvasGroup>();
            if (mortCanvasGroup == null)
                mortCanvasGroup = mortUI.AddComponent<CanvasGroup>();

            mortCanvasGroup.alpha = 1f;
            mortCanvasGroup.interactable = true;
            mortCanvasGroup.blocksRaycasts = true;
        }
        else
        {
            Debug.LogWarning("MortMenu: mortUI non assigné!");
        }
    }

    /// <summary>
    /// Active le panneau de mort et met le jeu en pause
    /// </summary>
    public void ActiverPanelMort()
    {
        if (mortUI != null)
        {
            mortUI.SetActive(true);

            // S'assure que le canvas est au-dessus de tout
            Canvas canvas = mortUI.GetComponentInParent<Canvas>();
            if (canvas != null)
                canvas.sortingOrder = 100;
        }

        // Met le jeu en pause
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Retourne au menu principal (appelé par le bouton)
    /// </summary>
    public void RetourMenu()
    {
        // Restaure le temps normal
        Time.timeScale = 1f;
        
        if (AudioManager.instance != null)
            AudioManager.instance.PlayButtonClick();

        // Charge le menu principal
        if (LoadingManager.instance != null)
        {
            LoadingManager.instance.LoadScene(menuSceneName);
        }
        else
        {
            // Fallback si LoadingManager n'existe pas
            UnityEngine.SceneManagement.SceneManager.LoadScene(menuSceneName);
        }
    }
}