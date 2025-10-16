using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 15; // modifiable dans l'Inspector
    private int currentHealth;

    public bool isAlive = true;

    [Header("UI Santé")]
    public Image healthFill; // Image rouge de la jauge
    public Text healthText;  // optionnel : afficher "15 / 15"

    [Header("Jauge")]
    public float widthMax = 450f;
    public float widthMin = 5f;
    public float height = 30f;

    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public static PlayerHealth instance;

    public DamageFlashDynamic damageFlash;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        if (isAlive)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            UpdateHealthUI();

            if (damageFlash != null)
                damageFlash.Flash(damage, maxHealth);

            if (currentHealth <= 0)
            {
                isAlive = false;
                animator.SetTrigger("Die");
            }
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthFill != null)
        {
            float t = currentHealth / (float)maxHealth; // proportion de vie restante
            float newWidth = Mathf.Lerp(widthMin, widthMax, t);

            healthFill.rectTransform.sizeDelta = new Vector2(newWidth, height);
        }

        if (healthText != null)
        {
            healthText.text = currentHealth + " / " + maxHealth;
        }
    }

    public void DisablePlayerVisual()
    {
        spriteRenderer.enabled = false;
    }
}
