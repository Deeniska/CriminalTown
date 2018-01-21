using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OnHospitalOpen : MonoBehaviour
{
    public GameObject hospitalObject;
    public Transform charactersLocation;
    public Transform parentButton;

    public GameObject characterPrefab;

    private Dictionary<Character, GameObject> charactersDict;

    private void OnEnable()
    {
        hospitalObject.transform.SetAsLastSibling();
    }

    public void UpdateHospital()
    {
        if (charactersDict != null)
            foreach (GameObject character in charactersDict.Values)
                if (character.gameObject) Destroy(character.gameObject);

        charactersDict = new Dictionary<Character, GameObject>();

        foreach (Character character in DataScript.chData.panelCharacters)
        {
            if (character.Status == CharacterStatus.hospital)
            {
                charactersDict.Add(character, Instantiate(characterPrefab, charactersLocation));
                charactersDict[character].GetComponent<CharacterCustomization>().CustomizeCharacter(character);
            }
        }
    }

    public void CloseHospital()
    {
        hospitalObject.SetActive(false);
    }

    public void TryToAddCharacterToHospital(Character character)
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

        if (character.Health <= 90)
        {
            EventButtonDetails yesButton = new EventButtonDetails
            {
                buttonText = "Да",
                action = () => {AddCharacterToHospital(character); }
            };
            EventButtonDetails noButton = new EventButtonDetails
            {
                buttonText = "Нет",
                action = WM1.modalPanel.ClosePanel
            };
            details = new ModalPanelDetails
            {
                button0Details = yesButton,
                button1Details = noButton,
                imageSprite = sprite,
                text = "Отправить персонажа на принудительное лечение?",
                titletext = name
            };
        }
        else
        {
            EventButtonDetails noButton = new EventButtonDetails
            {
                buttonText = "Ну ладно...",
                action = WM1.modalPanel.ClosePanel
            };
            details = new ModalPanelDetails
            {
                button1Details = noButton,
                imageSprite = sprite,
                text = "Босс, я не пойду в больницу из-за этой царапины!",
                titletext = name
            };
        }
        WM1.modalPanel.CallModalPanel(details);

    }

    public void AddCharacterToHospital(Character character)
    {
        character.Status = CharacterStatus.hospital;
        character.StatusValue = character.Health;
        character.BoostCoefficient = 1;
        UpdateHospital();
    }
}
