using UnityEngine;
using System.Collections;

/// <summary>
/// Gère les effets temporaires appliqués au joueur (vitesse, dégâts, invincibilité)
/// </summary>
public class PlayerEffects : MonoBehaviour
{
    /// <summary>
    /// Ajoute un effet de vitesse au joueur
    /// </summary>
    /// <param name="speedGiven">Bonus de vitesse</param>
    /// <param name="speedDuration">Durée en secondes</param>
    public void AddSpeed(int speedGiven, float speedDuration)
    {
        if (ConfigPlayer.instance == null)
        {
            Debug.LogWarning("PlayerEffects: ConfigPlayer.instance est null!");
            return;
        }

        // Applique le bonus de vitesse
        ConfigPlayer.instance.moveSpeed += speedGiven;
        
        // Ajoute l'effet visuel
        ConfigPlayer.instance.AddVisualEffect("speed");
        
        // Met à jour l'UI
        if (ActiveEffectsUI.instance != null)
        {
            ActiveEffectsUI.instance.AddSpeedEffect(speedDuration);
        }
        
        // Lance la coroutine pour retirer l'effet
        StartCoroutine(RemoveSpeed(speedGiven, speedDuration));
    }

    /// <summary>
    /// Retire l'effet de vitesse après la durée
    /// </summary>
    private IEnumerator RemoveSpeed(int speedGiven, float speedDuration)
    {
        yield return new WaitForSeconds(speedDuration);
        
        if (ConfigPlayer.instance != null)
        {
            ConfigPlayer.instance.moveSpeed -= speedGiven;
            ConfigPlayer.instance.RemoveVisualEffect("speed");
        }
        
        if (ActiveEffectsUI.instance != null)
        {
            ActiveEffectsUI.instance.RemoveSpeedEffect();
        }
    }

    /// <summary>
    /// Ajoute un effet de dégâts au joueur
    /// </summary>
    /// <param name="damageGiven">Bonus de dégâts</param>
    /// <param name="damageDuration">Durée en secondes</param>
    public void AddDamage(int damageGiven, float damageDuration)
    {
        if (ConfigPlayer.instance == null)
        {
            Debug.LogWarning("PlayerEffects: ConfigPlayer.instance est null!");
            return;
        }

        // Applique le bonus de dégâts
        ConfigPlayer.instance.damage += damageGiven;
        
        // Ajoute l'effet visuel
        ConfigPlayer.instance.AddVisualEffect("damage");
        
        // Met à jour l'UI
        if (ActiveEffectsUI.instance != null)
        {
            ActiveEffectsUI.instance.AddDamageEffect(damageDuration);
        }
        
        // Lance la coroutine pour retirer l'effet
        StartCoroutine(RemoveDamage(damageGiven, damageDuration));
    }

    /// <summary>
    /// Retire l'effet de dégâts après la durée
    /// </summary>
    private IEnumerator RemoveDamage(int damageGiven, float damageDuration)
    {
        yield return new WaitForSeconds(damageDuration);
        
        if (ConfigPlayer.instance != null)
        {
            ConfigPlayer.instance.damage -= damageGiven;
            ConfigPlayer.instance.RemoveVisualEffect("damage");
        }
        
        if (ActiveEffectsUI.instance != null)
        {
            ActiveEffectsUI.instance.RemoveDamageEffect();
        }
    }

    /// <summary>
    /// Ajoute l'invincibilité au joueur
    /// </summary>
    /// <param name="invincibilityDuration">Durée en secondes</param>
    public void AddInvincibility(float invincibilityDuration)
    {
        if (ConfigPlayer.instance == null)
        {
            Debug.LogWarning("PlayerEffects: ConfigPlayer.instance est null!");
            return;
        }

        // Active l'invincibilité
        ConfigPlayer.instance.SetInvincible(true);
        
        // Ajoute l'effet visuel
        ConfigPlayer.instance.AddVisualEffect("invincibility");
        
        // Met à jour l'UI
        if (ActiveEffectsUI.instance != null)
        {
            ActiveEffectsUI.instance.AddInvincibilityEffect(invincibilityDuration);
        }
        
        // Lance la coroutine pour retirer l'effet
        StartCoroutine(RemoveInvincibility(invincibilityDuration));
    }

    /// <summary>
    /// Retire l'invincibilité après la durée
    /// </summary>
    private IEnumerator RemoveInvincibility(float invincibilityDuration)
    {
        yield return new WaitForSeconds(invincibilityDuration);
        
        if (ConfigPlayer.instance != null)
        {
            ConfigPlayer.instance.SetInvincible(false);
            ConfigPlayer.instance.RemoveVisualEffect("invincibility");
        }
        
        if (ActiveEffectsUI.instance != null)
        {
            ActiveEffectsUI.instance.RemoveInvincibilityEffect();
        }
    }
}