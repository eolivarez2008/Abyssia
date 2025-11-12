using UnityEngine;
using System.Collections;

public class PlayerEffects : MonoBehaviour
{
    public void AddSpeed(int speedGiven, float speedDuration)
    {
        ConfigPlayer.instance.moveSpeed += speedGiven;
        ConfigPlayer.instance.AddVisualEffect("speed");
        
        // Active le canvas de l'effet
        if (ActiveEffectsUI.instance != null)
        {
            ActiveEffectsUI.instance.AddSpeedEffect(speedDuration);
        }
        
        StartCoroutine(RemoveSpeed(speedGiven, speedDuration));
    }

    private IEnumerator RemoveSpeed(int speedGiven, float speedDuration)
    {
        yield return new WaitForSeconds(speedDuration);
        ConfigPlayer.instance.moveSpeed -= speedGiven;
        ConfigPlayer.instance.RemoveVisualEffect("speed");
        
        // Désactive le canvas (normalement déjà fait par le timer)
        if (ActiveEffectsUI.instance != null)
        {
            ActiveEffectsUI.instance.RemoveSpeedEffect();
        }
    }

    public void AddDamage(int damageGiven, float damageDuration)
    {
        ConfigPlayer.instance.damage += damageGiven;
        ConfigPlayer.instance.AddVisualEffect("damage");
        
        // Active le canvas de l'effet
        if (ActiveEffectsUI.instance != null)
        {
            ActiveEffectsUI.instance.AddDamageEffect(damageDuration);
        }
        
        StartCoroutine(RemoveDamage(damageGiven, damageDuration));
    }

    private IEnumerator RemoveDamage(int damageGiven, float damageDuration)
    {
        yield return new WaitForSeconds(damageDuration);
        ConfigPlayer.instance.damage -= damageGiven;
        ConfigPlayer.instance.RemoveVisualEffect("damage");
        
        // Désactive le canvas
        if (ActiveEffectsUI.instance != null)
        {
            ActiveEffectsUI.instance.RemoveDamageEffect();
        }
    }

    public void AddInvincibility(float invincibilityDuration)
    {
        ConfigPlayer.instance.SetInvincible(true);
        ConfigPlayer.instance.AddVisualEffect("invincibility");
        
        // Active le canvas de l'effet
        if (ActiveEffectsUI.instance != null)
        {
            ActiveEffectsUI.instance.AddInvincibilityEffect(invincibilityDuration);
        }
        
        StartCoroutine(RemoveInvincibility(invincibilityDuration));
    }

    private IEnumerator RemoveInvincibility(float invincibilityDuration)
    {
        yield return new WaitForSeconds(invincibilityDuration);
        ConfigPlayer.instance.SetInvincible(false);
        ConfigPlayer.instance.RemoveVisualEffect("invincibility");
        
        // Désactive le canvas
        if (ActiveEffectsUI.instance != null)
        {
            ActiveEffectsUI.instance.RemoveInvincibilityEffect();
        }
    }
}