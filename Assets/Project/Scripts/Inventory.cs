using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public int coinsCount;
    public Text coinsCountText;

    // Liste principale des items (gardée pour compatibilité avec autres scripts)
    public List<Item> content = new List<Item>();

    // Système de slots pour l'UI
    [System.Serializable]
    public class ItemSlot
    {
        public Item itemType;           // Type d'item pour ce slot
        public Button slotButton;       // Bouton du slot
        public Text quantityText;       // Texte de quantité
        public Image itemImage;         // Image de l'item (optionnel)
    }

    public List<ItemSlot> itemSlots = new List<ItemSlot>();
    
    public PlayerEffects playerEffects;
    public static Inventory instance;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("Il y a plus d'une instance de Inventory dans la scène");
            return;
        }

        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    private void Start()
    {
        SetupSlotButtons();
        UpdateInventoryUI();
    }

    // Configure les boutons des slots
    private void SetupSlotButtons()
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            int index = i; // Copie locale pour la closure
            if (itemSlots[i].slotButton != null)
            {
                itemSlots[i].slotButton.onClick.AddListener(() => ConsumeItemFromSlot(index));
            }
        }
    }

    // Consomme un item depuis un slot spécifique
    public void ConsumeItemFromSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= itemSlots.Count)
            return;

        ItemSlot slot = itemSlots[slotIndex];
        if (slot.itemType == null)
            return;

        Item itemToConsume = content.FirstOrDefault(item => item == slot.itemType || item.nameItem == slot.itemType.nameItem);
        
        if (itemToConsume == null)
            return;

        // Soin
        if (itemToConsume.hpGiven > 0)
            ConfigPlayer.instance.Heal(itemToConsume.hpGiven);

        // Boost de vitesse (Violet)
        if (itemToConsume.speedGiven != 0 && itemToConsume.speedDuration > 0f)
            playerEffects.AddSpeed(itemToConsume.speedGiven, itemToConsume.speedDuration);

        // Boost de dégâts (Vert)
        if (itemToConsume.damageGiven != 0 && itemToConsume.damageDuration > 0f)
            playerEffects.AddDamage(itemToConsume.damageGiven, itemToConsume.damageDuration);

        // Invincibilité (Bleu)
        if (itemToConsume.givesInvincibility && itemToConsume.invincibilityDuration > 0f)
            playerEffects.AddInvincibility(itemToConsume.invincibilityDuration);

        content.Remove(itemToConsume);
        UpdateInventoryUI();
    }

    // Ancienne méthode maintenue pour compatibilité (utilise le premier item de content)
    public void ConsumeItem()
    {
        if(content.Count == 0)
            return;

        Item currentItem = content[0];

        // Soin
        if (currentItem.hpGiven > 0)
            ConfigPlayer.instance.Heal(currentItem.hpGiven);

        // Boost de vitesse (Violet)
        if (currentItem.speedGiven != 0 && currentItem.speedDuration > 0f)
            playerEffects.AddSpeed(currentItem.speedGiven, currentItem.speedDuration);

        // Boost de dégâts (Vert)
        if (currentItem.damageGiven != 0 && currentItem.damageDuration > 0f)
            playerEffects.AddDamage(currentItem.damageGiven, currentItem.damageDuration);

        // Invincibilité (Bleu)
        if (currentItem.givesInvincibility && currentItem.invincibilityDuration > 0f)
            playerEffects.AddInvincibility(currentItem.invincibilityDuration);

        content.Remove(currentItem);
        UpdateInventoryUI();
    }

    // Met à jour l'UI de tous les slots
    public void UpdateInventoryUI()
    {
        foreach (ItemSlot slot in itemSlots)
        {
            if (slot.itemType == null)
                continue;

            // Compte combien d'items de ce type sont dans content
            int quantity = content.Count(item => item == slot.itemType || item.nameItem == slot.itemType.nameItem);

            // Met à jour le texte de quantité
            if (slot.quantityText != null)
            {
                slot.quantityText.text = quantity.ToString();
            }

            // Met à jour l'image si elle existe
            if (slot.itemImage != null && slot.itemType.image != null)
            {
                slot.itemImage.sprite = slot.itemType.image;
            }

            // Active/désactive le bouton selon la quantité
            if (slot.slotButton != null)
            {
                slot.slotButton.interactable = quantity > 0;
            }
        }
    }

    public void AddCoins(int count)
    {
        coinsCount += count;
        UpdateTextUI();
    }

    public void RemoveCoins(int count)
    {
        coinsCount -= count;
        UpdateTextUI();
    }

    public void UpdateTextUI()
    {
        coinsCountText.text = coinsCount.ToString();
    }
}