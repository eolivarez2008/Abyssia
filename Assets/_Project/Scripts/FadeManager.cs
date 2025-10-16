using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 0.5f;

    // 🔊 Ajout audio
    public AudioSource audioSource;
    public AudioClip fadeOutSound;
    public AudioClip fadeInSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        fadeCanvasGroup.alpha = 1f;
        StartCoroutine(FadeIn());
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        fadeCanvasGroup.alpha = 1f;
        StartCoroutine(FadeIn());
    }

    public IEnumerator FadeIn()
    {
        // 🔊 Joue le son de fade-in
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
        // 🔊 Joue le son de fade-out
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
}
