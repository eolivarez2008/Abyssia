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

    [Header("=== PATHFINDING ===")]
    public Path path;
    int currWp = 0;
    public Seeker seeker;
    public Rigidbody2D rb;
    public LayerMask obstacleMask;
    
    [Header("=== PHYSIQUE ===")]
    public float pushResistance = 100f;

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
    private bool isAttacking = false;

    [Header("=== HEALTH BAR ===")]
    public EnemyHealthBar healthBar;

    [HideInInspector]
    public string challengeID = "";

    [System.Serializable]
    public class DropItem
    {
        public GameObject prefab;
        [Range(0f, 100f)]
        public float dropChance = 50f;
        public int minAmount = 1;
        public int maxAmount = 1;
    }
    
    [Header("=== DROPS ===")]
    public List<DropItem> possibleDrops = new List<DropItem>();
    public float dropForce = 5f;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    void Start()
    {
        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
        }

        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        if (target == null)
        {
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
            path = null;
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

        if (rb.linearVelocity.x > 0.1f)
        {
            spriteRenderer.flipX = false;
        }
        else if (rb.linearVelocity.x < -0.1f)
        {
            spriteRenderer.flipX = true;
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
        isAttacking = true;
        animator.SetBool("isAttacking", true);
        currentCooldown = attackCooldown;
        animator.SetTrigger("Attack");
    }

    void EndOfAttack()
    {
        isAttacking = false;
        animator.SetBool("isAttacking", false);

        if (target != null && Vector2.Distance(transform.position, target.position) <= attackRange)
        {
            target.GetComponent<ConfigPlayer>().TakeDamage(damage);
        }
    }

    public void InterruptAttack()
    {
        if (isAttacking)
        {
            isAttacking = false;
            animator.SetBool("isAttacking", false);
            animator.ResetTrigger("Attack");
            currentCooldown = attackCooldown;
        }
    }

    public void TakeDamage(int damage)
    {
        if (!isAlive) return;

        currentHealth -= damage;

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
        }

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

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;

        Collider2D[] cols = GetComponents<Collider2D>();
        foreach (var c in cols)
        {
            c.enabled = false;
        }

        if (healthBar != null)
        {
            healthBar.Hide();
        }

        if (!string.IsNullOrEmpty(challengeID) && ChallengeManager.instance != null)
        {
            ChallengeManager.instance.OnChallengeEnemyKilled(challengeID);
        }

        DropLoot();

        animator.SetTrigger("Die");

        Destroy(gameObject, 3f);
    }

    void DropLoot()
    {
        foreach (DropItem drop in possibleDrops)
        {
            if (drop.prefab == null) continue;

            float randomChance = Random.Range(0f, 100f);
            
            if (randomChance <= drop.dropChance)
            {
                int amount = Random.Range(drop.minAmount, drop.maxAmount + 1);

                for (int i = 0; i < amount; i++)
                {
                    Vector3 dropPosition = transform.position + new Vector3(
                        Random.Range(-0.5f, 0.5f),
                        Random.Range(-0.5f, 0.5f),
                        0
                    );

                    GameObject droppedItem = Instantiate(drop.prefab, dropPosition, Quaternion.identity);

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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}