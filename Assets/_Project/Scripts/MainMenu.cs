using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuPrincipal : MonoBehaviour
{
    [Header("Nom des scènes")]
    [SerializeField] private string sceneJouer = "Jeu";
    [SerializeField] private string sceneSettings = "Settings";

    [Header("Référence FadeManager")]
    public FadeManager fadeManager; // Drag & drop du FadeManager de la scène

    // --- Bouton Jouer ---
    public void Jouer()
    {
        ChargerScene(sceneJouer);
    }

    // --- Bouton Paramètres ---
    public void Settings()
    {
        ChargerScene(sceneSettings);
    }

    // --- Bouton Quitter ---
    public void Quitter()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // --- Méthode utilitaire ---
    private void ChargerScene(string nomScene)
    {
        if (string.IsNullOrEmpty(nomScene))
        {
            Debug.LogWarning("MenuPrincipal: le nom de la scène est vide !");
            return;
        }

        if (fadeManager != null)
        {
            fadeManager.LoadSceneWithFade(nomScene);
        }
        else
        {
            Debug.LogWarning("MenuPrincipal: FadeManager non assigné, chargement direct !");
            UnityEngine.SceneManagement.SceneManager.LoadScene(nomScene);
        }
    }
}
