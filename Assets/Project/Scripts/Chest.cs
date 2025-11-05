using UnityEngine;
using UnityEngine.UI;

public class Chest : MonoBehaviour
{
    [SerializeField] private Text interactUI;
    private bool isInRange;
    private bool isOpened;

    public Animator animator;
    public int coinsToAdd;
    public Item[] itemsToAdd; 

    void Update()
    {
        if (isInRange && !isOpened && Input.GetKeyDown(KeyCode.E))
            OpenChest();
    }

    void OpenChest()
    {
        isOpened = true;
        animator.SetTrigger("OpenChest");

        // Ajout des coins
        Inventory.instance.AddCoins(coinsToAdd);

        // Ajout des items
        foreach (Item item in itemsToAdd)
        {
            if (item != null)
                Inventory.instance.content.Add(item);
        }

        GetComponent<BoxCollider2D>().enabled = false;
        interactUI.enabled = false;

        // Mise à jour de l'UI
        Inventory.instance.UpdateInventoryUI();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isOpened)
        {
            isInRange = true;
            interactUI.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = false;
            interactUI.enabled = false;
        }
    }
}
