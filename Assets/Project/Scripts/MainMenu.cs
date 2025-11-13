using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuPrincipal : MonoBehaviour
{
    [Header("Nom de la scène Jouer")]
    [SerializeField] private string sceneJouer = "Jeu";

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
        if (LoadingManager.instance != null)
        {
            LoadingManager.instance.LoadScene(sceneJouer);
        }
        else
        {
            Debug.LogWarning("MenuPrincipal: LoadingManager introuvable, chargement direct !");
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneJouer);
        }
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

    // Désactive le canvas des paramètres
    public void CloseSettings()
    {
        if (settingsCanvas != null)
            settingsCanvas.gameObject.SetActive(false);
    }
}