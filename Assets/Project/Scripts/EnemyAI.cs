using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

/// <summary>
/// Intelligence artificielle d'un ennemi avec pathfinding, combat et système de drop
/// </summary>
public class EnemyAI : MonoBehaviour
{
    [Header("=== CIBLE ===")]
    [Tooltip("Transform du joueur à cibler")]
    public Transform target;

    [Header("=== MOUVEMENT ===")]
    [Tooltip("Vitesse de déplacement")]
    public float speed = 120f;
    
    [Tooltip("Distance minimale au prochain waypoint")]
    public float nextWpDistance = 1f;
    
    [Tooltip("Portée d'attaque")]
    public float attackRange = 2f;
    
    [Tooltip("Portée de détection du joueur")]
    public float detectionRange = 7f;

    [Header("=== PATHFINDING ===")]
    public Path path;
    int currWp = 0;
    
    [Tooltip("Composant Seeker pour le pathfinding")]
    public Seeker seeker;
    
    [Tooltip("Rigidbody2D de l'ennemi")]
    public Rigidbody2D rb;
    
    [Tooltip("Layer des obstacles (murs)")]
    public LayerMask obstacleMask;
    
    [Header("=== PHYSIQUE ===")]
    [Tooltip("Résistance au push (non utilisé actuellement)")]
    public float pushResistance = 100f;

    [Header("=== ANIMATION ===")]
    [Tooltip("Animator de l'ennemi")]
    public Animator animator;
    
    [Tooltip("SpriteRenderer de l'ennemi")]
    public SpriteRenderer spriteRenderer;

    [Header("=== COMBAT ===")]
    [Tooltip("Temps entre deux attaques")]
    public float attackCooldown = 2f;
    
    [Tooltip("Dégâts infligés par attaque")]
    public int damage = 1;
    
    [Tooltip("Points de vie maximum")]
    public int maxHealth = 2;

    private float currentCooldown = 0f;
    private int currentHealth;
    private bool isAlive = true;

    [Header("=== HEALTH BAR ===")]
    [Tooltip("Barre de vie de l'ennemi")]
    public EnemyHealthBar healthBar;

    [Header("=== CHALLENGE SYSTEM ===")]
    [Tooltip("ID du challenge auquel appartient cet ennemi (laisser vide si pas dans un challenge)")]
    [HideInInspector]
    public string challengeID = "";

    [System.Serializable]
    public class DropItem
    {
        [Tooltip("Prefab de l'objet à faire tomber")]
        public GameObject prefab;
        
        [Range(0f, 100f)]
        [Tooltip("Chance de drop (0-100%)")]
        public float dropChance = 50f;
        
        [Tooltip("Quantité minimale")]
        public int minAmount = 1;
        
        [Tooltip("Quantité maximale")]
        public int maxAmount = 1;
    }
    
    [Header("=== DROPS ===")]
    [Tooltip("Liste des objets pouvant être droppés")]
    public List<DropItem> possibleDrops = new List<DropItem>();
    
    [Tooltip("Force d'éjection des objets droppés")]
    public float dropForce = 5f;

    [Header("=== PATHFINDING UPDATE ===")]
    [Tooltip("Intervalle de mise à jour du pathfinding en secondes")]
    public float pathUpdateInterval = 0.5f;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    void Start()
    {
        // Met à jour la barre de vie
        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
        }

