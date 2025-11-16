using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Contrôleur principal du joueur gérant la santé, le mouvement, l'attaque et les effets visuels
/// Pattern Singleton avec DontDestroyOnLoad pour persister entre les scènes
/// </summary>
public class ConfigPlayer : MonoBehaviour
{
    [Header("=== COMPOSANTS ===")]
    [Tooltip("Animator du joueur")]
    public Animator animator;
    
    [Tooltip("SpriteRenderer du joueur")]
    public SpriteRenderer spriteRenderer;
    
    [Tooltip("Composant gérant le flash de dégâts")]
    public DamageFlashDynamic damageFlash;
    
    [Tooltip("Menu de mort")]
    public MortMenu mortMenu;
    
    [Tooltip("Canvas persistant du joueur")]
    public Canvas persistentCanvas;

    [Header("=== SANTÉ ===")]
    [Tooltip("Points de vie maximum")]
    public int maxHealth = 15;
    
    [Tooltip("Image de remplissage de la barre de vie")]
    public Image healthFill;
    
    [Tooltip("Largeur maximale de la barre de vie en pixels")]
    public float widthMax = 450f;
    
    [Tooltip("Largeur minimale de la barre de vie en pixels")]
    public float widthMin = 5f;
    
    [Tooltip("Hauteur de la barre de vie en pixels")]
    public float height = 30f;

    private int currentHealth;
    public bool isAlive = true;
    public bool isInvincible = false;

    [Header("=== COULEURS DES EFFETS ===")]
    [Tooltip("Couleur appliquée lors de l'effet de vitesse")]
    public Color speedColor = new Color(0.5f, 0f, 1f, 0.8f);
    
    [Tooltip("Couleur appliquée lors de l'effet de dégâts")]
    public Color damageColor = new Color(0f, 1f, 0f, 0.8f);
    
    [Tooltip("Couleur appliquée lors de l'effet d'invincibilité")]
    public Color invincibilityColor = new Color(0f, 0.5f, 1f, 0.8f);
    
    private Color originalColor;
    private List<string> activeEffects = new List<string>();

    [Header("=== ATTAQUE ===")]
    [Tooltip("Portée de l'attaque en unités")]
    public float attackRange = 1.5f;
    
    [Tooltip("Dégâts infligés par attaque")]
    public int damage = 1;
    
    [Tooltip("Image de remplissage de la barre d'attaque")]
    public Image attackFill;
    
    [Tooltip("Points d'attaque maximum")]
    public int maxAttackPoints = 5;
    
    [Tooltip("Largeur maximale de la barre d'attaque en pixels")]
    public float attackWidthMax = 200f;
    
    [Tooltip("Largeur minimale de la barre d'attaque en pixels")]
    public float attackWidthMin = 5f;
    
    [Tooltip("Hauteur de la barre d'attaque en pixels")]
    public float attackHeight = 20f;
    
    [Tooltip("Cooldown entre deux attaques en secondes")]
    public float attackCooldown = 0.2f;
    
    [Tooltip("Délai avant la recharge d'un point d'attaque en secondes")]
    public float rechargeDelay = 1f;

    private int currentAttackPoints;
    private bool canAttack = true;
    private Coroutine rechargeRoutine = null;

    [Header("=== MOUVEMENT ===")]
    [Tooltip("Vitesse de déplacement")]
    public float moveSpeed = 3f;
    
    [Tooltip("Rigidbody2D du joueur")]
    public Rigidbody2D rb;

    private Vector2 movement;

    [Header("=== INPUT ===")]
    [Tooltip("Touche pour attaquer")]
    public KeyCode attackKey = KeyCode.Space;

    public static ConfigPlayer instance;

    void Awake()
    {
        // Pattern Singleton avec DontDestroyOnLoad
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Rend le canvas persistant
        if (persistentCanvas != null)
        {
            DontDestroyOnLoad(persistentCanvas.gameObject);
        }

        // Initialise les valeurs
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

    /// <summary>
    /// Appelé quand une nouvelle scène est chargée
    /// </summary>
    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Recherche le menu de mort dans la nouvelle scène
        mortMenu = FindFirstObjectByType<MortMenu>();
    }

    void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    void Update()
    {
        if (!isAlive) return;

        // Récupère les inputs de mouvement
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Gère l'attaque
        if (Input.GetKeyDown(attackKey) && canAttack)
        {
            if (currentAttackPoints > 0)
            {
                PerformAttack();
                currentAttackPoints--;
                UpdateAttackUI();
                canAttack = false;
                StartCoroutine(AttackCooldownCoroutine());

                // Lance la recharge si nécessaire
                if (currentAttackPoints < maxAttackPoints && rechargeRoutine == null)
                    rechargeRoutine = StartCoroutine(RechargeCoroutine());
            }
            else
            {
                // Son d'erreur si pas de points d'attaque
                if (AudioManager.instance != null)
                    AudioManager.instance.PlayPlayerNotAttack();
            }
        }
    }

    void FixedUpdate()
    {
        if (!isAlive) return;

        // Gère le mouvement avec collision sur les murs
        Vector2 newPos = rb.position;
        float radius = 0.5f; // Valeur par défaut
        
        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
        if (circleCollider != null)
        {
            radius = circleCollider.radius;
        }

        int wallLayer = LayerMask.GetMask("IgnorePathfinding");

        // Déplacement horizontal
        Vector2 moveX = new Vector2(movement.x * moveSpeed * Time.fixedDeltaTime, 0);
        bool blockedX = Physics2D.CircleCast(rb.position, radius, moveX.normalized, moveX.magnitude, wallLayer);
        if (!blockedX && movement.x != 0)
        {
            newPos.x += moveX.x;
        }

        // Déplacement vertical
        Vector2 moveY = new Vector2(0, movement.y * moveSpeed * Time.fixedDeltaTime);
        bool blockedY = Physics2D.CircleCast(rb.position, radius, moveY.normalized, moveY.magnitude, wallLayer);
        if (!blockedY && movement.y != 0)
        {
            newPos.y += moveY.y;
        }

        rb.MovePosition(newPos);

        // Animation
        Vector2 movementApplied = new Vector2(blockedX ? 0 : movement.x, blockedY ? 0 : movement.y);
        
        if (animator != null)
        {
            animator.SetFloat("Speed", movementApplied.sqrMagnitude);
        }

        // Flip du sprite selon la direction
        if (spriteRenderer != null && movementApplied.x != 0)
        {
            spriteRenderer.flipX = movementApplied.x < 0;
        }
    }

