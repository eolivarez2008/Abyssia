using UnityEngine;
using UnityEngine.UI;

public class ShopTrigger : MonoBehaviour
{
    private bool isInRange;
    public Text interactUI;

    public string pnjName;
    public Item[] itemsToSell;

    void Update()
    {
        if (isInRange && Input.GetKeyDown(KeyCode.E))
        {
            OpenShop();
        }

        if (ShopManager.instance.IsShopOpen() && Input.GetKeyDown(KeyCode.Tab))
        {
            ShopManager.instance.CloseShop();
        }
    }

    private void OpenShop()
    {
        interactUI.enabled = false;

        ShopManager.instance.OpenShop(itemsToSell, pnjName, OnShopClosed);
    }

    private void OnShopClosed()
    {
        if (isInRange)
            interactUI.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = true;

            if (!ShopManager.instance.IsShopOpen())
                interactUI.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = false;
            interactUI.enabled = false;

            if (ShopManager.instance.IsShopOpen())
                ShopManager.instance.CloseShop();
        }
    }
}
