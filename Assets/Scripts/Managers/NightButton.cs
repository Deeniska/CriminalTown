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

        foreach (CommonCharacter comChar in DataScript.chData.panelComCharacters)
        {
            
            if (comChar.Status == CharacterStatus.arrested)
                if (comChar.DaysLeft < 2)
                {
                    ModalPanelDetails details = new ModalPanelDetails
                    {
                        button0Details = yesButton,
                        button1Details = noButton,
                        imageSprite = comChar.Sprite,
                        text = "Этот персонаж скоро нас сдаст. Босс, ты уверен, что стоит оставить его в грязных руках копов?",
                        titletext = comChar.Name
                    };
                    WM1.modalPanel.CallModalPanel(details);
                    return;
                }
        }
        foreach (SpecialCharacter spChar in DataScript.chData.panelSpCharacters)
        {

            if (spChar.Status == CharacterStatus.arrested)
                if (spChar.DaysLeft < 2)
                {
                    ModalPanelDetails details = new ModalPanelDetails
                    {
                        button0Details = yesButton,
                        button1Details = noButton,
                        imageSprite = WM1.charactersOptions.specialSprites[spChar.SpriteId],
                        text = "Этот персонаж скоро нас сдаст. Босс, ты уверен, что стоит оставить его в грязных руках копов?",
                        titletext = spChar.Name
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
                robbery.commonCharacters = new List<CommonCharacter>();
                robbery.specialCharacters = new List<SpecialCharacter>();

                foreach (CommonCharacter comChar in DataScript.eData.GetCommonCharactersForRobbery(robbery.robberyType, robbery.locationNum))
                    robbery.commonCharacters.Add(comChar);
                foreach (SpecialCharacter spChar in DataScript.eData.GetSpecialCharactersForRobbery(robbery.robberyType, robbery.locationNum))
                    robbery.specialCharacters.Add(spChar);
            }
    }

    private void UpdateDataAfterDay()
    {
        for (int i = 0; i < DataScript.chData.panelComCharacters.Count; i++)
        {
            CommonCharacter comChar = DataScript.chData.panelComCharacters[i];
            if (comChar.Status == CharacterStatus.hospital)
            {
                comChar.StatusValue += CharactersOptions.recoveryStep * comChar.BoostCoefficient;
                if (comChar.StatusValue >= 100)
                {
                    WM1.charactersPanel.commonCharacters[i].GetComponent<CharacterCustomization>().Animator.SetTrigger("Recovering");
                    comChar.Status = CharacterStatus.normal;
                    comChar.Health = 100;
                }
            }
            if (comChar.Status == CharacterStatus.arrested)
            {
                comChar.StatusValue -= comChar.Fear;
                if (comChar.StatusValue <= 0)
                {
                    DataScript.eData.policeKnowledge += 10;
                    WM1.charactersPanel.RemoveCharacter(false, i);
                }
            }
        }
        for (int i = 0; i < DataScript.chData.panelSpCharacters.Count; i++)
        {
            SpecialCharacter spChar = DataScript.chData.panelSpCharacters[i];
            if (spChar.Status == CharacterStatus.hospital)
            {
                spChar.StatusValue += CharactersOptions.recoveryStep * spChar.BoostCoefficient;
                if (spChar.StatusValue >= 100)
                {
                    WM1.charactersPanel.specialCharacters[i].GetComponent<CharacterCustomization>().Animator.SetTrigger("Recovering");
                    spChar.Status = CharacterStatus.normal;
                    spChar.Health = 100;
                }
            }
            if (spChar.Status == CharacterStatus.arrested)
            {
                spChar.StatusValue -= spChar.Fear;
                if (spChar.StatusValue <= 0)
                {
                    DataScript.eData.policeKnowledge += 10;
                    WM1.charactersPanel.RemoveCharacter(true, i);
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
            
            foreach (CommonCharacter comChar in robbery.commonCharacters)
            {
                if (comChar.Health <= 0)
                {

                }
                comChar.Status = CharacterStatus.normal;
            }
            foreach (SpecialCharacter spChar in robbery.specialCharacters)
                spChar.Status = CharacterStatus.normal;
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
