using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

/// <summary>
/// Gère l'inventaire du joueur (pièces et items)
/// Pattern Singleton avec DontDestroyOnLoad
/// </summary>
public class Inventory : MonoBehaviour
{
    [Header("Pièces")]
    [Tooltip("Nombre de pièces possédées")]
    public int coinsCount;
    
    [Tooltip("Texte affichant le nombre de pièces")]
    public Text coinsCountText;

    [Header("Items")]
    [Tooltip("Liste des items possédés")]
    public List<Item> content = new List<Item>();

    [System.Serializable]
    public class ItemSlot
    {
        [Tooltip("Type d'item pour ce slot")]
        public Item itemType;
        
        [Tooltip("Bouton du slot")]
        public Button slotButton;
        
        [Tooltip("Texte affichant la quantité")]
        public Text quantityText;
        
        [Tooltip("Image de l'item")]
        public Image itemImage;
    }

    [Tooltip("Liste des slots d'inventaire UI")]
    public List<ItemSlot> itemSlots = new List<ItemSlot>();
    
    [Header("Effets")]
    [Tooltip("Composant gérant les effets des potions")]
    public PlayerEffects playerEffects;
    
    public static Inventory instance;

    private void Awake()
    {
        // Pattern Singleton avec DontDestroyOnLoad
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        SetupSlotButtons();
        UpdateInventoryUI();
    }

    /// <summary>
    /// Configure les listeners des boutons de slots
    /// </summary>
    private void SetupSlotButtons()
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            int index = i; // Capture locale pour le closure
            
            if (itemSlots[i].slotButton != null)
            {
                itemSlots[i].slotButton.onClick.AddListener(() => ConsumeItemFromSlot(index));
            }
        }
    }

    /// <summary>
    /// Consomme un item depuis un slot
    /// </summary>
    /// <param name="slotIndex">Index du slot</param>
    public void ConsumeItemFromSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= itemSlots.Count)
        {
            Debug.LogWarning($"Inventory: Index de slot invalide {slotIndex}");
            return;
        }

        ItemSlot slot = itemSlots[slotIndex];
        if (slot.itemType == null)
        {
            Debug.LogWarning("Inventory: ItemType null dans le slot");
            return;
        }

        // Trouve le premier item correspondant dans l'inventaire
        Item itemToConsume = content.FirstOrDefault(item => 
            item != null && (item == slot.itemType || item.nameItem == slot.itemType.nameItem)
        );
        
        if (itemToConsume == null)
        {
            Debug.LogWarning($"Inventory: Aucun item trouvé pour {slot.itemType.nameItem}");
            return;
        }

        // Applique les effets de l'item
        bool itemUsed = false;

        // Soin
        if (itemToConsume.hpGiven > 0 && ConfigPlayer.instance != null)
        {
            ConfigPlayer.instance.Heal(itemToConsume.hpGiven);
            
            if (AudioManager.instance != null)
                AudioManager.instance.PlayUseHealthPotion();
            
            itemUsed = true;
        }

        // Effet de vitesse
        if (itemToConsume.speedGiven != 0 && itemToConsume.speedDuration > 0f && playerEffects != null)
        {
            playerEffects.AddSpeed(itemToConsume.speedGiven, itemToConsume.speedDuration);
            
            if (AudioManager.instance != null)
                AudioManager.instance.PlayUseSpeedPotion();
            
            itemUsed = true;
        }

        // Effet de dégâts
        if (itemToConsume.damageGiven != 0 && itemToConsume.damageDuration > 0f && playerEffects != null)
        {
            playerEffects.AddDamage(itemToConsume.damageGiven, itemToConsume.damageDuration);
            
            if (AudioManager.instance != null)
                AudioManager.instance.PlayUseForcePotion();
            
            itemUsed = true;
        }

        // Effet d'invincibilité
        if (itemToConsume.givesInvincibility && itemToConsume.invincibilityDuration > 0f && playerEffects != null)
        {
            playerEffects.AddInvincibility(itemToConsume.invincibilityDuration);
            
            if (AudioManager.instance != null)
                AudioManager.instance.PlayUseShieldPotion();
            
            itemUsed = true;
        }

        // Retire l'item de l'inventaire si utilisé
        if (itemUsed)
        {
            content.Remove(itemToConsume);
            UpdateInventoryUI();
        }
        else
        {
            Debug.LogWarning($"Inventory: L'item {itemToConsume.nameItem} n'a aucun effet défini");
        }
    }

    /// <summary>
    /// Met à jour l'affichage de l'inventaire
    /// </summary>
    public void UpdateInventoryUI()
    {
        foreach (ItemSlot slot in itemSlots)
        {
            if (slot.itemType == null) continue;

            // Compte la quantité d'items de ce type
            int quantity = content.Count(item => 
                item != null && (item == slot.itemType || item.nameItem == slot.itemType.nameItem)
            );

            // Met à jour le texte de quantité
            if (slot.quantityText != null)
            {
                slot.quantityText.text = quantity.ToString();
            }

            // Met à jour l'image
            if (slot.itemImage != null && slot.itemType.image != null)
            {
                slot.itemImage.sprite = slot.itemType.image;
            }

            // Active/désactive le bouton selon la disponibilité
            if (slot.slotButton != null)
            {
                slot.slotButton.interactable = quantity > 0;
            }
        }
    }

    /// <summary>
    /// Ajoute des pièces à l'inventaire
    /// </summary>
    public void AddCoins(int count)
    {
        coinsCount += count;
        UpdateTextUI();
    }

    /// <summary>
    /// Retire des pièces de l'inventaire
    /// </summary>
    public void RemoveCoins(int count)
    {
        coinsCount -= count;
        coinsCount = Mathf.Max(0, coinsCount); // Ne peut pas être négatif
        UpdateTextUI();
    }

    /// <summary>
    /// Met à jour l'affichage du texte de pièces
    /// </summary>
    public void UpdateTextUI()
    {
        if (coinsCountText != null)
        {
            coinsCountText.text = coinsCount.ToString();
        }
    }
}