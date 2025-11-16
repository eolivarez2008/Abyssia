using UnityEngine;

/// <summary>
/// Charge une scène quand le joueur entre dans le trigger
/// </summary>
public class sceneLoader : MonoBehaviour
{
    [Header("Scène à charger")]
    [Tooltip("Nom de la scène à charger")]
    public string levelToLoad;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Vérifie que c'est le joueur
        if (!collision.CompareTag("Player"))
            return;

        if (string.IsNullOrEmpty(levelToLoad))
        {
            Debug.LogWarning("SceneLoader: levelToLoad est vide!");
            return;
        }

        // Charge la scène avec le LoadingManager si disponible
        if (LoadingManager.instance != null)
        {
            LoadingManager.instance.LoadScene(levelToLoad);
        }
        else
        {
            // Fallback si LoadingManager n'existe pas
            UnityEngine.SceneManagement.SceneManager.LoadScene(levelToLoad);
        }
    }
}