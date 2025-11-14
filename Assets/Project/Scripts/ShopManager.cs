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

    private bool buttonsGenerated = false;
    private System.Action onShopClosedCallback;

    private void Awake()
    {
        if (instance != null)
        {
            return;
        }

        instance = this;
    }

    public void OpenShop(Item[] items, string pnjName, System.Action onClose = null)
    {
        onShopClosedCallback = onClose;
        pnjNameText.text = pnjName;

        if (!buttonsGenerated)
        {
            GenerateButtons(items);
            buttonsGenerated = true;
        }

        animator.SetBool("isOpen", true);
    }

    private void GenerateButtons(Item[] items)
    {
        for (int i = 0; i < items.Length; i++)
        {
            GameObject button = Instantiate(sellButtonPrefab, sellButtonsParent);
            button.SetActive(true);

            if (sellButtonPrefab.activeSelf)
                sellButtonPrefab.SetActive(false);

            foreach (var text in button.GetComponentsInChildren<Text>(true))
                text.enabled = true;

            foreach (var image in button.GetComponentsInChildren<Image>(true))
                image.enabled = true;

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

        if (onShopClosedCallback != null)
        {
            onShopClosedCallback.Invoke();
            onShopClosedCallback = null;
        }
    }

    public bool IsShopOpen()
    {
        return animator.GetBool("isOpen");
    }
}
