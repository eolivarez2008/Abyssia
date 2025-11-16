using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public int coinsCount;
    public Text coinsCountText;

    public List<Item> content = new List<Item>();

    [System.Serializable]
    public class ItemSlot
    {
        public Item itemType;
        public Button slotButton;
        public Text quantityText;
        public Image itemImage;
    }

    public List<ItemSlot> itemSlots = new List<ItemSlot>();
    
    public PlayerEffects playerEffects;
    public static Inventory instance;

    private void Awake()
    {
        if(instance != null)
        {
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

    private void SetupSlotButtons()
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            int index = i;
            if (itemSlots[i].slotButton != null)
            {
                itemSlots[i].slotButton.onClick.AddListener(() => ConsumeItemFromSlot(index));
            }
        }
    }

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

        if (itemToConsume.hpGiven > 0)
        {
            ConfigPlayer.instance.Heal(itemToConsume.hpGiven);
            AudioManager.instance.PlayUseHealthPotion();
        }

        if (itemToConsume.speedGiven != 0 && itemToConsume.speedDuration > 0f)
        {
            playerEffects.AddSpeed(itemToConsume.speedGiven, itemToConsume.speedDuration);
            AudioManager.instance.PlayUseSpeedPotion();
        }

        if (itemToConsume.damageGiven != 0 && itemToConsume.damageDuration > 0f)
        {
            playerEffects.AddDamage(itemToConsume.damageGiven, itemToConsume.damageDuration);
            AudioManager.instance.PlayUseForcePotion();
        }

        if (itemToConsume.givesInvincibility && itemToConsume.invincibilityDuration > 0f)
        {
            playerEffects.AddInvincibility(itemToConsume.invincibilityDuration);
            AudioManager.instance.PlayUseShieldPotion();
        }

        content.Remove(itemToConsume);
        UpdateInventoryUI();
    }

    public void UpdateInventoryUI()
    {
        foreach (ItemSlot slot in itemSlots)
        {
            if (slot.itemType == null)
                continue;

            int quantity = content.Count(item => item == slot.itemType || item.nameItem == slot.itemType.nameItem);

            if (slot.quantityText != null)
            {
                slot.quantityText.text = quantity.ToString();
            }

            if (slot.itemImage != null && slot.itemType.image != null)
            {
                slot.itemImage.sprite = slot.itemType.image;
            }

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