using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    public enum ItemType { Coin, Item, Heart }

    public ItemType itemType;
    public Item item; 
    public int value = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        switch (itemType)
        {
            case ItemType.Coin:
                AudioManager.instance.PlayPickupCoin();
                Inventory.instance.AddCoins(value);
                break;

            case ItemType.Item:
                if (item != null)
                {
                    AudioManager.instance.PlayPickupItem();
                    Inventory.instance.content.Add(item);
                    Inventory.instance.UpdateInventoryUI();
                }
                break;

            case ItemType.Heart:
                AudioManager.instance.PlayUseHealthPop();
                ConfigPlayer.instance.Heal(value);
                break;
        }

        Destroy(gameObject);
    }
}
