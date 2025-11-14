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
        if (settingsCanvas != null)
            settingsCanvas.gameObject.SetActive(true);
    }

    public void Quitter()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void CloseSettings()
    {
        if (settingsCanvas != null)
            settingsCanvas.gameObject.SetActive(false);
    }
}