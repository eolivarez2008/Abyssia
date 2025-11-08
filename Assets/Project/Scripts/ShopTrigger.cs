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
        // Cache le texte d'interaction
        interactUI.enabled = false;

        // Ouvre le shop et fournit un callback à la fermeture
        ShopManager.instance.OpenShop(itemsToSell, pnjName, OnShopClosed);
    }

    private void OnShopClosed()
    {
        // Si le joueur est toujours dans le trigger, réaffiche le texte
        if (isInRange)
            interactUI.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = true;

            // N'affiche le texte que si le shop n'est pas ouvert
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

            // Ferme le shop si le joueur sort du trigger
            if (ShopManager.instance.IsShopOpen())
                ShopManager.instance.CloseShop();
        }
    }
}
