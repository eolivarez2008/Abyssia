using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    public bool isAlive = true;

    public Transform healthbarUI; // UI pour afficher la santé du joueur
    public GameObject hpPrefab;

    public Animator animator;

    public SpriteRenderer spriteRenderer;

    public static PlayerHealth instance;

    public DamageFlashDynamic damageFlash;

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
        UpdateHealthbarUI();
    }

    public void TakeDamage(int damage)
    {
        if(isAlive)
        {
            currentHealth -= damage;
            UpdateHealthbarUI();

            // Flash dynamique selon les dégâts
            if(damageFlash != null)
                damageFlash.Flash(damage, maxHealth);

            if(currentHealth <= 0)
            {
                isAlive = false;
                animator.SetTrigger("Die");
            }
        }
    }

    public void UpdateHealthbarUI()
    {
        if (healthbarUI == null)
        {
            // Rechercher automatiquement le HealthBar de la scène
            GameObject ui = GameObject.Find("HealthBar"); // ou "HealthBarParent" selon ton nom
            if (ui != null)
            {
                healthbarUI = ui.transform;
            }
            else
            {
                return; // pas d'UI, on ne fait rien
            }
        }

        // Supprimer les anciens coeurs
        foreach (Transform child in healthbarUI)
            Destroy(child.gameObject);

        // Ajouter les coeurs selon la vie
        for (int i = 0; i < currentHealth; i++)
            Instantiate(hpPrefab, healthbarUI);
    }


    public void DisablePlayerVisual()
    {
        spriteRenderer.enabled = false;
    }
}
