using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor; // Seulement dans l’éditeur
#endif

public class MenuPrincipal : MonoBehaviour
{
    [Header("Scènes")]
    [SerializeField] private SceneReference sceneJouer;
    [SerializeField] private SceneReference sceneSettings;

    // -------------------
    // Bouton Jouer
    public void Jouer()
    {
        if (!string.IsNullOrEmpty(sceneJouer.SceneName))
            SceneManager.LoadScene(sceneJouer.SceneName);
        else
            Debug.LogWarning("MenuPrincipal: La scène Jouer n'est pas définie !");
    }

    // Bouton Settings
    public void Settings()
    {
        if (!string.IsNullOrEmpty(sceneSettings.SceneName))
            SceneManager.LoadScene(sceneSettings.SceneName);
        else
            Debug.LogWarning("MenuPrincipal: La scène Settings n'est pas définie !");
    }

    // Bouton Quitter
    public void Quitter()
    {
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}

[System.Serializable]
public class SceneReference
{
#if UNITY_EDITOR
    [Header("Choisir une scène")]
    [SerializeField] private SceneAsset sceneAsset; 
#endif

    [SerializeField, HideInInspector] private string sceneName;

    public string SceneName => sceneName;

#if UNITY_EDITOR
    // Met à jour automatiquement le nom quand on choisit la scène dans l’inspector
    void OnValidate()
    {
        if (sceneAsset != null)
        {
            sceneName = sceneAsset.name;
        }
    }
#endif
}
