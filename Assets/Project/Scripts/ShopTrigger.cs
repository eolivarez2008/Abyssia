using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Trigger pour ouvrir un magasin quand le joueur interagit avec
/// </summary>
public class ShopTrigger : MonoBehaviour
{
    [Header("Shop Configuration")]
    [Tooltip("Nom du marchand")]
    public string pnjName;
    
    [Tooltip("Items à vendre dans ce magasin")]
    public Item[] itemsToSell;

    [Header("UI")]
    [Tooltip("Texte affichant comment interagir")]
    public Text interactUI;

    [Header("Input Settings")]
    [Tooltip("Touche pour ouvrir le magasin")]
    public KeyCode interactKey = KeyCode.E;
    
    [Tooltip("Touche pour fermer le magasin")]
    public KeyCode closeKey = KeyCode.Tab;

    private bool isInRange;

    void Update()
    {
        if (ShopManager.instance == null)
        {
            Debug.LogWarning("ShopTrigger: ShopManager.instance est null!");
            return;
        }

        // Ouvre le magasin
        if (isInRange && Input.GetKeyDown(interactKey) && !ShopManager.instance.IsShopOpen())
        {
            OpenShop();
        }

        // Ferme le magasin avec Tab
        if (ShopManager.instance.IsShopOpen() && Input.GetKeyDown(closeKey))
        {
            ShopManager.instance.CloseShop();
        }
    }

    /// <summary>
    /// Ouvre le magasin
    /// </summary>
    private void OpenShop()
    {
        if (ShopManager.instance == null)
        {
            Debug.LogError("ShopTrigger: ShopManager.instance est null!");
            return;
        }

        if (itemsToSell == null || itemsToSell.Length == 0)
        {
            Debug.LogWarning("ShopTrigger: Aucun item à vendre!");
            return;
        }

        if (interactUI != null)
            interactUI.enabled = false;

        ShopManager.instance.OpenShop(itemsToSell, pnjName, OnShopClosed);
    }

    /// <summary>
    /// Appelé quand le magasin se ferme
    /// </summary>
    private void OnShopClosed()
    {
        // Réaffiche l'UI d'interaction si le joueur est encore dans la zone
        if (isInRange && interactUI != null)
            interactUI.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = true;

            // Affiche l'UI d'interaction si le magasin n'est pas ouvert
            if (ShopManager.instance != null && 
                !ShopManager.instance.IsShopOpen() && 
                interactUI != null)
            {
                interactUI.enabled = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = false;
            
            if (interactUI != null)
                interactUI.enabled = false;

            // Ferme le magasin si le joueur s'éloigne
            if (ShopManager.instance != null && ShopManager.instance.IsShopOpen())
            {
                ShopManager.instance.CloseShop();
            }
        }
    }
}