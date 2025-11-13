using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    [Header("Fade")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 0.5f;

    [Header("Audio")]
    public AudioClip soundTransition;

    [Header("Loading Screen")]
    public GameObject loadingScreen; // Canvas du loading
    public Image progressBar;        // Image type Filled

    // Méthode principale à appeler
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        // 1️⃣ Fade Out du menu
        yield return StartCoroutine(FadeOut());

        // 2️⃣ Active l'écran de chargement
        if (loadingScreen != null)
            loadingScreen.SetActive(true);

        // 3️⃣ Lance le chargement asynchrone
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        float displayedProgress = 0f; // Pour lisser la barre
        while (!asyncLoad.isDone)
        {
            // Unity limite asyncLoad.progress à 0.9 avant allowSceneActivation
            float targetProgress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            displayedProgress = Mathf.MoveTowards(displayedProgress, targetProgress, Time.deltaTime);
            if (progressBar != null)
                progressBar.fillAmount = displayedProgress;

            // Si le chargement est prêt
            if (asyncLoad.progress >= 0.9f)
            {
                // Attend un petit temps pour que la barre atteigne 100%
                if (progressBar != null)
                    progressBar.fillAmount = 1f;

                yield return new WaitForSeconds(0.5f);

                asyncLoad.allowSceneActivation = true; // Affiche la scène
            }

            yield return null;
        }
    }

    public void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator FadeOut()
    {
        if (soundTransition != null)
            AudioManager.instance.PlaySFX(soundTransition);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 1f;
    }

    public IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 0f;
    }
}