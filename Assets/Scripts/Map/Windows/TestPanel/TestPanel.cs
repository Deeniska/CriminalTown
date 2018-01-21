using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TestPanel : MonoBehaviour
{
    public InputField charNumberField;
    public InputField expField;
    public Dropdown charType;
    public Text errorText;

    private void OnEnable()
    {
        transform.SetAsLastSibling();
    }

    public void OnAddExpButtonClick()
    {
        DataScript.chData.panelCharacters[0].AddExperience(5000);
        //int charNumber;
        //int exp;

        //int.TryParse(charNumberField.text, out charNumber);
        //int.TryParse(expField.text, out exp);
        //if ((charType.value == 0 && charNumber > DataScript.chData.panelComCharacters.Count - 1) ||
        //    (charType.value == 1 && charNumber > DataScript.chData.panelSpCharacters.Count - 1))
        //{
        //    errorText.text = "Нет такого персонажа!!!";
        //}
        //else if (exp <= 0)
        //{
        //    errorText.text = "Персонажи не могут деградировать в этой игре!!!";
        //}
        //else
        //{
        //    errorText.text = null;
        //    if (charType.value == 0)
        //    {
        //        CharactersOptions.AddExperienceToChar(false, charNumber, exp);
        //        WM1.charactersPanel.commonCharacters[charNumber].GetComponent<CharacterCustomization>().SetCharStats();
        //    }
        //    if (charType.value == 1)
        //    {
        //        CharactersOptions.AddExperienceToChar(true, charNumber, exp);
        //        WM1.charactersPanel.specialCharacters[charNumber].GetComponent<CharacterCustomization>().SetCharStats();
        //    }
        //    DataScript.SaveCharactersData();
        //}
    }

    public void SaveAll()
    {
        DataScript.SaveAll();
    }
}
