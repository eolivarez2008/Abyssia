using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gère l'effet visuel de flash rouge quand le joueur prend des dégâts
/// L'intensité du flash varie selon les dégâts reçus
/// </summary>
public class DamageFlashDynamic : MonoBehaviour
{
    [Header("Flash Settings")]
    [Tooltip("Image utilisée pour l'effet de flash")]
    public Image flashImage;
    
    [Tooltip("Durée du flash en secondes")]
    public float flashDuration = 0.2f;
    
    [Tooltip("Opacité maximale du flash (0-1)")]
    [Range(0f, 1f)]
    public float maxAlpha = 0.6f;
    
    [Tooltip("Opacité minimale garantie du flash")]
    [Range(0f, 1f)]
    public float minAlpha = 0.3f;

    /// <summary>
    /// Déclenche un flash avec une intensité basée sur les dégâts
    /// </summary>
    /// <param name="damage">Dégâts reçus</param>
    /// <param name="maxHealth">Vie maximale pour calculer l'intensité</param>
    public void Flash(int damage, int maxHealth)
    {
        if (flashImage == null)
        {
            Debug.LogWarning("DamageFlash: flashImage non assignée!");
            return;
        }

        // Arrête les flash précédents
        StopAllCoroutines();

        // Calcule l'intensité basée sur le ratio dégâts/vie max
        float intensity = Mathf.Clamp01((float)damage / maxHealth) * maxAlpha;
        
        // Garantit une intensité minimale
        intensity = Mathf.Max(intensity, minAlpha);

        StartCoroutine(DoFlash(intensity));
    }

    /// <summary>
    /// Coroutine effectuant le flash (fade in puis fade out)
    /// </summary>
    private IEnumerator DoFlash(float intensity)
    {
        // Fade in
        float t = 0f;
        while (t < flashDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, intensity, t / flashDuration);
            flashImage.color = new Color(0.6f, 0f, 0f, alpha);
            yield return null;
        }

        // Fade out
        t = 0f;
        while (t < flashDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(intensity, 0f, t / flashDuration);
            flashImage.color = new Color(0.6f, 0f, 0f, alpha);
            yield return null;
        }

        // S'assure que le flash est complètement transparent
        flashImage.color = new Color(0.6f, 0f, 0f, 0f);
    }
}