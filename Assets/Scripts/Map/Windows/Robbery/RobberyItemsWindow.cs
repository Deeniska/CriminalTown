using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobberyItemsWindow : MonoBehaviour
{
    public GameObject itemOptions;
    public Image itemImage;
    public Text description;
    public Text itemName;
    public Text countText;
    public Text buttonText;
    public Slider countSlider;
    public Button addRemoveButton;
    public GameObject itemsWindowObject;


    private int number;
    private bool isAdding;
    private RobberyType robType;
    private int locNum;


    public static RobberyItemsWindow itemsWindow;


    public void SetItemsWindow(int numberOfItem, bool isItemAdding, RobberyType robberyType, int locationNum)
    {
        number = numberOfItem;
        isAdding = isItemAdding;
        robType = robberyType;
        locNum = locationNum;

        itemImage.sprite = itemOptions.GetComponent<ItemsOptions>().itemsSprites[number];
        itemName.text = ItemsOptions.GetItemData(number)[ItemProperty.name];

        countSlider.interactable = true;
        addRemoveButton.interactable = false;

        if (isAdding)
        {
            buttonText.text = "Добавить";
            countSlider.maxValue = DataScript.sData.itemsCount[numberOfItem] <= 10 ?
                DataScript.sData.itemsCount[numberOfItem] : 10;
        }
        else
        {
            buttonText.text = "Убрать";
            countSlider.maxValue = DataScript.eData.robberiesData[robType][locNum].itemsCount[number] <= 10 ?
                DataScript.eData.robberiesData[robType][locNum].itemsCount[number] : 10;
        }

        
        countSlider.value = 0;
        countText.text = "0";


        itemsWindowObject.transform.SetAsLastSibling();
        itemsWindowObject.SetActive(true);
    }

    public void AddRemoveButtonClick()
    {
        if (isAdding) WM1.robberyWindow.AddItemToRobberyAndUpdate(number, (int)countSlider.value, robType, locNum);
        else WM1.robberyWindow.RemoveItemFromRobberyAndUpdate(number, (int)countSlider.value, robType, locNum);
        itemsWindowObject.SetActive(false);
    }

    public void OnSliderValueChanged()
    {
        countText.text = countSlider.value.ToString();
        if (countSlider.value > 0) addRemoveButton.interactable = true;
        else addRemoveButton.interactable = false;
    }

}
