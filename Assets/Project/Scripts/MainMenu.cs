using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Gère le menu principal du jeu
/// </summary>
public class MenuPrincipal : MonoBehaviour
{
    [Header("Nom de la scène Jouer")]
    [Tooltip("Nom de la scène de jeu à charger")]
    [SerializeField] private string sceneJouer = "Jeu";

    [Header("Canvas Paramètres")]
    [Tooltip("Canvas du menu des paramètres")]
    [SerializeField] private Canvas settingsCanvas;

    [Header("DDOL à supprimer au démarrage")]
    [Tooltip("Noms des objets DontDestroyOnLoad à détruire au retour au menu")]
    public string[] ddolNamesToDestroy;

    [Header("Sons personnalisés (optionnel)")]
    [Tooltip("Son personnalisé pour les clics")]
    [SerializeField] private AudioClip customClickSound;
    
    [Tooltip("Son personnalisé pour le survol")]
    [SerializeField] private AudioClip customHoverSound;

    private void Awake()
    {
        // Détruit les objets persistants spécifiés au retour au menu
        if (ddolNamesToDestroy != null)
        {
            foreach (string name in ddolNamesToDestroy)
            {
                if (string.IsNullOrEmpty(name)) continue;
                
                GameObject obj = GameObject.Find(name);
                if (obj != null)
                {
                    Destroy(obj);
                    Debug.Log($"MenuPrincipal: {name} détruit");
                }
            }
        }
    }

    /// <summary>
    /// Lance le jeu
    /// </summary>
    public void Jouer()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayButtonClick();
        }

        if (LoadingManager.instance != null)
        {
            LoadingManager.instance.LoadScene(sceneJouer);
        }
        else
        {
            // Fallback si LoadingManager n'existe pas
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneJouer);
        }
    }

    /// <summary>
    /// Ouvre le menu des paramètres
    /// </summary>
    public void Settings()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayButtonClick();
        }

        if (settingsCanvas != null)
            settingsCanvas.gameObject.SetActive(true);
    }

    /// <summary>
    /// Quitte le jeu
    /// </summary>
    public void Quitter()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayButtonClick();
        }

#if UNITY_EDITOR
        // Arrête le mode play dans l'éditeur
        EditorApplication.isPlaying = false;
#else
        // Quitte l'application
        Application.Quit();
#endif
    }

    /// <summary>
    /// Ferme le menu des paramètres
    /// </summary>
    public void CloseSettings()
    {
        if (AudioManager.instance != null)
        {
            if (customClickSound != null)
                AudioManager.instance.PlaySound(customClickSound);
            else
                AudioManager.instance.PlayButtonClick();
        }

        if (settingsCanvas != null)
            settingsCanvas.gameObject.SetActive(false);
    }

    /// <summary>
    /// Appelé quand le curseur survole un bouton (pour Event Trigger)
    /// </summary>
    public void OnButtonHover()
    {
        if (AudioManager.instance != null)
        {
            if (customHoverSound != null)
                AudioManager.instance.PlaySound(customHoverSound, 0.5f);
            else
                AudioManager.instance.PlayButtonClick();
        }
    }
}