        // Trouve le joueur
        if (target == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                target = playerObject.transform;
            }
            else
            {
                Debug.LogWarning($"EnemyAI: Aucun joueur trouvé pour {gameObject.name}");
                enabled = false;
                return;
            }
        }

        // Lance la mise à jour du pathfinding
        InvokeRepeating("UpdatePath", 0, pathUpdateInterval);
    }

    /// <summary>
    /// Met à jour le chemin vers le joueur si dans la portée de détection
    /// </summary>
    void UpdatePath()
    {
        if (!isAlive || seeker == null || !seeker.IsDone()) return;

        // Vérifie si le joueur existe toujours
        if (target == null) 
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
            }
            else
            {
                return;
            }
        }

        float distToPlayer = Vector2.Distance(transform.position, target.position);

        // Si le joueur est dans la portée de détection
        if (distToPlayer <= detectionRange)
        {
            // Vérifie s'il y a un mur entre l'ennemi et le joueur
            RaycastHit2D hit = Physics2D.Linecast(transform.position, target.position, obstacleMask);

            if (hit.collider == null)
            {
                // Pas de mur, calcule le chemin
                seeker.StartPath(rb.position, target.position, OnPathComplete);
            }
            else
            {
                // Mur détecté, arrête le mouvement
                path = null;
            }
        }
        else
        {
            // Joueur hors de portée, arrête le mouvement
            path = null;
        }
    }

    /// <summary>
    /// Callback appelé quand le pathfinding a calculé un chemin
    /// </summary>
    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currWp = 0;
        }
    }

    void Update()
    {
        if (!isAlive) return;

        // Met à jour l'animation
        if (animator != null && rb != null)
        {
            // Compatible Unity 6+ (linearVelocity) et Unity 2020+ (velocity)
            #if UNITY_6000_0_OR_NEWER
                animator.SetFloat("Speed", rb.linearVelocity.sqrMagnitude);
            #else
                animator.SetFloat("Speed", rb.velocity.sqrMagnitude);
            #endif
        }

        // Flip du sprite selon la direction
        if (spriteRenderer != null && rb != null)
        {
            #if UNITY_6000_0_OR_NEWER
                float velocityX = rb.linearVelocity.x;
            #else
                float velocityX = rb.velocity.x;
            #endif

            if (velocityX > 0.1f)
            {
                spriteRenderer.flipX = false;
            }
            else if (velocityX < -0.1f)
            {
                spriteRenderer.flipX = true;
            }
        }

        // Réduit le cooldown
        if (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        if (!isAlive || path == null || currWp >= path.vectorPath.Count || target == null)
        {
            return;
        }

        float playerDistance = Vector2.Distance(target.position, transform.position);

        // Si le joueur est hors de portée d'attaque, se déplacer
        if (playerDistance > attackRange)
        {
            MoveAlongPath();
        }
        // Sinon, attaquer si le cooldown est terminé
        else
        {
            if (currentCooldown <= 0)
            {
                Attack();
            }
        }
    }

    /// <summary>
    /// Déplace l'ennemi le long du chemin calculé
    /// </summary>
    void MoveAlongPath()
    {
        if (rb == null || path == null) return;

        Vector2 direction = ((Vector2)path.vectorPath[currWp] - rb.position).normalized;
        
        // Compatible Unity 6+ et Unity 2020+
        #if UNITY_6000_0_OR_NEWER
            Vector2 smoothDirection = Vector2.Lerp(rb.linearVelocity.normalized, direction, 0.1f);
        #else
            Vector2 smoothDirection = Vector2.Lerp(rb.velocity.normalized, direction, 0.1f);
        #endif
        
        Vector2 velocity = smoothDirection * speed * Time.fixedDeltaTime;

        #if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = velocity;
        #else
            rb.velocity = velocity;
        #endif

        // Passe au waypoint suivant si assez proche
        float distance = Vector2.Distance(rb.position, path.vectorPath[currWp]);
        if (distance < nextWpDistance)
        {
            currWp++;
        }
    }

    /// <summary>
    /// Lance une attaque
    /// </summary>
    void Attack()
    {        
        if (animator != null)
        {
            animator.SetBool("isAttacking", true);
            animator.SetTrigger("Attack");
        }
        
        currentCooldown = attackCooldown;
    }

    /// <summary>
    /// Appelé par un Animation Event à la fin de l'animation d'attaque
    /// </summary>
    void EndOfAttack()
    {        
        if (animator != null)
            animator.SetBool("isAttacking", false);

        if (AudioManager.instance != null)
            AudioManager.instance.PlayEnemyAttack();

        // Inflige des dégâts au joueur s'il est toujours à portée
        if (target != null && Vector2.Distance(transform.position, target.position) <= attackRange)
        {
            ConfigPlayer player = target.GetComponent<ConfigPlayer>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
    }

    /// <summary>
    /// Inflige des dégâts à l'ennemi
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (!isAlive) return;

        currentHealth -= damage;

        if (AudioManager.instance != null)
            AudioManager.instance.PlayEnemyHit();

        // Met à jour la barre de vie
        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
        }

        // Mort
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Animation de hit
            if (animator != null)
                animator.SetTrigger("Hit");
            
            // Réinitialise le cooldown d'attaque
            currentCooldown = attackCooldown;
        }
    }

    /// <summary>
    /// Tue l'ennemi
    /// </summary>
    void Die()
    {
        isAlive = false;
        
        if (AudioManager.instance != null)
            AudioManager.instance.PlayEnemyDeath();

        // Arrête le mouvement
        if (rb != null)
        {
            #if UNITY_6000_0_OR_NEWER
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            #else
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
            #endif
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // Désactive les colliders
        Collider2D[] cols = GetComponents<Collider2D>();
        foreach (var c in cols)
        {
            if (c != null)
                c.enabled = false;
        }

        // Cache la barre de vie
        if (healthBar != null)
        {
            healthBar.Hide();
        }

        // Notifie le ChallengeManager si c'est un ennemi de challenge
        if (!string.IsNullOrEmpty(challengeID) && ChallengeManager.instance != null)
        {
            ChallengeManager.instance.OnChallengeEnemyKilled(challengeID);
        }

        // Drop le loot
        DropLoot();

        // Animation de mort
        if (animator != null)
            animator.SetTrigger("Die");

        // Détruit l'objet après 3 secondes
        Destroy(gameObject, 3f);
    }

    /// <summary>
    /// Fait tomber les objets aléatoirement selon les chances configurées
    /// </summary>
    void DropLoot()
    {
        if (possibleDrops == null || possibleDrops.Count == 0) return;

        foreach (DropItem drop in possibleDrops)
        {
            if (drop.prefab == null) continue;

            float randomChance = Random.Range(0f, 100f);
            
            // Vérifie si l'objet doit être droppé
            if (randomChance <= drop.dropChance)
            {
                int amount = Random.Range(drop.minAmount, drop.maxAmount + 1);

                // Fait apparaître plusieurs exemplaires si nécessaire
                for (int i = 0; i < amount; i++)
                {
                    Vector3 dropPosition = transform.position + new Vector3(
                        Random.Range(-0.5f, 0.5f),
                        Random.Range(-0.5f, 0.5f),
                        0
                    );

                    GameObject droppedItem = Instantiate(drop.prefab, dropPosition, Quaternion.identity);

                    // Applique une force pour éjecter l'objet
                    Rigidbody2D dropRb = droppedItem.GetComponent<Rigidbody2D>();
                    if (dropRb != null)
                    {
                        Vector2 randomDirection = Random.insideUnitCircle.normalized;
                        dropRb.AddForce(randomDirection * dropForce, ForceMode2D.Impulse);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Affiche les portées dans l'éditeur
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // Portée d'attaque en rouge
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Portée de détection en jaune
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}