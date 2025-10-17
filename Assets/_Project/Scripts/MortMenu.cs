using UnityEngine;

public class MortMenu : MonoBehaviour
{
    [Header("Panel Mort UI")]
    public GameObject mortUI;               // Le panel avec texte et bouton
    public string menuSceneName = "Menu";   // Nom exact de la scène MenuPrincipal

    [Header("FadeManager de la scène")]
    public FadeManager fadeManager;         // Drag & drop du FadeManager de la scène

    private CanvasGroup mortCanvasGroup;

    void Awake()
    {
        if (mortUI != null)
        {
            mortUI.SetActive(false); // Panel masqué au départ
            mortCanvasGroup = mortUI.GetComponent<CanvasGroup>();

            if (mortCanvasGroup == null)
                mortCanvasGroup = mortUI.AddComponent<CanvasGroup>();

            mortCanvasGroup.alpha = 1f;
            mortCanvasGroup.interactable = true;
            mortCanvasGroup.blocksRaycasts = true;
        }
    }

    /// <summary>
    /// Appeler quand le joueur meurt
    /// </summary>
    public void ActiverPanelMort()
    {
        if (mortUI != null)
        {
            mortUI.SetActive(true);

            // Assure que le panel est au premier plan
            Canvas canvas = mortUI.GetComponentInParent<Canvas>();
            if (canvas != null)
                canvas.sortingOrder = 100;
        }

        Time.timeScale = 0f; // met le jeu en pause
    }

    /// <summary>
    /// Bouton retour au menu
    /// </summary>
    public void RetourMenu()
    {
        Time.timeScale = 1f;

        if (fadeManager != null)
            fadeManager.LoadSceneWithFade(menuSceneName);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(menuSceneName);
    }
}