    /// <summary>
    /// Inflige des dégâts au joueur
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (!isAlive || isInvincible) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();

        // Animation et son
        if (animator != null)
            animator.SetTrigger("Hit");
        
        if (AudioManager.instance != null)
            AudioManager.instance.PlayPlayerHit();

        // Flash de dégâts
        if (damageFlash != null)
            damageFlash.Flash(damage, maxHealth);

        // Mort
        if (currentHealth <= 0)
        {
            isAlive = false;
            
            if (AudioManager.instance != null)
                AudioManager.instance.PlayPlayerDeath();
            
            if (animator != null)
                animator.SetTrigger("Die");
            
            StartCoroutine(AttendreFinAnimationDie());
        }
    }

    /// <summary>
    /// Définit l'état d'invincibilité du joueur
    /// </summary>
    public void SetInvincible(bool invincible)
    {
        isInvincible = invincible;
    }

    /// <summary>
    /// Ajoute un effet visuel au joueur
    /// </summary>
    public void AddVisualEffect(string effectType)
    {
        if (!activeEffects.Contains(effectType))
        {
            activeEffects.Add(effectType);
            UpdateVisualEffects();
        }
    }

    /// <summary>
    /// Retire un effet visuel du joueur
    /// </summary>
    public void RemoveVisualEffect(string effectType)
    {
        if (activeEffects.Contains(effectType))
        {
            activeEffects.Remove(effectType);
            UpdateVisualEffects();
        }
    }

    /// <summary>
    /// Met à jour les effets visuels en fonction des effets actifs
    /// </summary>
    private void UpdateVisualEffects()
    {
        if (spriteRenderer == null)
            return;

        // Réinitialise la couleur si aucun effet
        if (activeEffects.Count == 0)
        {
            spriteRenderer.color = originalColor;
            return;
        }

        // Mélange les couleurs des effets actifs
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

        // Moyenne des couleurs
        if (effectCount > 0)
        {
            blendedColor.r = blendedColor.r / (effectCount + 1);
            blendedColor.g = blendedColor.g / (effectCount + 1);
            blendedColor.b = blendedColor.b / (effectCount + 1);
            blendedColor.a = Mathf.Max(speedColor.a, damageColor.a, invincibilityColor.a);
        }

        spriteRenderer.color = blendedColor;
    }

    /// <summary>
    /// Attend la fin de l'animation de mort
    /// </summary>
    private IEnumerator AttendreFinAnimationDie()
    {
        if (animator != null)
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            yield return new WaitForSeconds(state.length);
        }

        // Active le menu de mort
        if (mortMenu != null)
            mortMenu.ActiverPanelMort();

        // Cache le sprite
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;
    }

    /// <summary>
    /// Soigne le joueur
    /// </summary>
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
    }

    /// <summary>
    /// Met à jour l'UI de la barre de vie
    /// </summary>
    private void UpdateHealthUI()
    {
        if (healthFill == null) return;

        float t = currentHealth / (float)maxHealth;
        float newWidth = Mathf.Lerp(widthMin, widthMax, t);
        healthFill.rectTransform.sizeDelta = new Vector2(newWidth, height);
    }

    /// <summary>
    /// Cache le sprite du joueur
    /// </summary>
    public void DisablePlayerVisual()
    {
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;
    }

    /// <summary>
    /// Effectue une attaque sur les ennemis à portée
    /// </summary>
    void PerformAttack()
    {
        if (animator != null)
            animator.SetTrigger("Attack");
        
        if (AudioManager.instance != null)
            AudioManager.instance.PlayPlayerAttack();

        // Détermine la direction de l'attaque
        Vector2 attackDirection = Vector2.right;
        if (spriteRenderer != null)
        {
            attackDirection = spriteRenderer.flipX ? Vector2.left : Vector2.right;
        }

        // Détecte les ennemis à portée
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackRange);

        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                Vector2 directionToEnemy = (collider.transform.position - transform.position).normalized;

                // Vérifie si l'ennemi est dans la direction de l'attaque
                if (Vector2.Dot(attackDirection, directionToEnemy) > 0)
                {
                    EnemyAI enemyScript = collider.GetComponent<EnemyAI>();
                    if (enemyScript != null)
                    {
                        enemyScript.TakeDamage(damage);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Cooldown entre deux attaques
    /// </summary>
    IEnumerator AttackCooldownCoroutine()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    /// <summary>
    /// Recharge progressivement les points d'attaque
    /// </summary>
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

    /// <summary>
    /// Met à jour l'UI de la barre d'attaque
    /// </summary>
    private void UpdateAttackUI()
    {
        if (attackFill == null) return;

        float t = currentAttackPoints / (float)maxAttackPoints;
        float newWidth = Mathf.Lerp(attackWidthMin, attackWidthMax, t);
        attackFill.rectTransform.sizeDelta = new Vector2(newWidth, attackHeight);
    }

    /// <summary>
    /// Dessine la portée d'attaque dans l'éditeur
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}