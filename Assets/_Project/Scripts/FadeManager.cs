using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    [Header("Canvas et fade")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 0.5f;

    [Header("Audio optionnel")]
    public AudioSource audioSource;
    public AudioClip fadeOutSound;
    public AudioClip fadeInSound;

    private void Start()
    {
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 1f;        // commence à noir
            StartCoroutine(FadeIn());          // fade-in au démarrage
        }
    }

    public IEnumerator FadeIn()
    {
        if (audioSource != null && fadeInSound != null)
            audioSource.PlayOneShot(fadeInSound);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 0f;
    }

    public IEnumerator FadeOut()
    {
        if (audioSource != null && fadeOutSound != null)
            audioSource.PlayOneShot(fadeOutSound);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 1f;
    }

    // Méthode pratique pour changer de scène avec fade
    public void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(FadeAndLoad(sceneName));
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        yield return FadeOut();               // fade-out avant le changement
        yield return SceneManager.LoadSceneAsync(sceneName);
        // fade-in sera déclenché automatiquement par Start() dans la nouvelle scène
    }
}
