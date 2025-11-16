using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gestionnaire centralisé des magasins
/// Pattern Singleton
/// </summary>
public class ShopManager : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("Texte affichant le nom du PNJ marchand")]
    public Text pnjNameText;
    
    [Tooltip("Animator contrôlant l'ouverture/fermeture")]
    public Animator animator;

    [Header("Shop Items")]
    [Tooltip("Prefab du bouton d'item à vendre")]
    public GameObject sellButtonPrefab;
    
    [Tooltip("Parent des boutons d'items")]
    public Transform sellButtonsParent;

    public static ShopManager instance;

    private bool buttonsGenerated = false;
    private System.Action onShopClosedCallback;

    private void Awake()
    {
        // Pattern Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// Ouvre le magasin avec les items spécifiés
    /// </summary>
    /// <param name="items">Items à vendre</param>
    /// <param name="pnjName">Nom du marchand</param>
    /// <param name="onClose">Callback appelé à la fermeture</param>
    public void OpenShop(Item[] items, string pnjName, System.Action onClose = null)
    {
        if (items == null || items.Length == 0)
        {
            Debug.LogWarning("ShopManager: Aucun item à vendre!");
            return;
        }

        onShopClosedCallback = onClose;

        // Configure le nom du marchand
        if (pnjNameText != null)
            pnjNameText.text = pnjName;

        // Génère les boutons si ce n'est pas déjà fait
        if (!buttonsGenerated)
        {
            GenerateButtons(items);
            buttonsGenerated = true;
        }

        // Ouvre l'animation
        if (animator != null)
            animator.SetBool("isOpen", true);

        if (AudioManager.instance != null)
            AudioManager.instance.PlayOpenDialogue();
    }

    /// <summary>
    /// Génère les boutons d'achat pour chaque item
    /// </summary>
    private void GenerateButtons(Item[] items)
    {
        if (sellButtonPrefab == null)
        {
            Debug.LogError("ShopManager: sellButtonPrefab non assigné!");
            return;
        }

        if (sellButtonsParent == null)
        {
            Debug.LogError("ShopManager: sellButtonsParent non assigné!");
            return;
        }

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                Debug.LogWarning($"ShopManager: Item {i} est null!");
                continue;
            }

            // Instancie le bouton
            GameObject button = Instantiate(sellButtonPrefab, sellButtonsParent);
            button.SetActive(true);

            // Cache le prefab si visible
            if (sellButtonPrefab.activeSelf)
                sellButtonPrefab.SetActive(false);

            // Active tous les composants UI
            foreach (var text in button.GetComponentsInChildren<Text>(true))
                text.enabled = true;

            foreach (var image in button.GetComponentsInChildren<Image>(true))
                image.enabled = true;

            // Configure le script du bouton
            SellButtonItem buttonScript = button.GetComponent<SellButtonItem>();
            if (buttonScript != null)
            {
                buttonScript.itemName.text = items[i].nameItem;
                buttonScript.itemImage.sprite = items[i].image;
                buttonScript.itemPrice.text = items[i].price.ToString();
                buttonScript.item = items[i];

                // Ajoute le listener d'achat
                Button buttonComponent = button.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    buttonComponent.onClick.AddListener(delegate { buttonScript.BuyItem(); });
                }
            }
            else
            {
                Debug.LogWarning("ShopManager: Le bouton n'a pas de SellButtonItem!");
            }
        }
    }

    /// <summary>
    /// Ferme le magasin
    /// </summary>
    public void CloseShop()
    {
        if (animator != null)
            animator.SetBool("isOpen", false);

        if (AudioManager.instance != null)
            AudioManager.instance.PlayCloseDialogue();

        // Appelle le callback de fermeture
        if (onShopClosedCallback != null)
        {
            onShopClosedCallback.Invoke();
            onShopClosedCallback = null;
        }
    }

    /// <summary>
    /// Vérifie si le magasin est ouvert
    /// </summary>
    public bool IsShopOpen()
    {
        if (animator == null) return false;
        return animator.GetBool("isOpen");
    }
}