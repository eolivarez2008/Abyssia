using UnityEngine;
using System.Collections;

public class PlayerEffects : MonoBehaviour
{
    public void AddSpeed(int speedGiven, float speedDuration)
    {
        ConfigPlayer.instance.moveSpeed += speedGiven;
        ConfigPlayer.instance.AddVisualEffect("speed");
        StartCoroutine(RemoveSpeed(speedGiven, speedDuration));
    }

    private IEnumerator RemoveSpeed(int speedGiven, float speedDuration)
    {
        yield return new WaitForSeconds(speedDuration);
        ConfigPlayer.instance.moveSpeed -= speedGiven;
        ConfigPlayer.instance.RemoveVisualEffect("speed");
    }

    public void AddDamage(int damageGiven, float damageDuration)
    {
        ConfigPlayer.instance.damage += damageGiven;
        ConfigPlayer.instance.AddVisualEffect("damage");
        StartCoroutine(RemoveDamage(damageGiven, damageDuration));
    }

    private IEnumerator RemoveDamage(int damageGiven, float damageDuration)
    {
        yield return new WaitForSeconds(damageDuration);
        ConfigPlayer.instance.damage -= damageGiven;
        ConfigPlayer.instance.RemoveVisualEffect("damage");
    }

    public void AddInvincibility(float invincibilityDuration)
    {
        ConfigPlayer.instance.SetInvincible(true);
        ConfigPlayer.instance.AddVisualEffect("invincibility");
        StartCoroutine(RemoveInvincibility(invincibilityDuration));
    }

    private IEnumerator RemoveInvincibility(float invincibilityDuration)
    {
        yield return new WaitForSeconds(invincibilityDuration);
        ConfigPlayer.instance.SetInvincible(false);
        ConfigPlayer.instance.RemoveVisualEffect("invincibility");
    }
}