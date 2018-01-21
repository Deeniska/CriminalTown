using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnMarketOpen : MonoBehaviour
{
    public GameObject options;
    public Button itemPrefab;
    public Transform itemsLocation;
 
    Dictionary<ItemProperty, string> itemDict;
    public Button[] items = new Button[ItemsOptions.totalAmount];

    private void Awake()
    {
        for (int i = 0; i < ItemsOptions.totalAmount; i++)
        {
            itemDict = ItemsOptions.GetItemData(i);

            items[i] = Instantiate(itemPrefab, itemsLocation);
            items[i].GetComponent<MarketItemCustomization>().number = i;
            items[i].GetComponent<MarketItemCustomization>().itemImage.sprite = options.GetComponent<ItemsOptions>().itemsSprites[i];
            items[i].GetComponent<MarketItemCustomization>().itemName.text = itemDict[ItemProperty.name];
            items[i].GetComponent<MarketItemCustomization>().itemPrice.text = itemDict[ItemProperty.price];
        }
    }

    public void OnEnable()
    {
        transform.SetAsLastSibling();
        string isSpecial;
        for (int i = 0; i < ItemsOptions.totalAmount; i++)
        {
            itemDict = ItemsOptions.GetItemData(i);
            isSpecial = itemDict[ItemProperty.isSpecial];
            items[i].gameObject.SetActive((isSpecial == "0" && DataScript.pData.isItemAvailable[i]));
        }
    }
}