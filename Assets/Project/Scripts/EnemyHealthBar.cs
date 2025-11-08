using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("Health Bar Settings")]
    public Image fillImage;
    public Image backgroundImage;
    public Vector3 offset = new Vector3(0, 1.5f, 0);
    public bool hideWhenFull = true;
    public bool hideWhenDead = true;
    
    [Header("Size Settings")]
    public float maxWidth = 100f;
    public float minWidth = 0f;
    public float barHeight = 10f;

    [Header("Colors")]
    public Color fullHealthColor = Color.green;
    public Color mediumHealthColor = Color.yellow;
    public Color lowHealthColor = Color.red;
    public float mediumHealthThreshold = 0.5f;
    public float lowHealthThreshold = 0.25f;

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

        if (fillImage != null)
        {
            fillRect = fillImage.GetComponent<RectTransform>();
            targetWidth = maxWidth;
        }

        canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
        }
        
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = mainCamera;

        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(1, 0.15f);
        
        transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        if (hideWhenFull)
        {
            canvasGroup.alpha = 0f;
        }
    }

    void LateUpdate()
    {
        if (enemy == null) return;

        transform.position = enemy.position + offset;

        if (mainCamera != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
        }

        if (smoothTransition && fillRect != null)
        {
            float currentWidth = fillRect.sizeDelta.x;
            float newWidth = Mathf.Lerp(currentWidth, targetWidth, Time.deltaTime * smoothSpeed);
            fillRect.sizeDelta = new Vector2(newWidth, barHeight);
        }
    }

    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (fillRect == null)
        {
            return;
        }

        float healthPercent = Mathf.Clamp01((float)currentHealth / maxHealth);
        targetWidth = Mathf.Lerp(minWidth, maxWidth, healthPercent);

        if (!smoothTransition)
        {
            fillRect.sizeDelta = new Vector2(targetWidth, barHeight);
        }

        UpdateHealthColor(healthPercent);

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

    private void UpdateHealthColor(float healthPercent)
    {
        if (fillImage == null) return;

        if (healthPercent <= lowHealthThreshold)
        {
            fillImage.color = lowHealthColor;
        }
        else if (healthPercent <= mediumHealthThreshold)
        {
            float t = (healthPercent - lowHealthThreshold) / (mediumHealthThreshold - lowHealthThreshold);
            fillImage.color = Color.Lerp(lowHealthColor, mediumHealthColor, t);
        }
        else
        {
            float t = (healthPercent - mediumHealthThreshold) / (1f - mediumHealthThreshold);
            fillImage.color = Color.Lerp(mediumHealthColor, fullHealthColor, t);
        }
    }

    public void Hide()
    {
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }

    public void Show()
    {
        if (canvasGroup != null)
            canvasGroup.alpha = 1f;
    }
}