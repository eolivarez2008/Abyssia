using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseUI;
    private bool isPaused = false;

    void Update()
    {
        // Quand on appuie sur Echap
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    // Reprendre la partie
    public void Resume()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1f; // le temps reprend
        isPaused = false;
    }

    // Mettre en pause
    void Pause()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0f; // stop le temps
        isPaused = true;
    }

    // Retour au menu principal
    public void LoadMenu()
    {
        Time.timeScale = 1f; // toujours remettre à 1
        SceneManager.LoadScene("Menu"); // ⚠️ Mets ici le nom EXACT de ta scène MenuPrincipal
    }

    // Aller dans les paramètres
    public void LoadSettings()
    {
        Time.timeScale = 1f; // remet le temps à la normale (sinon ton jeu resterait figé en revenant)
        SceneManager.LoadScene("Settings"); // ⚠️ Mets ici le nom EXACT de ta scène Settings
    }

//     public void QuitGame()
//     {
//         Time.timeScale = 1f; // au cas où
//         Application.Quit();
// #if UNITY_EDITOR
//         UnityEditor.EditorApplication.isPlaying = false; // seulement dans l’éditeur Unity
// #endif
//     }
}
