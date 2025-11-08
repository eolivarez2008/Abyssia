using UnityEngine;
using UnityEngine.UI;

public class ActiveEffectsUI : MonoBehaviour
{
    [Header("Speed Effect")]
    public GameObject speedCanvas;
    public Text speedTimerText;
    private float speedRemainingTime;
    private bool speedActive = false;

    [Header("Damage Effect")]
    public GameObject damageCanvas;
    public Text damageTimerText;
    private float damageRemainingTime;
    private bool damageActive = false;

    [Header("Invincibility Effect")]
    public GameObject invincibilityCanvas;
    public Text invincibilityTimerText;
    private float invincibilityRemainingTime;
    private bool invincibilityActive = false;

    public static ActiveEffectsUI instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Désactive tous les canvas au départ
        if (speedCanvas != null)
            speedCanvas.SetActive(false);
        if (damageCanvas != null)
            damageCanvas.SetActive(false);
        if (invincibilityCanvas != null)
            invincibilityCanvas.SetActive(false);
    }

    private void Update()
    {
        // Met à jour Speed
        if (speedActive)
        {
            speedRemainingTime -= Time.deltaTime;
            UpdateTimerText(speedTimerText, speedRemainingTime);

            if (speedRemainingTime <= 0)
            {
                RemoveSpeedEffect();
            }
        }

        // Met à jour Damage
        if (damageActive)
        {
            damageRemainingTime -= Time.deltaTime;
            UpdateTimerText(damageTimerText, damageRemainingTime);

            if (damageRemainingTime <= 0)
            {
                RemoveDamageEffect();
            }
        }

        // Met à jour Invincibility
        if (invincibilityActive)
        {
            invincibilityRemainingTime -= Time.deltaTime;
            UpdateTimerText(invincibilityTimerText, invincibilityRemainingTime);

            if (invincibilityRemainingTime <= 0)
            {
                RemoveInvincibilityEffect();
            }
        }
    }

    /// <summary>
    /// Active l'effet de vitesse
    /// </summary>
    public void AddSpeedEffect(float duration)
    {
        speedRemainingTime = duration;
        speedActive = true;
        if (speedCanvas != null)
            speedCanvas.SetActive(true);
    }

    /// <summary>
    /// Désactive l'effet de vitesse
    /// </summary>
    public void RemoveSpeedEffect()
    {
        speedActive = false;
        if (speedCanvas != null)
            speedCanvas.SetActive(false);
    }

    /// <summary>
    /// Active l'effet de dégâts
    /// </summary>
    public void AddDamageEffect(float duration)
    {
        damageRemainingTime = duration;
        damageActive = true;
        if (damageCanvas != null)
            damageCanvas.SetActive(true);
    }

    /// <summary>
    /// Désactive l'effet de dégâts
    /// </summary>
    public void RemoveDamageEffect()
    {
        damageActive = false;
        if (damageCanvas != null)
            damageCanvas.SetActive(false);
    }

    /// <summary>
    /// Active l'effet d'invincibilité
    /// </summary>
    public void AddInvincibilityEffect(float duration)
    {
        invincibilityRemainingTime = duration;
        invincibilityActive = true;
        if (invincibilityCanvas != null)
            invincibilityCanvas.SetActive(true);
    }

    /// <summary>
    /// Désactive l'effet d'invincibilité
    /// </summary>
    public void RemoveInvincibilityEffect()
    {
        invincibilityActive = false;
        if (invincibilityCanvas != null)
            invincibilityCanvas.SetActive(false);
    }

    /// <summary>
    /// Met à jour le texte du timer
    /// </summary>
    private void UpdateTimerText(Text timerText, float remainingTime)
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);

        if (minutes > 0)
            timerText.text = $"{minutes}:{seconds:00}";
        else
            timerText.text = $"{seconds}s";
    }
}