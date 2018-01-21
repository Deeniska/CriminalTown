using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RobberyWindow : MonoBehaviour
{
    #region References
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
    #endregion

    public RobberyType robType;
    public int locationNum;
    private Robbery robberyData;

    private List<GameObject> items = new List<GameObject>();

    private Dictionary<Character, Button> charactersDict;

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
        if (charactersDict != null)
            foreach (Button character in charactersDict.Values)
                Destroy(character.gameObject);

        charactersDict = new Dictionary<Character, Button>();

        foreach (Character character in DataScript.chData.panelCharacters)
        {
            if (character.Status == CharacterStatus.robbery)
            {
                if (character.LocationNum == locationNum && character.StatusValue == (int)robType)
                {
                    charactersDict.Add(character, Instantiate(characterPrefab, charactersLocation));
                    charactersDict[character].GetComponent<CharacterCustomization>().CustomizeCharacter(character);
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

    public void TryToAddCharacterToRobbery(Character character, RobberyType robType, int locNum)
    {
        Sprite sprite = null;
        string name = null;
        ModalPanelDetails details;
        if (character.GetType() == typeof(CommonCharacter))
        {
            CommonCharacter comChar = (CommonCharacter)character;
            sprite = comChar.Sprite;
            name = comChar.Name;
        }
        else if (character.GetType() == typeof(SpecialCharacter))
        {
            SpecialCharacter spChar = (SpecialCharacter)character;
            sprite = spChar.Sprite;
            name = spChar.Name;
        }

        if (character.Health <= 10)
        {
            EventButtonDetails yesButton = new EventButtonDetails
            {
                buttonText = "Да мне плевать",
                action = () => { AddCharacterToRobberyAndUpdate(character, robType, locNum); }
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
                imageSprite = sprite,
                text = "Босс, может мне лучше в больницу?",
                titletext = name
            };
            WM1.modalPanel.CallModalPanel(details);
        }
        else
        {
            AddCharacterToRobberyAndUpdate(character, robType, locNum);
        }

    }

    public void AddCharacterToRobberyAndUpdate(Character character, RobberyType robberyType, int locationNum)
    {
        character.Status = CharacterStatus.robbery;
        character.RobberyType = (int)robberyType;
        character.LocationNum = locationNum;

        if (robberyWindowObject.activeInHierarchy) UpdateCharacters();
        RM.rmInstance.GetRobberyCustomization(robberyType, locationNum).CounterPlus();
    }

    public void RemoveCharacterFromRobberyAndUpdate(Character character, RobberyType robType, int locNum)
    {
        character.Status = CharacterStatus.normal;
        character.RobberyType = 0;
        character.LocationNum = locationNum;

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
