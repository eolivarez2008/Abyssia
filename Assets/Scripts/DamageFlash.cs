using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageFlashDynamic : MonoBehaviour
{
    public Image flashImage;
    public float flashDuration = 0.2f;
    public float maxAlpha = 0.6f; // alpha max possible

    public void Flash(int damage, int maxHealth)
    {
        StopAllCoroutines();

        // Calcule alpha proportionnel aux dégâts
        float intensity = Mathf.Clamp01((float)damage / maxHealth) * maxAlpha;

        // Minimum d’intensité pour que le flash soit toujours visible
        intensity = Mathf.Max(intensity, 0.3f); // par exemple 0.2f minimum

        StartCoroutine(DoFlash(intensity));
    }

    IEnumerator DoFlash(float intensity)
    {
        // Fade in
        float t = 0f;
        while (t < flashDuration)
        {
            t += Time.deltaTime;
            flashImage.color = new Color(0.6f, 0f, 0f, Mathf.Lerp(0f, intensity, t / flashDuration));
            yield return null;
        }

        // Fade out
        t = 0f;
        while (t < flashDuration)
        {
            t += Time.deltaTime;
            flashImage.color = new Color(0.6f, 0f, 0f, Mathf.Lerp(intensity, 0f, t / flashDuration));
            yield return null;
        }

        flashImage.color = new Color(0.6f, 0f, 0f, 0f);
    }
}
