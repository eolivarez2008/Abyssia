using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gère l'affichage et la mise à jour de la barre de vie d'un ennemi
/// </summary>
public class EnemyHealthBar : MonoBehaviour
{
    [Header("Health Bar Settings")]
    [Tooltip("Image de remplissage de la barre")]
    public Image fillImage;
    
    [Tooltip("Image de fond de la barre")]
    public Image backgroundImage;
    
    [Tooltip("Décalage par rapport à l'ennemi")]
    public Vector3 offset = new Vector3(0, 1.5f, 0);
    
    [Tooltip("Cacher la barre quand la vie est pleine")]
    public bool hideWhenFull = true;
    
    [Tooltip("Cacher la barre quand l'ennemi est mort")]
    public bool hideWhenDead = true;
    
    [Header("Size Settings")]
    [Tooltip("Largeur maximale de la barre")]
    public float maxWidth = 100f;
    
    [Tooltip("Largeur minimale de la barre")]
    public float minWidth = 0f;
    
    [Tooltip("Hauteur de la barre")]
    public float barHeight = 10f;

    [Header("Colors")]
    [Tooltip("Couleur quand la vie est pleine")]
    public Color fullHealthColor = Color.green;
    
    [Tooltip("Couleur quand la vie est moyenne")]
    public Color mediumHealthColor = Color.yellow;
    
    [Tooltip("Couleur quand la vie est basse")]
    public Color lowHealthColor = Color.red;
    
    [Range(0f, 1f)]
    [Tooltip("Seuil pour la couleur moyenne (% de vie)")]
    public float mediumHealthThreshold = 0.5f;
    
    [Range(0f, 1f)]
    [Tooltip("Seuil pour la couleur basse (% de vie)")]
    public float lowHealthThreshold = 0.25f;

    [Header("Smooth Animation")]
    [Tooltip("Transition lissée de la barre")]
    public bool smoothTransition = true;
    
    [Tooltip("Vitesse de la transition")]
    public float smoothSpeed = 5f;

    private Transform enemy;
    private Camera mainCamera;
    private Canvas canvas;
    private float targetWidth;
    private CanvasGroup canvasGroup;
    private RectTransform fillRect;

    void Start()
    {
        // Récupère le transform de l'ennemi parent
        enemy = transform.parent;
        mainCamera = Camera.main;

        if (fillImage != null)
        {
            fillRect = fillImage.GetComponent<RectTransform>();
            targetWidth = maxWidth;
        }

        // Configure le canvas
        canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
        }
        
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = mainCamera;

        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = new Vector2(1, 0.15f);
        }
        
        transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        // Configure le CanvasGroup pour gérer la visibilité
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Cache la barre au démarrage si hideWhenFull
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

        // Transition lissée de la largeur
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
    /// <param name="currentHealth">Vie actuelle</param>
    /// <param name="maxHealth">Vie maximale</param>
    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (fillRect == null || maxHealth <= 0)
        {
            Debug.LogWarning("EnemyHealthBar: fillRect null ou maxHealth invalide");
            return;
        }

        // Calcule le pourcentage de vie
        float healthPercent = Mathf.Clamp01((float)currentHealth / maxHealth);
        targetWidth = Mathf.Lerp(minWidth, maxWidth, healthPercent);

        // Met à jour immédiatement si pas de transition lissée
        if (!smoothTransition)
        {
            fillRect.sizeDelta = new Vector2(targetWidth, barHeight);
        }

        // Met à jour la couleur
        UpdateHealthColor(healthPercent);

        // Gère la visibilité
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
    /// Met à jour la couleur de la barre selon le pourcentage de vie
    /// </summary>
    private void UpdateHealthColor(float healthPercent)
    {
        if (fillImage == null) return;

        if (healthPercent <= lowHealthThreshold)
        {
            // Vie basse = rouge
            fillImage.color = lowHealthColor;
        }
        else if (healthPercent <= mediumHealthThreshold)
        {
            // Transition rouge -> jaune
            float t = (healthPercent - lowHealthThreshold) / (mediumHealthThreshold - lowHealthThreshold);
            fillImage.color = Color.Lerp(lowHealthColor, mediumHealthColor, t);
        }
        else
        {
            // Transition jaune -> vert
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