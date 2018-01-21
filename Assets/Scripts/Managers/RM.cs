using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class RM : MonoBehaviour
{
    public static RM rmInstance;

    public RobberyCustomization GetRobberyCustomization(RobberyType robType, int number)
    {
        switch (robType)
        {
            case RobberyType.darkStreet:
                return darkStreets[number].GetComponent<RobberyCustomization>();
            case RobberyType.stall:
                return stalls[number].GetComponent<RobberyCustomization>();
            case RobberyType.house:
                return null;
            case RobberyType.shop:
                return null;
            case RobberyType.band:
                return null;
            default:
                return null;
        }
    }

    public GameObject[] darkStreets = new GameObject[RobberiesOptions.darkStreetsAmount];
    public GameObject[] stalls = new GameObject[RobberiesOptions.stallsAmount];

    private void Awake()
    {
        rmInstance = gameObject.GetComponent<RM>();
        CustomizeRobberies();

        /*Добавить виды ограблений:
         * 1. OnRobberiesUpdate -> Awake()
         * 2. OnRobberiesUpdate -> ActivateRobbery()
         * 3. OnRobberiesUpdate -> UpdateRobberies()
         * 4. Drag and Drop -> Update()
         */

        UpdateRobberies();
    }

    public void UpdateRobberies()
    {
        DeactivateAllRobberies();

        foreach (RobberyType robType in DataScript.eData.robberiesData.Keys)
            foreach (int locNum in DataScript.eData.robberiesData[robType].Keys)
                ActivateRobbery(robType, locNum);
    }

    public void DeactivateAllRobberies()
    {
        DeactivateRobberiesOfType(darkStreets);
        DeactivateRobberiesOfType(stalls);
    }

    private void DeactivateRobberiesOfType(GameObject[] robberies)
    {
        for (int i = 0; i < robberies.Length; i++)
        {
            robberies[i].GetComponentInChildren<Button>().interactable = false;
            robberies[i].GetComponent<RobberyCustomization>().isAvailable = false;
        }
    }

    public void CustomizeRobberies()
    {
        for (int i = 0; i < darkStreets.Length; i++) darkStreets[i].GetComponent<RobberyCustomization>().CustomizeRobbery(i, RobberyType.darkStreet);
        for (int i = 0; i < stalls.Length; i++) stalls[i].GetComponent<RobberyCustomization>().CustomizeRobbery(i, RobberyType.stall);
    }

    public void ActivateRobbery(RobberyType robType, int locationNum)
    {
        switch (robType)
        {
            case RobberyType.darkStreet:
                darkStreets[locationNum].GetComponentInChildren<Button>().interactable = true;
                darkStreets[locationNum].GetComponent<RobberyCustomization>().isAvailable = true;
                break;
            case RobberyType.stall:
                stalls[locationNum].GetComponentInChildren<Button>().interactable = true;
                stalls[locationNum].GetComponent<RobberyCustomization>().isAvailable = true;
                break;
            case RobberyType.house:
                break;
            case RobberyType.shop:
                break;
            case RobberyType.band:
                break;
        }
    }

    public void AddNightEvent(RobberyType robType, int locationNum, UnityAction windowSettings, EventStatus eventStatus, float eventTime)
    {
        switch (robType)
        {
            case RobberyType.darkStreet:
                darkStreets[locationNum].GetComponentInChildren<RobberyCustomization>().AddNightEvent(windowSettings, eventStatus, eventTime);
                break;
            case RobberyType.stall:
                stalls[locationNum].GetComponentInChildren<RobberyCustomization>().AddNightEvent(windowSettings, eventStatus, eventTime);
                break;
            case RobberyType.house:
                break;
            case RobberyType.shop:
                break;
            case RobberyType.band:
                break;
        }
    }

    public void ResetNightEvent(RobberyType robType, int locationNum)
    {
        switch (robType)
        {
            case RobberyType.darkStreet:
                darkStreets[locationNum].GetComponentInChildren<RobberyCustomization>().ResetNightButton();
                break;
            case RobberyType.stall:
                stalls[locationNum].GetComponentInChildren<RobberyCustomization>().ResetNightButton();
                break;
            case RobberyType.house:
                break;
            case RobberyType.shop:
                break;
            case RobberyType.band:
                break;
        }
    }
}
