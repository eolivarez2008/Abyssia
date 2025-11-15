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

    [Header("Sons personnalisés (optionnel)")]
    [SerializeField] private AudioClip customClickSound;
    [SerializeField] private AudioClip customHoverSound;

    private void Awake()
    {
        foreach (string name in ddolNamesToDestroy)
        {
            GameObject obj = GameObject.Find(name);
            if (obj != null)
            {
                Destroy(obj);
            }
        }
    }

    public void Jouer()
    {
        // Jouer le son de clic
        if (AudioManager.instance != null)
        {
            if (customClickSound != null)
                AudioManager.instance.PlaySound(customClickSound);
            else
                AudioManager.instance.PlayButtonClick();
        }

        if (LoadingManager.instance != null)
        {
            LoadingManager.instance.LoadScene(sceneJouer);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneJouer);
        }
    }

    public void Settings()
    {
        // Son d'ouverture de menu
        if (AudioManager.instance != null)
        {
            if (customClickSound != null)
                AudioManager.instance.PlaySound(customClickSound);
            else
                AudioManager.instance.PlayMenuOpen();
        }

        if (settingsCanvas != null)
            settingsCanvas.gameObject.SetActive(true);
    }

    public void Quitter()
    {
        // Son de clic
        if (AudioManager.instance != null)
        {
            if (customClickSound != null)
                AudioManager.instance.PlaySound(customClickSound);
            else
                AudioManager.instance.PlayButtonClick();
        }

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void CloseSettings()
    {
        // Son de fermeture
        if (AudioManager.instance != null)
        {
            if (customClickSound != null)
                AudioManager.instance.PlaySound(customClickSound);
            else
                AudioManager.instance.PlayMenuClose();
        }

        if (settingsCanvas != null)
            settingsCanvas.gameObject.SetActive(false);
    }

    // Méthode pour le hover (à appeler depuis les EventTriggers Unity)
    public void OnButtonHover()
    {
        if (AudioManager.instance != null)
        {
            if (customHoverSound != null)
                AudioManager.instance.PlaySound(customHoverSound, 0.5f);
            else
                AudioManager.instance.PlayButtonHover();
        }
    }
}