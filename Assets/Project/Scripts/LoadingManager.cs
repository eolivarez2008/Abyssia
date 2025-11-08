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
    public float minLoadingTime = 2f;

    [Header("Audio (Optional)")]
    public AudioClip transitionSound;

    public static LoadingManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        if (loadingScreen != null)
            loadingScreen.SetActive(false);

        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 0f;
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        yield return StartCoroutine(FadeOut());

        if (loadingScreen != null)
            loadingScreen.SetActive(true);

        if (loadingBarFill != null)
            loadingBarFill.fillAmount = 0f;

        if (transitionSound != null && AudioManager.instance != null)
            AudioManager.instance.PlaySFX(transitionSound);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;
        float elapsedTime = 0f;
        float fakeProgress = 0f;

        while (!asyncLoad.isDone)
        {
            elapsedTime += Time.deltaTime;

            float realProgress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

            fakeProgress = Mathf.Clamp01(elapsedTime / minLoadingTime);

            float displayProgress = Mathf.Min(realProgress, fakeProgress);

            if (loadingBarFill != null)
                loadingBarFill.fillAmount = displayProgress;

            if (loadingText != null)
                loadingText.text = $"Chargement... {Mathf.RoundToInt(displayProgress * 100)}%";

            if (asyncLoad.progress >= 0.9f && elapsedTime >= minLoadingTime)
            {
                if (loadingBarFill != null)
                    loadingBarFill.fillAmount = 1f;
                if (loadingText != null)
                    loadingText.text = "Chargement... 100%";

                yield return new WaitForSeconds(0.3f);

                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        if (loadingScreen != null)
            loadingScreen.SetActive(false);

        yield return StartCoroutine(FadeIn());
    }

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

    public void ReloadCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        LoadScene(currentScene);
    }

    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            LoadScene(SceneManager.GetSceneByBuildIndex(nextSceneIndex).name);
        }
    }
}