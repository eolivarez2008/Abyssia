using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gère l'affichage des effets actifs (vitesse, dégâts, invincibilité) avec leurs timers
/// </summary>
public class ActiveEffectsUI : MonoBehaviour
{
    [Header("Speed Effect")]
    [Tooltip("Canvas contenant l'UI de l'effet de vitesse")]
    public GameObject speedCanvas;
    [Tooltip("Texte affichant le temps restant pour l'effet de vitesse")]
    public Text speedTimerText;
    private float speedRemainingTime;
    private bool speedActive = false;

    [Header("Damage Effect")]
    [Tooltip("Canvas contenant l'UI de l'effet de dégâts")]
    public GameObject damageCanvas;
    [Tooltip("Texte affichant le temps restant pour l'effet de dégâts")]
    public Text damageTimerText;
    private float damageRemainingTime;
    private bool damageActive = false;

    [Header("Invincibility Effect")]
    [Tooltip("Canvas contenant l'UI de l'effet d'invincibilité")]
    public GameObject invincibilityCanvas;
    [Tooltip("Texte affichant le temps restant pour l'effet d'invincibilité")]
    public Text invincibilityTimerText;
    private float invincibilityRemainingTime;
    private bool invincibilityActive = false;

    public static ActiveEffectsUI instance;

    private void Awake()
    {
        // Pattern Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Désactiver tous les canvas au démarrage
        if (speedCanvas != null)
            speedCanvas.SetActive(false);
        if (damageCanvas != null)
            damageCanvas.SetActive(false);
        if (invincibilityCanvas != null)
            invincibilityCanvas.SetActive(false);
    }

    private void Update()
    {
        // Mise à jour de l'effet de vitesse
        if (speedActive)
        {
            speedRemainingTime -= Time.deltaTime;
            UpdateTimerText(speedTimerText, speedRemainingTime);

            if (speedRemainingTime <= 0)
            {
                RemoveSpeedEffect();
            }
        }

        // Mise à jour de l'effet de dégâts
        if (damageActive)
        {
            damageRemainingTime -= Time.deltaTime;
            UpdateTimerText(damageTimerText, damageRemainingTime);

            if (damageRemainingTime <= 0)
            {
                RemoveDamageEffect();
            }
        }

        // Mise à jour de l'effet d'invincibilité
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
    /// Active l'affichage de l'effet de vitesse avec sa durée
    /// </summary>
    public void AddSpeedEffect(float duration)
    {
        speedRemainingTime = duration;
        speedActive = true;
        if (speedCanvas != null)
            speedCanvas.SetActive(true);
    }

    /// <summary>
    /// Désactive l'affichage de l'effet de vitesse
    /// </summary>
    public void RemoveSpeedEffect()
    {
        speedActive = false;
        if (speedCanvas != null)
            speedCanvas.SetActive(false);
    }

    /// <summary>
    /// Active l'affichage de l'effet de dégâts avec sa durée
    /// </summary>
    public void AddDamageEffect(float duration)
    {
        damageRemainingTime = duration;
        damageActive = true;
        if (damageCanvas != null)
            damageCanvas.SetActive(true);
    }

    /// <summary>
    /// Désactive l'affichage de l'effet de dégâts
    /// </summary>
    public void RemoveDamageEffect()
    {
        damageActive = false;
        if (damageCanvas != null)
            damageCanvas.SetActive(false);
    }

    /// <summary>
    /// Active l'affichage de l'effet d'invincibilité avec sa durée
    /// </summary>
    public void AddInvincibilityEffect(float duration)
    {
        invincibilityRemainingTime = duration;
        invincibilityActive = true;
        if (invincibilityCanvas != null)
            invincibilityCanvas.SetActive(true);
    }

    /// <summary>
    /// Désactive l'affichage de l'effet d'invincibilité
    /// </summary>
    public void RemoveInvincibilityEffect()
    {
        invincibilityActive = false;
        if (invincibilityCanvas != null)
            invincibilityCanvas.SetActive(false);
    }

    /// <summary>
    /// Met à jour le texte du timer avec le format mm:ss ou ss
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