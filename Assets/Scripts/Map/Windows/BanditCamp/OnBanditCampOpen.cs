using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnBanditCampOpen : MonoBehaviour
{
    #region References
    public Transform charactersLocation;
    public Button characterPrefab;

    public Image moneyImage;
    public Text priceText;
    public Button hireButton;
    #endregion

    private Dictionary<Character, Button> charactersDict;

    private Character highlightedChar;
    private int highlightedCharPrice;

    private void OnEnable()
    {
        transform.SetAsLastSibling();
    }

    public void UpdateBanditCamp()
    {
        moneyImage.gameObject.SetActive(false);
        priceText.gameObject.SetActive(false);
        hireButton.interactable = false;

        if (charactersDict != null)
            foreach (Button character in charactersDict.Values)
                if (character.gameObject) Destroy(character.gameObject);

        charactersDict = new Dictionary<Character, Button>();

        foreach (Character character in DataScript.chData.campCharacters)
        {
            charactersDict.Add(character, Instantiate(characterPrefab, charactersLocation));
            charactersDict[character].GetComponent<CampCharacterCustomization>().CustomizeCharacter(character);
        }
    }

    public void HighlightCharacter(Character character)
    {
        if (highlightedChar != null)
        {
            charactersDict[highlightedChar].GetComponent<CampCharacterCustomization>().SetDefaultColor();
        }

        moneyImage.gameObject.SetActive(true);
        priceText.gameObject.SetActive(true);

        if (character.GetType() == typeof(CommonCharacter))
            highlightedCharPrice = CharactersOptions.GetComPrice(character.Level);
        else if (character.GetType() == typeof(SpecialCharacter))
            highlightedCharPrice = CharactersOptions.GetSpPrice(character.Level);

        priceText.text = highlightedCharPrice.ToString();

        if (DataScript.sData.money >= highlightedCharPrice
            && DataScript.chData.panelCharacters.Count < CharactersOptions.panelCellsMaxAmount)
        {
            hireButton.interactable = true;
        }
        else hireButton.interactable = false;

        highlightedChar = character;
    }

    public void HireCharacter()
    {
        Destroy(charactersDict[highlightedChar].gameObject);
        DataScript.sData.money -= highlightedCharPrice;

        moneyImage.gameObject.SetActive(false);
        priceText.gameObject.SetActive(false);
        hireButton.interactable = false;

        DataScript.chData.AddCharacter(highlightedChar);
        charactersDict.Remove(highlightedChar);
    }
}
