using UnityEngine;

/// <summary>
/// Gère le ramassage d'objets (pièces, items, coeurs)
/// </summary>
public class PickUpItem : MonoBehaviour
{
    public enum ItemType
    {
        Coin,   // Pièce
        Item,   // Item (potion, etc.)
        Heart   // Coeur de vie
    }

    [Header("Type d'objet")]
    [Tooltip("Type de l'objet à ramasser")]
    public ItemType itemType;
    
    [Header("Configuration selon le type")]
    [Tooltip("Item ScriptableObject (si type = Item)")]
    public Item item;
    
    [Tooltip("Valeur (pièces ou points de vie)")]
    public int value = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Vérifie que c'est le joueur qui ramasse
        if (!collision.CompareTag("Player"))
            return;

        // Traite selon le type d'objet
        switch (itemType)
        {
            case ItemType.Coin:
                HandleCoinPickup();
                break;

            case ItemType.Item:
                HandleItemPickup();
                break;

            case ItemType.Heart:
                HandleHeartPickup();
                break;
        }

        // Détruit l'objet après ramassage
        Destroy(gameObject);
    }

    /// <summary>
    /// Gère le ramassage d'une pièce
    /// </summary>
    private void HandleCoinPickup()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.PlayPickupCoin();

        if (Inventory.instance != null)
            Inventory.instance.AddCoins(value);
        else
            Debug.LogWarning("PickUpItem: Inventory.instance est null!");
    }

    /// <summary>
    /// Gère le ramassage d'un item
    /// </summary>
    private void HandleItemPickup()
    {
        if (item == null)
        {
            Debug.LogWarning("PickUpItem: Item non assigné!");
            return;
        }

        if (AudioManager.instance != null)
            AudioManager.instance.PlayPickupItem();

        if (Inventory.instance != null)
        {
            Inventory.instance.content.Add(item);
            Inventory.instance.UpdateInventoryUI();
        }
        else
        {
            Debug.LogWarning("PickUpItem: Inventory.instance est null!");
        }
    }

    /// <summary>
    /// Gère le ramassage d'un coeur de vie
    /// </summary>
    private void HandleHeartPickup()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.PlayUseHealthPop();

        if (ConfigPlayer.instance != null)
            ConfigPlayer.instance.Heal(value);
        else
            Debug.LogWarning("PickUpItem: ConfigPlayer.instance est null!");
    }
}