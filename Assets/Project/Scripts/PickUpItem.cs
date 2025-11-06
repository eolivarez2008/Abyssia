using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    public bool isCoin;
    public Item item; 
    public int coinValue = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (isCoin)
        {
            Inventory.instance.AddCoins(coinValue);
        }
        else if (item != null)
        {
            Inventory.instance.content.Add(item);
            Inventory.instance.UpdateInventoryUI();
        }

        Destroy(gameObject);
    }
}
