using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuPrincipal : MonoBehaviour
{
    [Header("Nom de la scène Jouer")]
    [SerializeField] private string sceneJouer = "Jeu";

    [Header("Référence FadeManager")]
    public FadeManager fadeManager; 

    [Header("Canvas Paramètres")]
    [SerializeField] private Canvas settingsCanvas;

    [Header("DDOL à supprimer au démarrage")]
    public string[] ddolNamesToDestroy;

    private void Awake()
    {
        // Supprime les objets DDOL avec les noms spécifiés
        foreach (string name in ddolNamesToDestroy)
        {
            GameObject obj = GameObject.Find(name);
            if (obj != null)
            {
                Destroy(obj);
            }
        }
    }

    // --- Bouton Jouer ---
    public void Jouer()
    {
        ChargerScene(sceneJouer);
    }

    // --- Bouton Paramètres ---
    public void Settings()
    {
        if (settingsCanvas != null)
            settingsCanvas.gameObject.SetActive(true);
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

    // Désactive le canvas des paramètres
    public void CloseSettings()
    {
        if (settingsCanvas != null)
            settingsCanvas.gameObject.SetActive(false);
    }
}
