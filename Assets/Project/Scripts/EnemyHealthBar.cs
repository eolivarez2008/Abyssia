using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("Health Bar Settings")]
    public Image fillImage;           // L'image de remplissage (barre verte/rouge)
    public Image backgroundImage;     // L'image de fond (barre grise)
    public Vector3 offset = new Vector3(0, 1.5f, 0); // Offset au-dessus de l'ennemi
    public bool hideWhenFull = true;  // Cache la barre si HP = 100%
    public bool hideWhenDead = true;  // Cache la barre quand mort
    
    [Header("Size Settings")]
    public float maxWidth = 100f;     // Largeur maximum de la barre
    public float minWidth = 0f;       // Largeur minimum
    public float barHeight = 10f;     // Hauteur de la barre

    [Header("Colors")]
    public Color fullHealthColor = Color.green;
    public Color mediumHealthColor = Color.yellow;
    public Color lowHealthColor = Color.red;
    public float mediumHealthThreshold = 0.5f;  // 50%
    public float lowHealthThreshold = 0.25f;    // 25%

    [Header("Smooth Animation")]
    public bool smoothTransition = true;
    public float smoothSpeed = 5f;

    private Transform enemy;
    private Camera mainCamera;
    private Canvas canvas;
    private float targetWidth;
    private CanvasGroup canvasGroup;
    private RectTransform fillRect;

    void Start()
    {
        enemy = transform.parent;
        mainCamera = Camera.main;

        // Récupère le RectTransform de la barre de vie
        if (fillImage != null)
        {
            fillRect = fillImage.GetComponent<RectTransform>();
            targetWidth = maxWidth;
        }

        // Récupère ou crée le Canvas
        canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
        }
        
        // IMPORTANT: Configure le Canvas en World Space
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = mainCamera;

        // Configure la taille du Canvas (PETIT pour que ce soit proportionnel)
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(1, 0.15f);
        
        // Scale pour que ce soit visible
        transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        // Ajoute un CanvasGroup pour gérer l'alpha
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Cache au départ si HP = 100%
        if (hideWhenFull)
        {
            canvasGroup.alpha = 0f;
        }
    }

    void LateUpdate()
    {
        if (enemy == null) return;

        // Positionne la barre au-dessus de l'ennemi
        transform.position = enemy.position + offset;

        // Fait toujours face à la caméra
        if (mainCamera != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
        }

        // Animation smooth de la largeur
        if (smoothTransition && fillRect != null)
        {
            float currentWidth = fillRect.sizeDelta.x;
            float newWidth = Mathf.Lerp(currentWidth, targetWidth, Time.deltaTime * smoothSpeed);
            fillRect.sizeDelta = new Vector2(newWidth, barHeight);
        }
    }

    /// <summary>
    /// Met à jour la barre de vie
    /// </summary>
    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (fillRect == null)
        {
            Debug.LogWarning("EnemyHealthBar: fillImage n'est pas assigné sur " + gameObject.name);
            return;
        }

        float healthPercent = Mathf.Clamp01((float)currentHealth / maxHealth);
        targetWidth = Mathf.Lerp(minWidth, maxWidth, healthPercent);

        // Si pas de transition smooth, applique directement
        if (!smoothTransition)
        {
            fillRect.sizeDelta = new Vector2(targetWidth, barHeight);
        }

        // Change la couleur selon le pourcentage
        UpdateHealthColor(healthPercent);

        // Affiche/cache la barre
        if (canvasGroup != null)
        {
            if (hideWhenFull && healthPercent >= 1f)
            {
                canvasGroup.alpha = 0f;
            }
            else if (hideWhenDead && currentHealth <= 0)
            {
                canvasGroup.alpha = 0f;
            }
            else
            {
                canvasGroup.alpha = 1f;
            }
        }
    }

    /// <summary>
    /// Change la couleur selon le pourcentage de vie
    /// </summary>
    private void UpdateHealthColor(float healthPercent)
    {
        if (fillImage == null) return;

        if (healthPercent <= lowHealthThreshold)
        {
            fillImage.color = lowHealthColor;
        }
        else if (healthPercent <= mediumHealthThreshold)
        {
            // Interpolation entre rouge et jaune
            float t = (healthPercent - lowHealthThreshold) / (mediumHealthThreshold - lowHealthThreshold);
            fillImage.color = Color.Lerp(lowHealthColor, mediumHealthColor, t);
        }
        else
        {
            // Interpolation entre jaune et vert
            float t = (healthPercent - mediumHealthThreshold) / (1f - mediumHealthThreshold);
            fillImage.color = Color.Lerp(mediumHealthColor, fullHealthColor, t);
        }
    }

    /// <summary>
    /// Cache la barre de vie
    /// </summary>
    public void Hide()
    {
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }

    /// <summary>
    /// Affiche la barre de vie
    /// </summary>
    public void Show()
    {
        if (canvasGroup != null)
            canvasGroup.alpha = 1f;
    }
}