using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gère l'interaction avec un coffre contenant des pièces et/ou des items
/// </summary>
public class Chest : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Texte affichant comment interagir")]
    [SerializeField] private Text interactUI;

    [Header("Animation")]
    [Tooltip("Animator contrôlant l'ouverture du coffre")]
    public Animator animator;

    [Header("Contenu du coffre")]
    [Tooltip("Nombre de pièces à ajouter")]
    public int coinsToAdd;
    
    [Tooltip("Items à ajouter à l'inventaire")]
    public Item[] itemsToAdd;

    [Header("Input Settings")]
    [Tooltip("Touche pour ouvrir le coffre")]
    public KeyCode openKey = KeyCode.E;

    private bool isInRange;
    private bool isOpened;

    void Update()
    {
        // Ouvre le coffre si le joueur appuie sur la touche d'interaction
        if (isInRange && !isOpened && Input.GetKeyDown(openKey))
        {
            OpenChest();
        }
    }

    /// <summary>
    /// Ouvre le coffre et donne son contenu au joueur
    /// </summary>
    void OpenChest()
    {
        if (isOpened) return;

        isOpened = true;

        // Déclenche l'animation d'ouverture
        if (animator != null)
        {
            animator.SetTrigger("OpenChest");
        }

        // Joue le son d'ouverture
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayChestOpen();
        }

        // Ajoute les pièces
        if (Inventory.instance != null && coinsToAdd > 0)
        {
            Inventory.instance.AddCoins(coinsToAdd);
        }

        // Ajoute les items
        if (Inventory.instance != null && itemsToAdd != null)
        {
            foreach (Item item in itemsToAdd)
            {
                if (item != null)
                {
                    Inventory.instance.content.Add(item);
                }
            }
        }

        // Désactive le collider pour empêcher une nouvelle interaction
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            boxCollider.enabled = false;
        }

        // Cache l'UI d'interaction
        if (interactUI != null)
        {
            interactUI.enabled = false;
        }

        // Met à jour l'UI de l'inventaire
        if (Inventory.instance != null)
        {
            Inventory.instance.UpdateInventoryUI();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isOpened)
        {
            isInRange = true;
            
            if (interactUI != null)
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
            {
                interactUI.enabled = false;
            }
        }
    }
}