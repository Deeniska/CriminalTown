using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class NightButton : MonoBehaviour
{
    public static bool isNight;

    private List<NightRobberyData> robberies;

    public Image map;
    Color defaultMapColor;
    public GameObject nightAnimation;

    private float animationTime = 2;
    private float animationTimer = 0;

    private float eventTime = 4;

    private int currentEventNum;

    public void OnNightButtonClick()
    {
        EventButtonDetails yesButton = new EventButtonDetails
        {
            buttonText = "Да",
            action = StartNight
        };
        EventButtonDetails noButton = new EventButtonDetails
        {
            buttonText = "Нет",
            action = WM1.modalPanel.ClosePanel
        };

        foreach (Character character in DataScript.chData.panelCharacters)
        {
            if (character.Status == CharacterStatus.arrested)
                if (character.DaysLeft < 2)
                {
                    Sprite sprite = null;
                    string name = null;
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

                    ModalPanelDetails details = new ModalPanelDetails
                    {
                        button0Details = yesButton,
                        button1Details = noButton,
                        imageSprite = sprite,
                        text = "Этот персонаж скоро нас сдаст. Босс, ты уверен, что стоит оставить его в грязных руках копов?",
                        titletext = name
                    };
                    WM1.modalPanel.CallModalPanel(details);
                    return;
                }
        }
        StartNight();
    }

    private void StartNight()
    {
        isNight = true;

        SetActiveMapAndUpdate(false);

        StartCoroutine(NightAnimation());
        CalculateEvents();
        UpdateDataAfterDay();
    }

    IEnumerator NightAnimation()
    {
        nightAnimation.SetActive(true);

        animationTimer = 0;
        defaultMapColor = map.color;
        while (animationTimer < 1)
        {
            animationTimer += Time.deltaTime / animationTime;
            map.color = Color.Lerp(defaultMapColor, Color.black, animationTimer);
            yield return null;
        }

        nightAnimation.SetActive(false);
        yield return new WaitForSeconds(eventTime);

        StartCoroutine(NightEvents());
        yield break;
    }

    IEnumerator NightEvents()
    {
        while (GetEventNum(out currentEventNum))
        {
            Debug.Log("Call robbery event: " + currentEventNum);
            NightRobberyData rob = robberies[currentEventNum];

            if (rob.nightEvent.rootNode != null)
            {
                RM.rmInstance.AddNightEvent(rob.robberyType, rob.locationNum,
                    () => { WM1.nightEventWindow.ShowChoice(rob.nightEvent.rootNode); }, EventStatus.inProgress, eventTime);
                yield return new WaitForSeconds(eventTime);
                if (NightEventWindow.choice == -1)
                {
                    WM1.nightEventWindow.CloseWindow();
                    MakeChoice(rob.nightEvent.rootNode, Random.Range(0, rob.nightEvent.rootNode.buttons.Count));
                }
                RM.rmInstance.ResetNightEvent(rob.robberyType, rob.locationNum);
                ApplyChangesAfterChoice(currentEventNum);

                if (rob.nightEvent.rootNode.buttons[NightEventWindow.choice].nextEventNode != null)
                    rob.nightEvent.rootNode = rob.nightEvent.rootNode.buttons[NightEventWindow.choice].nextEventNode;
                else rob.nightEvent.rootNode = null;
            }
            else
            {
                switch (GetResult(currentEventNum))
                {
                    case false:
                        RM.rmInstance.AddNightEvent(rob.robberyType, rob.locationNum,
                            () => { WM1.nightEventWindow.ShowFail(rob.nightEvent.fail); }, EventStatus.fail, eventTime);

                        yield return new WaitForSeconds(eventTime);
                        if (NightEventWindow.choice == -1)
                        {
                            WM1.nightEventWindow.CloseWindow();
                            MakeChoice(rob.nightEvent.fail, 0);
                        }
                        RM.rmInstance.ResetNightEvent(rob.robberyType, rob.locationNum);
                        rob.result = false;
                        break;
                    case true:
                        RM.rmInstance.AddNightEvent(rob.robberyType, rob.locationNum,
                            () => { WM1.nightEventWindow.ShowSuccess(rob.nightEvent.success, rob.awards, rob.money); }, EventStatus.success, eventTime);

                        yield return new WaitForSeconds(eventTime);
                        if (NightEventWindow.choice == -1)
                        {
                            MakeChoice(rob.nightEvent.success, 0);
                            WM1.nightEventWindow.CloseWindow();
                        }
                        RM.rmInstance.ResetNightEvent(rob.robberyType, rob.locationNum);
                        rob.result = true;
                        break;
                };
                rob.nightEvent = null;
            }
        }

        StartCoroutine(DayAnimation());
        UpdateDataAfterNight();
        yield break;
    }

    IEnumerator DayAnimation()
    {
        nightAnimation.SetActive(true);

        animationTimer = 0;
        while (animationTimer < 1)
        {
            animationTimer += Time.deltaTime / animationTime;
            map.color = Color.Lerp(Color.black, defaultMapColor, animationTimer);
            yield return null;
        }

        nightAnimation.SetActive(false);

        yield return new WaitForSeconds(eventTime);


        FinishNight();
        yield break;
    }

    private void FinishNight()
    {
        WM1.nightResumeWindow.SetActive(true);
        SetActiveMapAndUpdate(true);

        isNight = false;
    }

    private void CalculateEvents()
    {
        robberies = new List<NightRobberyData>();

        foreach (RobberyType robberyType in DataScript.eData.robberiesData.Keys)
            foreach (int locationNum in DataScript.eData.robberiesData[robberyType].Keys)
            {
                //Avoid empty robberies
                if (DataScript.eData.IsRobberyEmpty(robberyType, locationNum))
                {
                    robberies.Add(new NightRobberyData
                    {
                        locationNum = locationNum,
                        robberyType = robberyType,
                    });
                }
            }

        if (robberies != null)
            foreach (NightRobberyData robbery in robberies)
            {
                robbery.nightEvent = NightEventsOptions.GetRandomEvent(robbery.robberyType);
                robbery.chance = RobberiesOptions.CalculatePreliminaryChance(robbery.robberyType, robbery.locationNum);
                robbery.policeChance = Random.Range(0, 51);
                robbery.hospitalChance = Random.Range(0, 51);
                robbery.money = RobberiesOptions.GetRobberyMoneyRewardAtTheCurrentMoment(robbery.robberyType);
                robbery.awards = RobberiesOptions.GetRobberyAwardsAtTheCurrentMoment(robbery.robberyType);
                robbery.policeKnowledge = 1;
                robbery.characters = new List<Character>();

                foreach (Character character in DataScript.eData.GetCharactersForRobbery(robbery.robberyType, robbery.locationNum))
                    robbery.characters.Add(character);
            }
    }

    private void UpdateDataAfterDay()
    {
        foreach (Character character in DataScript.chData.panelCharacters)
        {
            if (character.Status == CharacterStatus.hospital)
            {
                character.StatusValue += CharactersOptions.recoveryStep * character.BoostCoefficient;
                if (character.StatusValue >= 100)
                {
                    //WM1.charactersPanel.charactersDict[character].GetComponent<CharacterCustomization>().Animator.SetTrigger("Recovering");
                    character.Status = CharacterStatus.normal;
                    character.Health = 100;
                }
            }
            if (character.Status == CharacterStatus.arrested)
            {
                character.StatusValue -= character.Fear;
                if (character.StatusValue <= 0)
                {
                    DataScript.eData.policeKnowledge += 10;
                    DataScript.chData.RemoveCharacter(character);
                }
            }
        }
    }

    private void UpdateDataAfterNight()
    {
        foreach (NightRobberyData robbery in robberies)
        {
            DataScript.eData.policeKnowledge++;
            DataScript.sData.money += robbery.money;
            foreach (int itemNum in robbery.awards.Keys)
                DataScript.sData.itemsCount[itemNum] += robbery.awards[itemNum];
            
            foreach (CommonCharacter character in robbery.characters)
            {
                if (character.Health <= 0)
                {
                    //TO HOSPITAL
                }
                character.Status = CharacterStatus.normal;
            }
            WM1.robberyWindow.RemoveAllItemsFromRoobbery(robbery.robberyType, robbery.locationNum);
        }
        robberies.Clear();
        NightEventsOptions.ClearUsedEvents();
        RobberiesOptions.GetNewRobberies();
    }

    private void SetActiveMapAndUpdate(bool status)
    {
        if (status == false)
        {
            WM1.wm1Instance.CloseAllDayWindows();
            RM.rmInstance.DeactivateAllRobberies();
        }
        else if (status == true)
        {
            RM.rmInstance.UpdateRobberies();
            WM1.charactersPanel.UpdateCharactersPanel();
            WM1.itemsPanel.UpdateItemsPanel();
        }

        WM1.SetActivePanels(status);
        PM.SetActiveAllPlaces(status);
    }

    private bool GetEventNum(out int eventNum)
    {
        if (robberies.Count == 0)
        {
            eventNum = -1;
            return false;
        }

        int rndEventNum = Random.Range(0, robberies.Count);

        for (int i = 0; robberies[rndEventNum].nightEvent == null; i++)
        {
            if (i > robberies.Count)
            {
                Debug.Log("No more events");
                eventNum = -1;
                return false;
            }
            rndEventNum++;
            if (rndEventNum >= robberies.Count) rndEventNum = 0;
        }

        eventNum = rndEventNum;
        return true;
    }

    public void MakeChoice(NightEventNode eventNode, int choiceNum)
    {
        NightEventWindow.choice = choiceNum;
        RM.rmInstance.ResetNightEvent(robberies[currentEventNum].robberyType, robberies[currentEventNum].locationNum);
    }

    public bool GetResult(int eventNum)
    {
        return (Random.Range(0f, 1f) < robberies[eventNum].chance);
    }

    public void ApplyChangesAfterChoice(int eventNum)
    {
        NightRobberyData rob = robberies[eventNum];
        NightEventButtonDetails bd = rob.nightEvent.rootNode.buttons[NightEventWindow.choice];

        rob.chance += bd.effect;
        rob.hospitalChance += bd.hospitalEffect;
        rob.policeChance += bd.policeEffect;
        rob.policeKnowledge += bd.policeKnowledge;
        rob.money += bd.money;
        rob.healthAffect += bd.healthAffect;

        if (bd.awards != null)
            foreach (int bKey in bd.awards.Keys)
            {
                if (rob.awards.ContainsKey(bKey)) rob.awards[bKey] += bd.awards[bKey];
                else rob.awards.Add(bKey, bd.awards[bKey]);
            }
    }
}
