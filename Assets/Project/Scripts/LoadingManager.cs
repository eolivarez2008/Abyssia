using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    [Header("Fade Settings")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1f;

    [Header("Loading Screen")]
    public GameObject loadingScreen;
    public Image loadingBarFill;
    public Text loadingText;
    public float minLoadingTime = 2f; // Temps minimum d'affichage du loading

    [Header("Audio (Optional)")]
    public AudioClip transitionSound;

    public static LoadingManager instance;

    private void Awake()
    {
        // Singleton DDOL
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Cache le loading screen au départ
        if (loadingScreen != null)
            loadingScreen.SetActive(false);

        // Fade invisible au départ
        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 0f;
    }

    /// <summary>
    /// Charge une scène avec fade + loading screen
    /// </summary>
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        // 1. FADE OUT (écran devient noir)
        yield return StartCoroutine(FadeOut());

        // 2. AFFICHE LE LOADING SCREEN
        if (loadingScreen != null)
            loadingScreen.SetActive(true);

        // Réinitialise la barre
        if (loadingBarFill != null)
            loadingBarFill.fillAmount = 0f;

        // Joue le son de transition
        if (transitionSound != null && AudioManager.instance != null)
            AudioManager.instance.PlaySFX(transitionSound);

        // 3. CHARGE LA SCÈNE EN ARRIÈRE-PLAN
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false; // Empêche l'activation automatique

        float elapsedTime = 0f;
        float fakeProgress = 0f;

        // Affiche la progression (avec un minimum de temps)
        while (!asyncLoad.isDone)
        {
            elapsedTime += Time.deltaTime;

            // Progression réelle de Unity (va jusqu'à 0.9)
            float realProgress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

            // Progression "fake" basée sur le temps minimum
            fakeProgress = Mathf.Clamp01(elapsedTime / minLoadingTime);

            // Prend la plus petite des deux (pour pas aller trop vite)
            float displayProgress = Mathf.Min(realProgress, fakeProgress);

            // Met à jour la barre
            if (loadingBarFill != null)
                loadingBarFill.fillAmount = displayProgress;

            // Met à jour le texte
            if (loadingText != null)
                loadingText.text = $"Chargement... {Mathf.RoundToInt(displayProgress * 100)}%";

            // Si le chargement est terminé ET le temps minimum écoulé
            if (asyncLoad.progress >= 0.9f && elapsedTime >= minLoadingTime)
            {
                // Finit la barre à 100%
                if (loadingBarFill != null)
                    loadingBarFill.fillAmount = 1f;
                if (loadingText != null)
                    loadingText.text = "Chargement... 100%";

                yield return new WaitForSeconds(0.3f); // Petit délai pour voir le 100%

                asyncLoad.allowSceneActivation = true; // Active la scène
            }

            yield return null;
        }

        // 4. CACHE LE LOADING SCREEN
        if (loadingScreen != null)
            loadingScreen.SetActive(false);

        // 5. FADE IN (retour à la normale)
        yield return StartCoroutine(FadeIn());
    }

    /// <summary>
    /// Fade vers le noir
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
    /// Fade vers la transparence
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
    /// Charge la scène suivante (utile pour des niveaux)
    /// </summary>
    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            LoadScene(SceneManager.GetSceneByBuildIndex(nextSceneIndex).name);
        }
        else
        {
            Debug.LogWarning("LoadingManager: Pas de scène suivante !");
        }
    }
}