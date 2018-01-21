using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class contains references to all windows and panels in game
/// and several methods to work with them
/// </summary>
public class WM1 : MonoBehaviour
{
    public static WM1 wm1Instance;

    #region Managers
    public GameObject charactersOptionsObject;
    public GameObject itemsOptionsObject;
    public GameObject robberiesOptionsObject;
    public GameObject nightEventsOprionsObject;
    public GameObject traitsOptionsObject;

    public static CharactersOptions charactersOptions;
    public static ItemsOptions itemsOptions;
    public static RobberiesOptions robberiesOptions;
    public static NightEventsOptions nightEventsOptions;
    public static TraitsOptions traitsOptions;
    #endregion

    #region Windows
    public GameObject modalPanelObject;
    public GameObject robberyWindowObject;
    public GameObject characterMenuObject;
    public GameObject robberyItemsWindowObject;
    public GameObject policeStationObject;
    public GameObject hospitalObject;
    public GameObject blackMarketObject;
    public GameObject buyWindowObject;
    public GameObject banditCampObject;

    public static ModalPanel modalPanel;
    public static RobberyWindow robberyWindow;
    public static CharacterMenu characterMenu;
    public static RobberyItemsWindow robberyItemsWindow;
    public static OnPoliceStationOpen policeStation;
    public static OnHospitalOpen hospital;
    public static OnMarketOpen blackMarket;
    public static BuyWindow buyWindow;
    public static OnBanditCampOpen banditCamp;

    #region Night
    public GameObject nightEventWindowObject;
    public GameObject nightResumeWindowObject;

    public static NightEventWindow nightEventWindow;
    public static GameObject nightResumeWindow;
    #endregion
    #endregion

    #region Panels
    public GameObject charactersPanelObject;
    public GameObject itemsPanelObject;
    public GameObject nightButtonObject;


    public static OnCharactersPanelUpdate charactersPanel;
    public static OnItemsPanelUpdate itemsPanel;
    public static NightButton nightButton;
    #endregion



    private void Awake()
    {
        wm1Instance = gameObject.GetComponent<WM1>();

        charactersOptions = charactersOptionsObject.GetComponent<CharactersOptions>();
        itemsOptions = itemsOptionsObject.GetComponent<ItemsOptions>();
        robberiesOptions = robberiesOptionsObject.GetComponent<RobberiesOptions>();
        nightEventsOptions = nightEventsOprionsObject.GetComponent<NightEventsOptions>();
        traitsOptions = traitsOptionsObject.GetComponent<TraitsOptions>();

        modalPanel = modalPanelObject.GetComponent<ModalPanel>();
        robberyWindow = robberyWindowObject.GetComponent<RobberyWindow>();
        characterMenu = characterMenuObject.GetComponent<CharacterMenu>();
        robberyItemsWindow = robberyItemsWindowObject.GetComponent<RobberyItemsWindow>();
        policeStation = policeStationObject.GetComponent<OnPoliceStationOpen>();
        hospital = hospitalObject.GetComponent<OnHospitalOpen>();
        blackMarket = blackMarketObject.GetComponent<OnMarketOpen>();
        buyWindow = buyWindowObject.GetComponent<BuyWindow>();
        banditCamp = banditCampObject.GetComponent<OnBanditCampOpen>();

        charactersPanel = charactersPanelObject.GetComponent<OnCharactersPanelUpdate>();
        itemsPanel = itemsPanelObject.GetComponent<OnItemsPanelUpdate>();
        nightButton = nightButtonObject.GetComponent<NightButton>();

        nightEventWindow = nightEventWindowObject.GetComponent<NightEventWindow>();
        nightResumeWindow = nightResumeWindowObject;
    }

    private void Start()
    {
        charactersPanel.UpdateCharactersPanel();
        banditCamp.UpdateBanditCamp();
        hospital.UpdateHospital();
        policeStation.UpdatePoliceStationCharacters();
    }

    public void CloseAllDayWindows()
    {
        modalPanelObject.SetActive(false);
        robberyWindowObject.SetActive(false);
        characterMenuObject.SetActive(false);
        robberyItemsWindowObject.SetActive(false);
        policeStationObject.SetActive(false);
        hospitalObject.SetActive(false);
        blackMarketObject.SetActive(false);
        buyWindowObject.SetActive(false);
        banditCampObject.SetActive(false);
        nightResumeWindowObject.SetActive(false);
    }

    public static void SetActivePanels(bool status)
    {
        charactersPanel.SetActive(status);
        //SetActiveItemsPanel(false);

        foreach (GameObject item in itemsPanel.Items)
            item.transform.GetChild(0).GetComponent<Button>().interactable = status;
        nightButton.gameObject.GetComponent<Button>().interactable = status;
    }
}
