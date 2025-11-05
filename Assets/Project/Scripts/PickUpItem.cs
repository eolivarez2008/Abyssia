using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    public enum ItemType { Coin, Item, Heart }

    public ItemType itemType;
    public Item item; 
    public int value = 1; // valeur pour les pièces ou les cœurs

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        switch (itemType)
        {
            case ItemType.Coin:
                Inventory.instance.AddCoins(value);
                break;

            case ItemType.Item:
                if (item != null)
                {
                    Inventory.instance.content.Add(item);
                    Inventory.instance.UpdateInventoryUI();
                }
                break;

            case ItemType.Heart:
                ConfigPlayer.instance.Heal(value);
                break;
        }

        Destroy(gameObject);
    }
}
