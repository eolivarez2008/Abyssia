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

    [Header("=== SANTÉ ===")]
    public int maxHealth = 15;
    private int currentHealth;
    public bool isAlive = true;
    public Image healthFill;
    public float widthMax = 450f;
    public float widthMin = 5f;
    public float height = 30f;

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
    }

    void Update()
    {
        if (isAlive)
        {
            // --- MOUVEMENT ---
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
            movement = movement.normalized;

            animator.SetFloat("Speed", movement.sqrMagnitude);

            if (movement.x != 0)
                spriteRenderer.flipX = movement.x < 0;

            // --- ATTAQUE ---
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
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movement * moveSpeed;
    }

    // === SANTÉ ===
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
    }

    public void DisablePlayerVisual()
    {
        spriteRenderer.enabled = false;
    }

    // === ATTAQUE ===
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
