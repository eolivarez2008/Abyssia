using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public Text pnjNameText;
    public Animator animator;

    public GameObject sellButtonPrefab;
    public Transform sellButtonsParent;

    public static ShopManager instance;

    private System.Action onShopClosedCallback;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Il y a plus d'une instance de ShopManager dans la scène");
            return;
        }

        instance = this;
    }

    // Ajout d'un callback facultatif
    public void OpenShop(Item[] items, string pnjName, System.Action onClose = null)
    {
        onShopClosedCallback = onClose;

        pnjNameText.text = pnjName;
        UpdateItemsToSell(items);
        animator.SetBool("isOpen", true);
    }

    void UpdateItemsToSell(Item[] items)
    {
        // Supprime les boutons existants
        for (int i = 0; i < sellButtonsParent.childCount; i++)
        {
            Destroy(sellButtonsParent.GetChild(i).gameObject);
        }

        // Instancie un bouton pour chaque item
        for (int i = 0; i < items.Length; i++)
        {
            GameObject button = Instantiate(sellButtonPrefab, sellButtonsParent);
            SellButtonItem buttonScript = button.GetComponent<SellButtonItem>();
            buttonScript.itemName.text = items[i].nameItem;
            buttonScript.itemImage.sprite = items[i].image;
            buttonScript.itemPrice.text = items[i].price.ToString();

            buttonScript.item = items[i];

            button.GetComponent<Button>().onClick.AddListener(delegate { buttonScript.BuyItem(); });
        }
    }

    public void CloseShop()
    {
        animator.SetBool("isOpen", false);

        // Appelle le callback si défini
        if (onShopClosedCallback != null)
        {
            onShopClosedCallback.Invoke();
            onShopClosedCallback = null;
        }
    }

    // Méthode pratique pour savoir si le shop est ouvert
    public bool IsShopOpen()
    {
        return animator.GetBool("isOpen");
    }
}
