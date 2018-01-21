using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CampCharacterCustomization : MonoBehaviour
{
    #region References
    public Text levelText;
    public Image portrait;
    public Text characterName;
    public Text strength;
    public Text agility;
    public Text skill;
    public Text luck;
    public Text fear;
    public Text description;

    public GameObject trait;
    public Text traitDescriptionText;
    public Text traitNameText;
    public Image traitImage;
    #endregion

    private Character character;
    public Button characterButton;

    private Color defaultSpColor = Color.yellow;
    private Color defaultComColor = Color.white;

    public void OnClick()
    {
        ColorBlock tempColorBlock = characterButton.colors;
        Color highlightColor = tempColorBlock.normalColor;
        highlightColor.a = 0.5f;
        tempColorBlock.normalColor = highlightColor;
        tempColorBlock.highlightedColor = highlightColor;
        characterButton.colors = tempColorBlock;
        WM1.banditCamp.HighlightCharacter(character);
    }

    public void SetDefaultColor()
    {
        Color tempColor = character.GetType() == typeof(SpecialCharacter) ?
            defaultSpColor : defaultComColor;
        ColorBlock tempColorBlock = characterButton.colors;
        tempColorBlock.normalColor = tempColor;
        tempColorBlock.highlightedColor = tempColor;
        characterButton.colors = tempColorBlock;
    }

    public void CustomizeCharacter(Character character)
    {
        this.character = character;
        if (character.GetType() == typeof(SpecialCharacter))
        {
            CustomizeSpecialCharacter();
        }
        else if (character.GetType() == typeof(CommonCharacter))
        {
            CustomizeCommonCharacter();
        }
    }

    public void CustomizeCommonCharacter()
    {
        CommonCharacter comChar = (CommonCharacter)character;
        portrait.sprite = comChar.Sprite;
        characterName.text = comChar.Name;
        description.text = comChar.History;
        trait.SetActive(false);

        SetCharStats();
        SetDefaultColor();
    }

    public void CustomizeSpecialCharacter()
    {
        SpecialCharacter spChar = (SpecialCharacter)character;
        portrait.sprite = spChar.Sprite;
        characterName.text = spChar.Name;
        description.text = spChar.History;

        Trait newTrait = TraitsOptions.GetTrait(spChar.TraitIds[0]);
        traitDescriptionText.text = newTrait.description;
        traitNameText.text = newTrait.name;
        traitImage.sprite = WM1.traitsOptions.traitsSprites[spChar.TraitIds[0]];
        trait.SetActive(true);

        SetCharStats();
        SetDefaultColor();
    }

    private void SetCharStats()
    {
        levelText.text = character.Level.ToString() + " Ур.";
        strength.text = character.Strength.ToString();
        agility.text = character.Agility.ToString();
        skill.text = character.Skill.ToString();
        luck.text = character.Luck.ToString();
        fear.text = character.Fear.ToString();
    }
}
