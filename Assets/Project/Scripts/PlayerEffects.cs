using UnityEngine;
using System.Collections;

public class PlayerEffects : MonoBehaviour
{
    public void AddSpeed(int speedGiven, float speedDuration)
    {
        ConfigPlayer.instance.moveSpeed += speedGiven;
        StartCoroutine(RemoveSpeed(speedGiven, speedDuration));
    }

    private IEnumerator RemoveSpeed(int speedGiven, float speedDuration)
    {
        yield return new WaitForSeconds(speedDuration);
        ConfigPlayer.instance.moveSpeed -= speedGiven;
    }

    public void AddDamage(int damageGiven, float damageDuration)
    {
        ConfigPlayer.instance.damage += damageGiven;
        StartCoroutine(RemoveDamage(damageGiven, damageDuration));
    }

    private IEnumerator RemoveDamage(int damageGiven, float damageDuration)
    {
        yield return new WaitForSeconds(damageDuration);
        ConfigPlayer.instance.damage -= damageGiven;
    }
}
