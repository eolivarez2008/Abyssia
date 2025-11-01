using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    // Cible que l'ennemi doit suivre (par exemple, le joueur)
    public Transform target;

    // Vitesse de déplacement de l'ennemi
    public float speed = 120f;

    // Distance à laquelle l'ennemi considère qu'il a atteint un waypoint
    public float nextWpDistance = 1f;

    // Distance minimale pour déclencher une attaque (l'ennemi s'arrête en dehors de cette portée)
    public float attackRange = 2f;

    public float detectionRange = 7f;

    // Chemin calculé par le Seeker
    public Path path;

    // Indice du waypoint actuel que l'ennemi essaie d'atteindre
    int currWp = 0;

    // Composant Seeker utilisé pour calculer le chemin
    public Seeker seeker;

    // Composant Rigidbody2D utilisé pour le mouvement physique de l'ennemi
    public Rigidbody2D rb;

    public Animator animator;

    public SpriteRenderer spriteRenderer;

    public float attackCooldown = 2f; // Délai entre les attaques

    private float currentCooldown = 0f;

    public int damage = 1;

    public int maxHealth = 2;

    private int currentHealth;

    private bool isAlive = true;

    public LayerMask obstacleMask;

    void Awake()
    {
        currentHealth = maxHealth;
    }

        // Méthode appelée au début de l'exécution
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


    // Méthode pour mettre à jour le chemin vers la cible
void UpdatePath()
{
    if (!isAlive || !seeker.IsDone()) return;

    if (target == null) 
    {
        // Essaye de retrouver le joueur s’il existe
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
        }
        else
        {
            return; // pas de joueur, on ne fait rien
        }
    }

    float dist = Vector2.Distance(transform.position, target.position);

    if (dist <= detectionRange)
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
}


    // Méthode appelée lorsque le Seeker a terminé de calculer un chemin
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

        // Met à jour l'animation en fonction de la vitesse actuelle
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
        // Si aucun chemin n'a été calculé ou si tous les waypoints ont été atteints
        if (!isAlive || path == null || currWp >= path.vectorPath.Count)
        {
            return;
        }

        // Calcule la distance entre l'ennemi et le joueur
        float playerDistance = Vector2.Distance(target.position, transform.position);

        // Si le joueur est en dehors de la portée d'attaque → poursuite
        if (playerDistance > attackRange)
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
        else // Sinon → attaque
        {
            if (currentCooldown <= 0)
            {
                Attack();
            }
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

        if (Vector2.Distance(transform.position, target.position) <= attackRange)
        {
            target.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }

    public void TakeDamage(int damage)
    {
        if (!isAlive) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            isAlive = false;

            // Stop le mouvement
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;

            // Fixe complètement le Rigidbody
            rb.bodyType = RigidbodyType2D.Kinematic;

            // Désactive tous les colliders pour rendre intangible
            Collider2D[] cols = GetComponents<Collider2D>();
            foreach (var c in cols)
            {
                c.enabled = false;
            }

            // Joue l'animation de mort
            animator.SetTrigger("Die");

            // Supprime l'objet après 3 secondes
            Destroy(gameObject, 3f);
        }
        else
        {
            // Si l'ennemi n'est pas mort, joue l'animation de hit
            animator.SetTrigger("Hit");

            // Reset cooldown pour pause avant prochaine attaque
            currentCooldown = attackCooldown;
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
