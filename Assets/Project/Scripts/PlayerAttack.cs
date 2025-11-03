using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attaque")]
    public float attackRange = 1.5f;
    public int damage = 1;
    public int knockbackForce = 20;

    [Header("UI / Attaque")]
    public Image attackFill;          
    public int maxAttackPoints = 5;   
    private int currentAttackPoints;
    public float attackWidthMax = 200f;
    public float attackWidthMin = 5f;
    public float attackHeight = 20f;

    [Header("Recharge")]
    public float attackCooldown = 0.2f; 
    public float rechargeDelay = 1f;    

    [Header("Composants")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public PlayerHealth playerHealth;

    private bool canAttack = true;
    private Coroutine rechargeRoutine = null; // <--- on garde la référence de la coroutine

    void Start()
    {
        currentAttackPoints = maxAttackPoints;
        UpdateAttackUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && playerHealth.isAlive && canAttack && currentAttackPoints > 0)
        {
            PerformAttack();

            currentAttackPoints--;
            UpdateAttackUI();

            canAttack = false;
            StartCoroutine(AttackCooldownCoroutine());

            // Lance la recharge uniquement si elle n'est pas déjà en cours et que la jauge n'est pas pleine
            if (currentAttackPoints < maxAttackPoints && rechargeRoutine == null)
            {
                rechargeRoutine = StartCoroutine(RechargeCoroutine());
            }
        }
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
        rechargeRoutine = null; // <--- la coroutine se termine et la référence est libérée
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
