using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Gère le chargement des scènes avec écran de chargement et effets de fade
/// Pattern Singleton avec DontDestroyOnLoad
/// </summary>
public class LoadingManager : MonoBehaviour
{
    [Header("Fade Settings")]
    [Tooltip("CanvasGroup pour l'effet de fade")]
    public CanvasGroup fadeCanvasGroup;
    
    [Tooltip("Durée du fade en secondes")]
    public float fadeDuration = 1f;

    [Header("Loading Screen")]
    [Tooltip("GameObject de l'écran de chargement")]
    public GameObject loadingScreen;
    
    [Tooltip("Barre de progression du chargement")]
    public Image loadingBarFill;
    
    [Tooltip("Texte affichant le pourcentage de chargement")]
    public Text loadingText;
    
    [Tooltip("Temps minimum d'affichage de l'écran de chargement")]
    public float minLoadingTime = 2f;

    [Header("Audio (Optional)")]
    [Tooltip("Son joué lors de la transition")]
    public AudioClip transitionSound;

    public static LoadingManager instance;

    private void Awake()
    {
        // Pattern Singleton avec DontDestroyOnLoad
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Désactive l'écran de chargement au démarrage
        if (loadingScreen != null)
            loadingScreen.SetActive(false);

        // Initialise le fade à transparent
        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 0f;
    }

    /// <summary>
    /// Charge une scène avec écran de chargement et transition
    /// </summary>
    /// <param name="sceneName">Nom de la scène à charger</param>
    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("LoadingManager: Nom de scène vide!");
            return;
        }

        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    /// <summary>
    /// Coroutine gérant le chargement avec effets visuels
    /// </summary>
    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        // Fade out
        yield return StartCoroutine(FadeOut());

        // Active l'écran de chargement
        if (loadingScreen != null)
            loadingScreen.SetActive(true);

        // Réinitialise la barre de progression
        if (loadingBarFill != null)
            loadingBarFill.fillAmount = 0f;

        // Joue le son de transition
        if (transitionSound != null && AudioManager.instance != null)
            AudioManager.instance.PlaySFX(transitionSound);

        // Lance le chargement asynchrone
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;
        
        float elapsedTime = 0f;
        float fakeProgress = 0f;

        // Attend que le chargement soit terminé ET que le temps minimum soit écoulé
        while (!asyncLoad.isDone)
        {
            elapsedTime += Time.deltaTime;

            // Progression réelle du chargement Unity (0-0.9)
            float realProgress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

            // Progression artificielle basée sur le temps écoulé
            fakeProgress = Mathf.Clamp01(elapsedTime / minLoadingTime);

            // Utilise la plus petite des deux progressions
            float displayProgress = Mathf.Min(realProgress, fakeProgress);

            // Met à jour la barre et le texte
            if (loadingBarFill != null)
                loadingBarFill.fillAmount = displayProgress;

            if (loadingText != null)
                loadingText.text = $"Chargement... {Mathf.RoundToInt(displayProgress * 100)}%";

            // Active la scène une fois que tout est prêt
            if (asyncLoad.progress >= 0.9f && elapsedTime >= minLoadingTime)
            {
                // Affiche 100%
                if (loadingBarFill != null)
                    loadingBarFill.fillAmount = 1f;
                if (loadingText != null)
                    loadingText.text = "Chargement... 100%";

                yield return new WaitForSeconds(0.3f);

                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        // Désactive l'écran de chargement
        if (loadingScreen != null)
            loadingScreen.SetActive(false);

        // Fade in
        yield return StartCoroutine(FadeIn());
    }

    /// <summary>
    /// Fait un fade vers l'opaque
    /// </summary>
    private IEnumerator FadeOut()
    {
        if (fadeCanvasGroup == null) yield break;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 1f;
    }

    /// <summary>
    /// Fait un fade vers le transparent
    /// </summary>
    private IEnumerator FadeIn()
    {
        if (fadeCanvasGroup == null) yield break;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 0f;
    }

    /// <summary>
    /// Recharge la scène actuelle
    /// </summary>
    public void ReloadCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        LoadScene(currentScene);
    }

    /// <summary>
    /// Charge la scène suivante dans le Build Settings
    /// </summary>
    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            string nextSceneName = System.IO.Path.GetFileNameWithoutExtension(
                SceneUtility.GetScenePathByBuildIndex(nextSceneIndex)
            );
            LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("LoadingManager: Aucune scène suivante disponible");
        }
    }
}