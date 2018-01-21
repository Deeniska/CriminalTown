using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class CharacterMenu : MonoBehaviour
{
    #region References
    public GameObject characterMenuObject;
    public Scrollbar verticalScrollbar;
    public Image background;

    private Color defaultSpColor = Color.yellow;
    private Color defaultComColor = Color.white;
    private Color hospitalColor = Color.green;
    private Color arrestedColor = Color.cyan;
    private Color robberyColor = Color.gray;

    public Text levelText;
    public Text characterName;
    public Image portrait;


    public GameObject baseParameters;

    public Text health;
    public Text tiredness;
    public Text experience;

    public Slider healthSlider;
    public Slider tirednessSlider;
    public Slider experienceSlider;

    public GameObject hospitalParameters;

    public Text recovery;
    public Slider recoverySlider;
    public Text moneyRecText;
    public Image moneyRecImage;
    public Text recDaysLeft;
    public Button boostRecoveryButton;

    public GameObject arrestParameters;

    public Text opposition;
    public Slider oppositionSlider;
    public Text moneyArrText;
    public Text arrDaysLeft;
    public Button breakOutButton;

    public Text strengthText;
    public Text agilityText;
    public Text skillText;
    public Text luckText;
    public Text fearText;

    public Slider strengthSlider;
    public Slider agilitySlider;
    public Slider skillSlider;
    public Slider luckSlider;
    public Slider fearSlider;

    public Button strengthButton;
    public Button agilityButton;
    public Button skillButton;
    public Button luckButton;

    public Button undoButton;
    public Text levelUpText;
    public Text pointsCountText;

    public GameObject traitsParameters;
    public List<GameObject> traits;
    public GameObject traitPrefab;

    public Text historyText;

    public Button kickButton;
    #endregion

    private int charNumber;
    private int price;

    private Character character;
    private Character chChanged;
    private bool isStatsChanged;

    public void SetCharacterMenu(Character characterReference)
    {
        SetSlidersMaxValues();

        this.character = characterReference;

        if (character.GetType() == typeof(CommonCharacter))
        {
            CommonCharacter comChar = (CommonCharacter)character;
            portrait.sprite = comChar.Sprite;
            characterName.text = comChar.Name;
            historyText.text = comChar.History;
        }
        else if (character.GetType() == typeof(SpecialCharacter))
        {
            SpecialCharacter spChar = (SpecialCharacter)character;
            portrait.sprite = spChar.Sprite;
            characterName.text = spChar.Name;
            historyText.text = spChar.History;
        }
        levelText.text = character.Level.ToString();

        CheckPointsAndStats();
        SetTraits();
        CommonWindowSettings();

        isStatsChanged = false;
        character.OnKickEvent += (ch) => OnKickCharacterWindowReaction();
    }

    private void SetSlidersMaxValues()
    {
        strengthSlider.maxValue = CharactersOptions.maxStat;
        agilitySlider.maxValue = CharactersOptions.maxStat;
        skillSlider.maxValue = CharactersOptions.maxStat;
        luckSlider.maxValue = CharactersOptions.maxStat;
        fearSlider.maxValue = CharactersOptions.maxFear;

        healthSlider.maxValue = CharactersOptions.maxHealth;
        tirednessSlider.maxValue = CharactersOptions.maxTiredness;
        recoverySlider.maxValue = CharactersOptions.maxRecovery;
        oppositionSlider.maxValue = CharactersOptions.maxOpposition;
    }
     
    private void CommonWindowSettings()
    {
        verticalScrollbar.value = 1;
        characterMenuObject.transform.SetAsLastSibling();
        characterMenuObject.SetActive(true);
    }

    public void CheckPointsAndStats()
    {
        baseParameters.SetActive(false);
        hospitalParameters.SetActive(false);
        arrestParameters.SetActive(false);

        chChanged = character.GetStats();

        switch (character.Status)
        {
            case CharacterStatus.normal:
                {
                    if (character.GetType() == typeof(CommonCharacter))
                        background.color = defaultComColor;
                    else if (character.GetType() == typeof(SpecialCharacter))
                        background.color = defaultSpColor;

                    baseParameters.SetActive(true);
                    experienceSlider.maxValue = CharactersOptions.GetExperienceMaxValue(chChanged.Level);
                    healthSlider.value = chChanged.Health;
                    tirednessSlider.value = chChanged.Tiredness;
                    experienceSlider.value = chChanged.Exp;

                    health.text = chChanged.Health.ToString() + " / " + healthSlider.maxValue.ToString();
                    tiredness.text = chChanged.Tiredness.ToString() + " / " + tirednessSlider.maxValue.ToString();
                    experience.text = chChanged.Exp.ToString() + " / " + experienceSlider.maxValue.ToString();

                    strengthButton.interactable = true;
                    agilityButton.interactable = true;
                    skillButton.interactable = true;
                    luckButton.interactable = true;
                    undoButton.interactable = true;
                    break;
                }
            case CharacterStatus.robbery:
                {
                    background.color = robberyColor;

                    baseParameters.SetActive(true);
                    experienceSlider.maxValue = CharactersOptions.GetExperienceMaxValue(chChanged.Level);
                    healthSlider.value = chChanged.Health;
                    tirednessSlider.value = chChanged.Tiredness;
                    experienceSlider.value = chChanged.Exp;

                    health.text = chChanged.Health.ToString() + " / " + healthSlider.maxValue.ToString();
                    tiredness.text = chChanged.Tiredness.ToString() + " / " + tirednessSlider.maxValue.ToString();
                    experience.text = chChanged.Exp.ToString() + " / " + experienceSlider.maxValue.ToString();

                    strengthButton.interactable = false;
                    agilityButton.interactable = false;
                    skillButton.interactable = false;
                    luckButton.interactable = false;
                    undoButton.interactable = false;
                    break;
                }
            case CharacterStatus.hospital:
                {
                    background.color = hospitalColor;

                    price = CharactersOptions.GetBoostRecoveryPrice(chChanged.Level);
                    hospitalParameters.SetActive(true);
                    recoverySlider.value = chChanged.StatusValue;
                    recovery.text = chChanged.StatusValue.ToString() + " / " + recoverySlider.maxValue.ToString();

                    recDaysLeft.text = chChanged.DaysLeft.ToString();
                    moneyRecText.text = price.ToString();

                    boostRecoveryButton.gameObject.SetActive(chChanged.BoostCoefficient == 1 && price < DataScript.sData.money);
                    moneyRecImage.gameObject.SetActive(chChanged.BoostCoefficient == 1);
                    moneyRecText.gameObject.SetActive(chChanged.BoostCoefficient == 1);

                    strengthButton.interactable = false;
                    agilityButton.interactable = false;
                    skillButton.interactable = false;
                    luckButton.interactable = false;
                    undoButton.interactable = false;
                    break;
                }
            case CharacterStatus.arrested:
                {
                    background.color = arrestedColor;

                    price = CharactersOptions.GetBreakOutPrice(chChanged.Level);
                    arrestParameters.SetActive(true);
                    oppositionSlider.value = chChanged.StatusValue;
                    opposition.text = chChanged.StatusValue.ToString() + " / " + oppositionSlider.maxValue.ToString();

                    arrDaysLeft.text = chChanged.DaysLeft.ToString();
                    moneyArrText.text = CharactersOptions.GetBreakOutPrice(chChanged.Level).ToString();

                    if (price > DataScript.sData.money) breakOutButton.gameObject.SetActive(false);
                    else breakOutButton.gameObject.SetActive(true);

                    strengthButton.interactable = false;
                    agilityButton.interactable = false;
                    skillButton.interactable = false;
                    luckButton.interactable = false;
                    undoButton.interactable = false;
                    break;
                }
        }

        strengthText.text = chChanged.Strength.ToString();
        agilityText.text = chChanged.Agility.ToString();
        skillText.text = chChanged.Skill.ToString();
        luckText.text = chChanged.Luck.ToString();
        fearText.text = chChanged.Fear.ToString();

        strengthSlider.value = chChanged.Strength;
        agilitySlider.value = chChanged.Agility;
        skillSlider.value = chChanged.Skill;
        luckSlider.value = chChanged.Luck;
        fearSlider.value = chChanged.Fear;

        undoButton.gameObject.SetActive(character.Points > 0);

        strengthButton.gameObject.SetActive(chChanged.Points > 0 && strengthSlider.value < strengthSlider.maxValue);
        agilityButton.gameObject.SetActive(chChanged.Points > 0 && agilitySlider.value < agilitySlider.maxValue);
        skillButton.gameObject.SetActive(chChanged.Points > 0 && skillSlider.value < skillSlider.maxValue);
        luckButton.gameObject.SetActive(chChanged.Points > 0 && luckSlider.value < luckSlider.maxValue);


        levelUpText.gameObject.SetActive(chChanged.Points > 0);
        pointsCountText.gameObject.SetActive(chChanged.Points > 0);
        if (chChanged.Points > 0) pointsCountText.text = chChanged.Points.ToString();
    }

    public void OnSkillButtonClick(int skillNum)
    {
        switch (skillNum)
        {
            case 0:
                {
                    chChanged.Strength++;
                    break;
                }
            case 1:
                {
                    chChanged.Agility++;
                    break;
                }
            case 2:
                {
                    chChanged.Skill++;
                    break;
                }
            case 3:
                {
                    chChanged.Luck++;
                    break;
                }
        }
        chChanged.Points--;
        isStatsChanged = true;
        CheckPointsAndStats();
    }

    public void OnUndoButtonClick()
    {
        isStatsChanged = false;
        chChanged = character.GetStats();
        CheckPointsAndStats();
    }

    public void SetTraits()
    {
        if (traits != null)
            foreach (GameObject trait in traits)
                Destroy(trait.gameObject);

        if (character.GetType() == typeof(SpecialCharacter))
        {
            SpecialCharacter spChar = (SpecialCharacter)character;
            traits = new List<GameObject>();
            for (int i = 0; i < spChar.TraitIds.Count; i++)
            {
                traits.Add(Instantiate(traitPrefab, traitsParameters.transform));
                traits[i].GetComponent<TraitsCustomization>().CustomizeTrait(spChar.TraitIds[i]);
            }
            traitsParameters.SetActive(true);
        }
        else if (character.GetType() == typeof(CommonCharacter))
        {
            traitsParameters.SetActive(false);
        }
    }

    public void CloseAndSave()
    {
        characterMenuObject.SetActive(false);
        if (isStatsChanged)
        {
            character.SetStats(chChanged);
            character.CallOnStatsChangedEvent();
        }
        character.OnKickEvent -= (ch) => OnKickCharacterWindowReaction();
    }

    public void BoostRecovery()
    {
        chChanged.BoostCoefficient = CharactersOptions.boostedCoef;
        character.BoostCoefficient = CharactersOptions.boostedCoef;
        DataScript.sData.money -= price;
        WM1.hospital.UpdateHospital();
        CheckPointsAndStats();
        character.CallOnStatsChangedEvent();
    }

    public void BreakOut()
    {
        chChanged.Status = CharacterStatus.normal;
        character.Status = CharacterStatus.normal;
        DataScript.sData.money -= price;
        WM1.policeStation.UpdatePoliceStationCharacters();
        CheckPointsAndStats();
        character.CallOnStatsChangedEvent();
    }

    public void OnKickButtonClick()
    {
        EventButtonDetails yesButton = new EventButtonDetails
        {
            buttonText = "Да",
            action = () => DataScript.chData.RemoveCharacter(character)
        };
        EventButtonDetails noButton = new EventButtonDetails
        {
            buttonText = "Нет",
            action = WM1.modalPanel.ClosePanel
        };
        ModalPanelDetails details = new ModalPanelDetails
        {
            button0Details = yesButton,
            button1Details = noButton,
            imageSprite = portrait.sprite,
            text = character.Status == CharacterStatus.arrested ? "Вы уверены что хотите оставить данного персонажа в грязных руках копов? Это скажется на осведомлённости полиции о вашей банде"
                : "Вы уверены, что хотите выгнать данного персонажа?",
            titletext = characterName.text
        };
        WM1.modalPanel.CallModalPanel(details);
    }

    public void OnKickCharacterWindowReaction()
    {
        //Animation???
        StartCoroutine(AnimationExample());
    }

    private IEnumerator AnimationExample()
    {
        yield return new WaitForSeconds(1);
        background.color = Color.black;
        yield return new WaitForSeconds(1);
        background.color = Color.white;
        yield return new WaitForSeconds(1);
        characterMenuObject.SetActive(false);
    }
}