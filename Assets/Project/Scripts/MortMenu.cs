using UnityEngine;

public class MortMenu : MonoBehaviour
{
    [Header("Panel Mort UI")]
    public GameObject mortUI;
    public string menuSceneName = "Menu";

    private CanvasGroup mortCanvasGroup;

    void Awake()
    {
        if (mortUI != null)
        {
            mortUI.SetActive(false);
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

            Canvas canvas = mortUI.GetComponentInParent<Canvas>();
            if (canvas != null)
                canvas.sortingOrder = 100;
        }

        Time.timeScale = 0f;
    }

    /// <summary>
    /// Bouton retour au menu avec LoadingManager
    /// </summary>
    public void RetourMenu()
    {
        Time.timeScale = 1f;
        AudioManager.instance.PlayButtonClick();

        if (LoadingManager.instance != null)
        {
            LoadingManager.instance.LoadScene(menuSceneName);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(menuSceneName);
        }
    }
}