using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    [Header("=== CIBLE ===")]
    public Transform target;

    [Header("=== MOUVEMENT ===")]
    public float speed = 120f;
    public float nextWpDistance = 1f;
    public float attackRange = 2f;
    public float detectionRange = 7f;
    
    // Position de départ pour le retour
    private Vector3 startPosition;
    public float returnRange = 10f; // Distance max avant de retourner au spawn
    private bool isReturningToStart = false;

    [Header("=== PATHFINDING ===")]
    public Path path;
    int currWp = 0;
    public Seeker seeker;
    public Rigidbody2D rb;
    public LayerMask obstacleMask;
    
    [Header("=== PHYSIQUE ===")]
    public float pushResistance = 100f; // Résistance au push du joueur

    [Header("=== ANIMATION ===")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    [Header("=== COMBAT ===")]
    public float attackCooldown = 2f;
    private float currentCooldown = 0f;
    public int damage = 1;
    public int maxHealth = 2;
    private int currentHealth;
    private bool isAlive = true;

    [System.Serializable]
    public class DropItem
    {
        public GameObject prefab;       // Le prefab à drop (coin, coeur, etc.)
        [Range(0f, 100f)]
        public float dropChance = 50f;  // Chance de drop en %
        public int minAmount = 1;       // Quantité minimum
        public int maxAmount = 1;       // Quantité maximum
    }
    
    [Header("=== DROPS ===")]
    public List<DropItem> possibleDrops = new List<DropItem>();
    public float dropForce = 5f; // Force d'éjection des drops

    void Awake()
    {
        currentHealth = maxHealth;
        // Sauvegarde la position de départ
        startPosition = transform.position;
    }

    void Start()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        if (target == null)
        {
            Debug.LogWarning("EnemyAI: No target assigned!");
            enabled = false;
            return;
        }

        InvokeRepeating("UpdatePath", 0, 0.5f);
    }

    void UpdatePath()
    {
        if (!isAlive || !seeker.IsDone()) return;

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
        float distToStart = Vector2.Distance(transform.position, startPosition);

        // Si trop loin du spawn, retourne à la position de départ
        if (distToStart > returnRange)
        {
            isReturningToStart = true;
            seeker.StartPath(rb.position, startPosition, OnPathComplete);
            return;
        }

        // Si proche du spawn et en mode retour, arrête le retour
        if (isReturningToStart && distToStart < 0.5f)
        {
            isReturningToStart = false;
            path = null;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Si en mode retour, continue de retourner
        if (isReturningToStart)
        {
            return;
        }

        // Comportement normal : poursuite du joueur
        if (distToPlayer <= detectionRange)
        {
            RaycastHit2D hit = Physics2D.Linecast(transform.position, target.position, obstacleMask);

            if (hit.collider == null)
            {
                seeker.StartPath(rb.position, target.position, OnPathComplete);
            }
            else
            {
                path = null;
            }
        }
        else
        {
            // Joueur hors de portée, commence à retourner
            if (distToStart > 0.5f)
            {
                isReturningToStart = true;
                seeker.StartPath(rb.position, startPosition, OnPathComplete);
            }
        }
    }

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
        if (!isAlive)
        {
            return;
        }

        animator.SetFloat("Speed", rb.linearVelocity.sqrMagnitude);

        if (rb.linearVelocity.x != 0)
        {
            spriteRenderer.flipX = rb.linearVelocity.x < 0; 
        }

        currentCooldown -= Time.deltaTime;

        if (currentCooldown < 0)
        {
            currentCooldown = 0;
        }
    }

    void FixedUpdate()
    {
        if (!isAlive || path == null || currWp >= path.vectorPath.Count)
        {
            return;
        }

        // Si en mode retour, ne pas attaquer
        if (isReturningToStart)
        {
            MoveAlongPath();
            return;
        }

        float playerDistance = Vector2.Distance(target.position, transform.position);

        if (playerDistance > attackRange)
        {
            MoveAlongPath();
        }
        else
        {
            if (currentCooldown <= 0)
            {
                Attack();
            }
        }
    }

    void MoveAlongPath()
    {
        Vector2 direction = ((Vector2)path.vectorPath[currWp] - rb.position).normalized;
        Vector2 smoothDirection = Vector2.Lerp(rb.linearVelocity.normalized, direction, 0.1f);
        Vector2 velocity = smoothDirection * speed * Time.fixedDeltaTime;

        rb.linearVelocity = velocity;

        float distance = Vector2.Distance(rb.position, path.vectorPath[currWp]);

        if (distance < nextWpDistance)
        {
            currWp++;
        }
    }

    void Attack()
    {
        animator.SetBool("isAttacking", true);
        currentCooldown = attackCooldown;
        animator.SetTrigger("Attack");
    }

    void EndOfAttack()
    {
        animator.SetBool("isAttacking", false);

        if (target != null && Vector2.Distance(transform.position, target.position) <= attackRange)
        {
            target.GetComponent<ConfigPlayer>().TakeDamage(damage);
        }
    }

    public void TakeDamage(int damage)
    {
        if (!isAlive) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("Hit");
            currentCooldown = attackCooldown;
        }
    }

    void Die()
    {
        isAlive = false;

        // Stop le mouvement
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;

        // Désactive tous les colliders
        Collider2D[] cols = GetComponents<Collider2D>();
        foreach (var c in cols)
        {
            c.enabled = false;
        }

        // Drop des objets
        DropLoot();

        // Animation de mort
        animator.SetTrigger("Die");

        // Supprime l'objet après 3 secondes
        Destroy(gameObject, 3f);
    }

    void DropLoot()
    {
        foreach (DropItem drop in possibleDrops)
        {
            if (drop.prefab == null) continue;

            // Vérifie la chance de drop
            float randomChance = Random.Range(0f, 100f);
            
            if (randomChance <= drop.dropChance)
            {
                // Détermine la quantité à drop
                int amount = Random.Range(drop.minAmount, drop.maxAmount + 1);

                // Spawn les objets
                for (int i = 0; i < amount; i++)
                {
                    // Position légèrement aléatoire autour de l'ennemi
                    Vector3 dropPosition = transform.position + new Vector3(
                        Random.Range(-0.5f, 0.5f),
                        Random.Range(-0.5f, 0.5f),
                        0
                    );

                    GameObject droppedItem = Instantiate(drop.prefab, dropPosition, Quaternion.identity);

                    // Applique une force d'éjection si l'objet a un Rigidbody2D
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

    void OnDrawGizmosSelected()
    {
        // Zone d'attaque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Zone de détection
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Zone de retour
        Gizmos.color = Color.blue;
        Vector3 spawnPos = Application.isPlaying ? startPosition : transform.position;
        Gizmos.DrawWireSphere(spawnPos, returnRange);
        
        // Ligne vers spawn
        if (Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, startPosition);
        }
    }
}