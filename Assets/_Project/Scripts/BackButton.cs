using UnityEngine;

public class UniversalBackButton : MonoBehaviour
{
    [Header("Référence FadeManager")]
    public FadeManager fadeManager; // Drag & drop du FadeManager de la scène

    public string menuSceneName = "Menu"; // Nom exact de la scène MenuPrincipal

    public void GoToMenu()
    {
        if (fadeManager != null)
        {
            fadeManager.LoadSceneWithFade(menuSceneName);
        }
        else
        {
            Debug.LogWarning("UniversalBackButton: FadeManager non assigné, chargement direct !");
            UnityEngine.SceneManagement.SceneManager.LoadScene(menuSceneName);
        }
    }
}
