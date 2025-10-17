using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement; // nécessaire pour les callbacks de scène

public class PlayerHealth : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 15;
    private int currentHealth;

    public bool isAlive = true;

    [Header("UI Santé")]
    public Image healthFill;
    public Text healthText;

    [Header("Jauge")]
    public float widthMax = 450f;
    public float widthMin = 5f;
    public float height = 30f;

    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public static PlayerHealth instance;

    public DamageFlashDynamic damageFlash;
    public MortMenu mortMenu;

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

    void OnEnable()
    {
        SceneManager.sceneLoaded += SceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneLoaded;
    }

    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Cherche le MortMenu dans la nouvelle scène
        mortMenu = FindFirstObjectByType<MortMenu>();
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
                StartCoroutine(AttendreFinAnimationDie());
            }
        }
    }

    private IEnumerator AttendreFinAnimationDie()
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(state.length);

        if (mortMenu != null)
            mortMenu.ActiverPanelMort();

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;
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
            float t = currentHealth / (float)maxHealth;
            float newWidth = Mathf.Lerp(widthMin, widthMax, t);
            healthFill.rectTransform.sizeDelta = new Vector2(newWidth, height);
        }

        if (healthText != null)
            healthText.text = currentHealth + " / " + maxHealth;
    }

    public void DisablePlayerVisual()
    {
        spriteRenderer.enabled = false;
    }
}
