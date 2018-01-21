using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterCustomization : MonoBehaviour
{
    public Sprite hospitalIcon;
    public Sprite normalIcon;
    public Sprite policeIcon;
    public Sprite deathIcon;
    public Sprite prisonIcon;

    #region References
    public Button characterObject;
    public Text levelText;
    public Image levelUpImage;

    public Text characterName;
    public Image portrait;

    public GameObject baseStats;

    public Slider health;
    public Slider tiredness;
    public Text strength;
    public Text agility;
    public Text skill;
    public Text luck;
    public Text fear;

    public GameObject hospitalStats;

    public Slider recovery;
    public Text recDaysLeft;

    public GameObject arrestStats;

    public Slider opposition;
    public Text arrDaysLeft;
    #endregion

    public Character character;
    public CharacterStatus status;


    //private Color defaultSpColor = Color.yellow;
    //private Color defaultComColor = Color.white;
    //private Color hospitalColor = Color.green;
    //private Color arrestedColor = Color.cyan;
    //private Color robberyColor = Color.gray;

    private Animator animator;

    public Animator Animator
    {
        get
        {
            if (animator == null) animator = characterObject.GetComponent<Animator>();
            return animator;
        }
        set
        {
            if (animator == null) animator = characterObject.GetComponent<Animator>();
            animator = value;
        }
    }

    public void OnClick()
    {
        WM1.characterMenu.SetCharacterMenu(character);
    }

    public void CustomizeCharacter(Character character)
    {
        this.character = character;

        tiredness.maxValue = CharactersOptions.maxTiredness;
        health.maxValue = CharactersOptions.maxHealth;

        if (character.GetType() == typeof(CommonCharacter))
        {
            CommonCharacter comChar = (CommonCharacter)character;
            portrait.sprite = comChar.Sprite;
            characterName.text = comChar.Name;
            status = comChar.Status;
        }
        else if (character.GetType() == typeof(SpecialCharacter))
        {
            SpecialCharacter spChar = (SpecialCharacter)character;
            portrait.sprite = spChar.Sprite;
            characterName.text = spChar.Name;
            status = spChar.Status;
        }
        else
        {
            Debug.LogError("Invalyd type of character! LOL");
        }
        SetCharStats();
    }

    public void SetCharStats()
    {
        baseStats.SetActive(false);
        arrestStats.SetActive(false);
        hospitalStats.SetActive(false);

        switch (status)
        {
            case CharacterStatus.normal:
                {
                    characterObject.GetComponent<Image>().sprite = normalIcon;
                    Animator.SetTrigger("Normal");
                    baseStats.SetActive(true);
                    break;
                }
            case CharacterStatus.robbery:
                {
                    characterObject.GetComponent<Image>().sprite = normalIcon;
                    Animator.SetTrigger("Normal");
                    baseStats.SetActive(true);
                    break;
                }
            case CharacterStatus.hospital:
                {
                    characterObject.GetComponent<Image>().sprite = hospitalIcon;
                    Animator.SetTrigger("Hospital");
                    hospitalStats.SetActive(true);
                    recovery.value = character.StatusValue;
                    recDaysLeft.text = "Осталось дней: " + character.DaysLeft.ToString();
                    break;
                }
            case CharacterStatus.arrested:
                {
                    characterObject.GetComponent<Image>().sprite = policeIcon;
                    if (character.DaysLeft < 2) Animator.SetTrigger("PoliceOneDayLeft");
                    else Animator.SetTrigger("Police");
                    arrestStats.SetActive(true);
                    opposition.value = character.StatusValue;
                    arrDaysLeft.text = "Осталось дней: " + character.DaysLeft.ToString();
                    break;
                }
        }

        if (character.Points > 0) levelUpImage.gameObject.SetActive(true);
        else levelUpImage.gameObject.SetActive(false);
        levelText.text = character.Level.ToString();

        health.value = character.Health;
        tiredness.value = character.Tiredness;
        strength.text = character.Strength.ToString();
        agility.text = character.Agility.ToString();
        skill.text = character.Skill.ToString();
        luck.text = character.Luck.ToString();
        fear.text = character.Fear.ToString();
    }
}
