using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageFlashDynamic : MonoBehaviour
{
    public Image flashImage;
    public float flashDuration = 0.2f;
    public float maxAlpha = 0.6f;

    public void Flash(int damage, int maxHealth)
    {
        StopAllCoroutines();

        float intensity = Mathf.Clamp01((float)damage / maxHealth) * maxAlpha;

        intensity = Mathf.Max(intensity, 0.3f);

        StartCoroutine(DoFlash(intensity));
    }

    IEnumerator DoFlash(float intensity)
    {
        float t = 0f;
        while (t < flashDuration)
        {
            t += Time.deltaTime;
            flashImage.color = new Color(0.6f, 0f, 0f, Mathf.Lerp(0f, intensity, t / flashDuration));
            yield return null;
        }

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
