using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PM : MonoBehaviour
{
    public GameObject hospitalObject;
    public GameObject policeStationObject;
    public GameObject blackMarketObject;
    public GameObject banditCampObject;
    public GameObject testPanelObject;

    public static Button hospital;
    public static Button policeStation;
    public static Button blackMarket;
    public static Button banditCamp;
    public static Button testPanel;

    private void Awake()
    {
        hospital = hospitalObject.GetComponentInChildren<Button>();
        policeStation = policeStationObject.GetComponent<Button>();
        blackMarket = blackMarketObject.GetComponent<Button>();
        banditCamp = banditCampObject.GetComponent<Button>();
        testPanel = testPanelObject.GetComponent<Button>();
    }

    public static void SetActiveAllPlaces(bool status)
    {
        hospital.interactable = status;
        policeStation.interactable = status;
        blackMarket.interactable = status;
        banditCamp.interactable = status;
        testPanel.interactable = status;
    }
}
