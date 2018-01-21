using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RobberyWindow : MonoBehaviour
{
    public GameObject robberyWindowObject;
    public Transform charactersLocation;
    public Button characterPrefab;
    public GameObject itemPrefab;
    public Transform itemsLocation;
    public Transform parentButton;

    public Text descriptionText;
    public Text nameText;
    public Image robberyImage;

    public GameObject itemsPanel;
    public GameObject charactersPanel;

    public RobberyType robType;
    public int locationNum;
    private Robbery robberyData;

    private List<GameObject> items = new List<GameObject>();
    private List<Button> characters = new List<Button>();


    public void OnEnable()
    {
        transform.SetAsLastSibling();
    }

    public void SetRobberyWindow(RobberyType robberyType, int locationNumber)
    {
        robType = robberyType;
        locationNum = locationNumber;

        robberyData = DataScript.eData.robberiesData[robberyType][locationNumber];

        UpdateCharacters();
        UpdateItems();

        robberyImage.sprite = WM1.robberiesOptions.robberySprites[(int)robType];
        descriptionText.text = RobberiesOptions.GetRobberyData(robType, RobberyProperty.description);
        nameText.text = RobberiesOptions.GetRobberyData(robType, RobberyProperty.name);

        robberyWindowObject.SetActive(true);
        robberyWindowObject.transform.SetAsLastSibling();
    }

    public void UpdateCharacters()
    {
        foreach (Button character in characters) Destroy(character.gameObject);
        characters.Clear();

        foreach (Button comChar in WM1.charactersPanel.commonCharacters)
        {
            CharacterCustomization charCust = comChar.GetComponent<CharacterCustomization>();
            Button tempChar;
            if (charCust.status == CharacterStatus.robbery)
            {
                if (DataScript.chData.panelComCharacters[charCust.number].LocationNum == locationNum
                    && DataScript.chData.panelComCharacters[charCust.number].StatusValue == (int)robType)
                {
                    tempChar = Instantiate(characterPrefab, charactersLocation);
                    tempChar.GetComponent<CharacterCustomization>().CustomizeCommonCharacter(charCust.number);
                    characters.Add(tempChar);
                }
            }
        }
        foreach (Button spChar in WM1.charactersPanel.specialCharacters)
        {
            CharacterCustomization charCust = spChar.GetComponent<CharacterCustomization>();
            Button tempChar;
            if (charCust.status == CharacterStatus.robbery)
            {
                if (DataScript.chData.panelSpCharacters[charCust.number].LocationNum == locationNum
                    && DataScript.chData.panelSpCharacters[charCust.number].RobberyType == (int)robType)
                {
                    tempChar = Instantiate(characterPrefab, charactersLocation);
                    tempChar.GetComponent<CharacterCustomization>().CustomizeSpecialCharacter(charCust.number);
                    characters.Add(tempChar);
                }
            }
        }
    }

    public void UpdateItems()
    {
        foreach (GameObject item in items) Destroy(item.gameObject);
        items.Clear();

        int j = 0;
        foreach (int number in robberyData.itemsCount.Keys)
        {
            items.Add(Instantiate(itemPrefab, itemsLocation));
            items[j].GetComponent<ItemCustomization>().number = number;
            items[j].GetComponent<ItemCustomization>().itemImage.sprite = WM1.itemsOptions.itemsSprites[number];
            items[j].GetComponent<ItemCustomization>().itemCount.text = robberyData.itemsCount[number].ToString();
            items[j].GetComponent<ItemCustomization>().itemName.text = ItemsOptions.GetItemData(number)[ItemProperty.name];
            items[j].SetActive(true);
            j++;
        }
    }

    public void TryToAddCharacterToRobbery(CharacterCustomization charCust, RobberyType robType, int locNum)
    {
        ModalPanelDetails details;

        if (charCust.health.value <= 10)
        {
            EventButtonDetails yesButton = new EventButtonDetails
            {
                buttonText = "Да мне плевать",
                action = () => { AddCharacterToRobberyAndUpdate(charCust.number, charCust.isSpecial, robType, locNum); }
            };
            EventButtonDetails noButton = new EventButtonDetails
            {
                buttonText = "Ладно, отдыхай",
                action = WM1.modalPanel.ClosePanel
            };
            details = new ModalPanelDetails
            {
                button0Details = yesButton,
                button1Details = noButton,
                imageSprite = charCust.portrait.sprite,
                text = "Босс, может мне лучше в больницу?",
                titletext = charCust.characterName.text
            };
            WM1.modalPanel.CallModalPanel(details);
        }
        else
        {
            AddCharacterToRobberyAndUpdate(charCust.number, charCust.isSpecial, robType, locNum);
        }

    }

    public void AddCharacterToRobberyAndUpdate(int charNum, bool isSpecial, RobberyType robberyType, int locationNum)
    {
        Character chDat;

        if (!isSpecial) chDat = DataScript.chData.panelComCharacters[charNum].GetStats();
        else chDat = DataScript.chData.panelSpCharacters[charNum].GetStats();

        chDat.Status = CharacterStatus.robbery;
        chDat.RobberyType = (int)robberyType;
        chDat.LocationNum = locationNum;

        if (!isSpecial)
        {
            DataScript.chData.panelComCharacters[charNum].SetStats(chDat);
            WM1.charactersPanel.commonCharacters[charNum].GetComponent<CharacterCustomization>().SetCharStats();
            //DataScript.eData.robberiesData[robberyType][locationNum].commonCharacters.Add(DataScript.chData.panelComCharacters[charNum]);
        }
        else
        {
            DataScript.chData.panelSpCharacters[charNum].SetStats(chDat);
            WM1.charactersPanel.specialCharacters[charNum].GetComponent<CharacterCustomization>().SetCharStats();
            //DataScript.eData.robberiesData[robberyType][locationNum].specialCharacters.Add(DataScript.chData.panelSpCharacters[charNum]);
        }

        if (robberyWindowObject.activeInHierarchy) UpdateCharacters();
        RM.rmInstance.GetRobberyCustomization(robberyType, locationNum).CounterPlus();
    }

    public void RemoveCharacterFromRobberyAndUpdate(int charNum, bool isSpecial, RobberyType robType, int locNum)
    {
        Character chDat;

        if (!isSpecial) chDat = DataScript.chData.panelComCharacters[charNum].GetStats();
        else chDat = DataScript.chData.panelSpCharacters[charNum].GetStats();

        chDat.Status = CharacterStatus.normal;
        chDat.RobberyType = 0;
        chDat.LocationNum = locationNum;

        if (!isSpecial)
        {
            DataScript.chData.panelComCharacters[charNum].SetStats(chDat);
            WM1.charactersPanel.commonCharacters[charNum].GetComponent<CharacterCustomization>().SetCharStats();
        }
        else
        {
            DataScript.chData.panelSpCharacters[charNum].SetStats(chDat);
            WM1.charactersPanel.specialCharacters[charNum].GetComponent<CharacterCustomization>().SetCharStats();
        }
        if (robberyWindowObject.activeInHierarchy) UpdateCharacters();
        RM.rmInstance.GetRobberyCustomization(robType, locationNum).CounterMinus();
    }

    public void TryToAddItemToRobbery(int itemNumber, RobberyType robberyType, int locationNum)
    {
        //ModalPanelDetails details;

        //if (DataScript.eData.IsRobberyEmpty(robberyType, locationNum))
            WM1.robberyItemsWindow.SetItemsWindow(itemNumber, isItemAdding: true, robberyType: robberyType, locationNum: locationNum);
        //else
        //{
        //    EventButtonDetails yesButton = new EventButtonDetails
        //    {
        //        buttonText = "ОК",
        //        action = () => { WM1.modalPanel.ClosePanel(); }
        //    };
        //    details = new ModalPanelDetails
        //    {
        //        iconSprite = WM1.modalPanel.wrongIcon,
        //        imageSprite = WM1.itemsOptions.itemsSprites[itemNumber],
        //        button0Details = yesButton,
        //        text = "Добавьте по крайней мере одного персонажа, прежде чем кидаться вещами!",
        //        titletext = "Ну и что это?"
        //    };
        //    WM1.modalPanel.CallModalPanel(details);
        //}
    }

    public void AddItemToRobberyAndUpdate(int itemNumber, int itemCount, RobberyType robberyType, int locationNum)
    {
        DataScript.sData.itemsCount[itemNumber] -= itemCount;
        if (DataScript.eData.robberiesData[robberyType][locationNum].itemsCount.ContainsKey(itemNumber))
        {
            DataScript.eData.robberiesData[robberyType][locationNum].itemsCount[itemNumber] += itemCount;
        }
        else DataScript.eData.robberiesData[robberyType][locationNum].itemsCount.Add(itemNumber, itemCount);
        //Debug.Log

        if (robberyWindowObject.activeInHierarchy) UpdateItems();
        WM1.itemsPanel.UpdateSingleItemWithAnimation(itemNumber);
    }

    public void TryToRemoveItemFromRobbery(int itemNumber, RobberyType robberyType, int locationNum)
    {
        WM1.robberyItemsWindow.SetItemsWindow(itemNumber, isItemAdding: false, robberyType: robberyType, locationNum: locationNum);
    }

    public void RemoveItemFromRobberyAndUpdate(int itemNumber, int itemCount, RobberyType robberyType, int locationNum)
    {
        DataScript.sData.itemsCount[itemNumber] += itemCount;
        if (DataScript.eData.robberiesData[robberyType][locationNum].itemsCount[itemNumber] == itemCount)
        {
            DataScript.eData.robberiesData[robberyType][locationNum].itemsCount.Remove(itemNumber);
        }
        else DataScript.eData.robberiesData[robberyType][locationNum].itemsCount[itemNumber] -= itemCount;
        UpdateItems();
        WM1.itemsPanel.UpdateSingleItemWithAnimation(itemNumber);
    }

    public void RemoveAllItemsFromRoobbery(RobberyType robberyType, int locationNum)
    {
        if (DataScript.eData.robberiesData[robberyType][locationNum].itemsCount != null)
        {
            foreach (var item in DataScript.eData.robberiesData[robberyType][locationNum].itemsCount)
            {
                DataScript.sData.itemsCount[item.Key] += item.Value;
                Debug.Log(DataScript.sData.itemsCount[item.Key]);
            }
            DataScript.eData.robberiesData[robberyType][locationNum].itemsCount.Clear();
        }
    }

    public void RemoveAllCharactersFromRobbery(RobberyType robberyType, int locationNum)
    {
        //foreach (CommonCharacter comChar in DataScript.eData.GetCommonCharactersForRobbery(robberyType, locationNum))
        //    RemoveCharacterFromRobbery(comChar.n;
    }

    public void CloseRobberyWindow()
    {
        robberyWindowObject.SetActive(false);
    }
}
