using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gère un bouton d'achat d'item dans un magasin
/// </summary>
public class SellButtonItem : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Texte affichant le nom de l'item")]
    public Text itemName;
    
    [Tooltip("Image de l'item")]
    public Image itemImage;
    
    [Tooltip("Texte affichant le prix")]
    public Text itemPrice;

    [Header("Item Data")]
    [Tooltip("Item à vendre")]
    public Item item;

    /// <summary>
    /// Achète l'item (appelé par le bouton)
    /// </summary>
    public void BuyItem()
    {
        if (item == null)
        {
            Debug.LogWarning("SellButtonItem: Item non assigné!");
            return;
        }

        if (Inventory.instance == null)
        {
            Debug.LogWarning("SellButtonItem: Inventory.instance est null!");
            return;
        }

        Inventory inventory = Inventory.instance;
        
        // Vérifie si le joueur a assez de pièces
        if (inventory.coinsCount >= item.price)
        {
            // Joue le son de validation
            if (AudioManager.instance != null)
                AudioManager.instance.PlayValidate();
            
            // Ajoute l'item à l'inventaire
            inventory.content.Add(item);
            inventory.UpdateInventoryUI();
            
            // Retire les pièces
            inventory.coinsCount -= item.price;
            inventory.UpdateTextUI();
        }
        else
        {
            // Pas assez de pièces, joue le son d'erreur
            if (AudioManager.instance != null)
                AudioManager.instance.PlayError();
            
            Debug.Log($"SellButtonItem: Pas assez de pièces pour acheter {item.nameItem}");
        }
    }
}