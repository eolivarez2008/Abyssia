using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public  Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    private Vector2 movement;
    public PlayerHealth playerHealth;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(playerHealth.isAlive)
        {
            // Récupère les inputs clavier 
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            // Normalise pour éviter d'aller plus vite en diagonale
            movement = movement.normalized;

            animator.SetFloat("Speed", movement.sqrMagnitude);

            if(movement.x !=0)
            {
                spriteRenderer.flipX = movement.x < 0;
            }
        }
    }

    void FixedUpdate()
    {
        // Déplace le joueur avec une vitesse constante
        rb.linearVelocity = movement * moveSpeed;
    }
}
