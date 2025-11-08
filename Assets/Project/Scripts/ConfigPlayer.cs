using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class ConfigPlayer : MonoBehaviour
{
    [Header("=== COMPOSANTS ===")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public DamageFlashDynamic damageFlash;
    public MortMenu mortMenu;
    public Canvas persistentCanvas;

    [Header("=== SANTÉ ===")]
    public int maxHealth = 15;
    private int currentHealth;
    public bool isAlive = true;
    public Image healthFill;
    public float widthMax = 450f;
    public float widthMin = 5f;
    public float height = 30f;
    public bool isInvincible = false;
    private Color originalColor;
    
    public Color speedColor = new Color(0.5f, 0f, 1f, 0.8f);      // Violet
    public Color damageColor = new Color(0f, 1f, 0f, 0.8f);       // Vert
    public Color invincibilityColor = new Color(0f, 0.5f, 1f, 0.8f); // Bleu
    
    private System.Collections.Generic.List<string> activeEffects = new System.Collections.Generic.List<string>();


    [Header("=== ATTAQUE ===")]
    public float attackRange = 1.5f;
    public int damage = 1;
    public int knockbackForce = 20;
    public Image attackFill;
    public int maxAttackPoints = 5;
    private int currentAttackPoints;
    public float attackWidthMax = 200f;
    public float attackWidthMin = 5f;
    public float attackHeight = 20f;
    public float attackCooldown = 0.2f;
    public float rechargeDelay = 1f;

    [Header("=== MOUVEMENT ===")]
    public float moveSpeed = 3f;
    public Rigidbody2D rb;
    private Vector2 movement;

    private bool canAttack = true;
    private Coroutine rechargeRoutine = null;

    public static ConfigPlayer instance;

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

        if (persistentCanvas != null)
        {
            DontDestroyOnLoad(persistentCanvas.gameObject);
        }

        currentHealth = maxHealth;
        currentAttackPoints = maxAttackPoints;

        UpdateHealthUI();
        UpdateAttackUI();
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
        mortMenu = FindFirstObjectByType<MortMenu>();
    }

    void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    Vector2 movementApplied;

    void Update()
    {
        if (!isAlive) return;

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && canAttack && currentAttackPoints > 0)
        {
            PerformAttack();
            currentAttackPoints--;
            UpdateAttackUI();
            canAttack = false;
            StartCoroutine(AttackCooldownCoroutine());

            if (currentAttackPoints < maxAttackPoints && rechargeRoutine == null)
                rechargeRoutine = StartCoroutine(RechargeCoroutine());
        }
    }

    void FixedUpdate()
    {
        Vector2 newPos = rb.position;
        float radius = rb.GetComponent<CircleCollider2D>().radius;
        int wallLayer = LayerMask.GetMask("IgnorePathfinding");

        Vector2 moveX = new Vector2(movement.x * moveSpeed * Time.fixedDeltaTime, 0);
        bool blockedX = Physics2D.CircleCast(rb.position, radius, moveX.normalized, moveX.magnitude, wallLayer);
        if (!blockedX) newPos.x += moveX.x;

        Vector2 moveY = new Vector2(0, movement.y * moveSpeed * Time.fixedDeltaTime);
        bool blockedY = Physics2D.CircleCast(rb.position, radius, moveY.normalized, moveY.magnitude, wallLayer);
        if (!blockedY) newPos.y += moveY.y;

        rb.MovePosition(newPos);

        movementApplied = new Vector2(blockedX ? 0 : movement.x, blockedY ? 0 : movement.y);
        animator.SetFloat("Speed", movementApplied.sqrMagnitude);

        if (movementApplied.x != 0)
            spriteRenderer.flipX = movementApplied.x < 0;
    }

    public void TakeDamage(int damage)
    {
        if (isAlive && !isInvincible)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            UpdateHealthUI();

            animator.SetTrigger("Hit");

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

    public void SetInvincible(bool invincible)
    {
        isInvincible = invincible;
    }

    public void AddVisualEffect(string effectType)
    {
        if (!activeEffects.Contains(effectType))
        {
            activeEffects.Add(effectType);
            UpdateVisualEffects();
        }
    }

    public void RemoveVisualEffect(string effectType)
    {
        if (activeEffects.Contains(effectType))
        {
            activeEffects.Remove(effectType);
            UpdateVisualEffects();
        }
    }

    private void UpdateVisualEffects()
    {
        if (spriteRenderer == null)
            return;

        if (activeEffects.Count == 0)
        {
            spriteRenderer.color = originalColor;
            return;
        }

        Color blendedColor = originalColor;
        int effectCount = 0;

        if (activeEffects.Contains("speed"))
        {
            blendedColor += speedColor;
            effectCount++;
        }

        if (activeEffects.Contains("damage"))
        {
            blendedColor += damageColor;
            effectCount++;
        }

        if (activeEffects.Contains("invincibility"))
        {
            blendedColor += invincibilityColor;
            effectCount++;
        }

        if (effectCount > 0)
        {
            blendedColor.r = blendedColor.r / (effectCount + 1);
            blendedColor.g = blendedColor.g / (effectCount + 1);
            blendedColor.b = blendedColor.b / (effectCount + 1);
            blendedColor.a = Mathf.Max(speedColor.a, damageColor.a, invincibilityColor.a);
        }

        spriteRenderer.color = blendedColor;
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
    }

    public void DisablePlayerVisual()
    {
        spriteRenderer.enabled = false;
    }

    void PerformAttack()
    {
        animator.SetTrigger("Attack");

        Vector2 attackDirection = spriteRenderer.flipX ? Vector2.left : Vector2.right;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackRange);

        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                Vector2 directionToEnemy = (collider.transform.position - transform.position).normalized;

                if (Vector2.Dot(attackDirection, directionToEnemy) > 0)
                {
                    EnemyAI enemyScript = collider.GetComponent<EnemyAI>();
                    if (enemyScript != null)
                    {
                        enemyScript.TakeDamage(damage);
                        Vector2 knockbackDirection = (collider.transform.position - transform.position).normalized;
                        enemyScript.rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
                    }
                }
            }
        }
    }

    IEnumerator AttackCooldownCoroutine()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    IEnumerator RechargeCoroutine()
    {
        while (currentAttackPoints < maxAttackPoints)
        {
            yield return new WaitForSeconds(rechargeDelay);
            currentAttackPoints++;
            UpdateAttackUI();
        }
        rechargeRoutine = null;
    }

    private void UpdateAttackUI()
    {
        if (attackFill != null)
        {
            float t = currentAttackPoints / (float)maxAttackPoints;
            float newWidth = Mathf.Lerp(attackWidthMin, attackWidthMax, t);
            attackFill.rectTransform.sizeDelta = new Vector2(newWidth, attackHeight);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
