using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class NightEventWindow : MonoBehaviour
{
    public static int choice;

    public Button button0;
    public Button button1;
    public Text button0Text;
    public Text button1Text;

    public Image background;

    public Text titleText;
    public Text descriptionText;
    public Image image;

    public GameObject awardsAndMoney;
    public Text moneyText;
    public Image moneyImage;
    public Image[] awardsImages = new Image[4];
    public Text[] awardsTexts = new Text[4];

    private void ShowNode(NightEventNode eventNode)
    {
        choice = -1;

        button0.gameObject.SetActive(false);
        button1.gameObject.SetActive(false);


        awardsAndMoney.gameObject.SetActive(false);

        titleText.text = eventNode.titleText;
        descriptionText.text = eventNode.description;
        image.sprite = NightEventsOptions.GetNightEventSprite(eventNode.spriteType, eventNode.spriteId, eventNode.charSpriteType);

        if (eventNode.buttons.Count > 0)
        {
            button0.onClick.RemoveAllListeners();
            button0.onClick.AddListener(() => { WM1.nightButton.MakeChoice(eventNode, 0); });
            button0.onClick.AddListener(CloseWindow);
            button0Text.text = eventNode.buttons[0].buttonText;
            button0.gameObject.SetActive(true);
        }
        if (eventNode.buttons.Count > 1)
        {
            button1.onClick.RemoveAllListeners();
            button1.onClick.AddListener(() => { WM1.nightButton.MakeChoice(eventNode, 1); });
            button1.onClick.AddListener(CloseWindow);
            button1Text.text = eventNode.buttons[1].buttonText;
            button1.gameObject.SetActive(true);
        }

        gameObject.transform.SetAsLastSibling();
        gameObject.SetActive(true);
    }

    public void ShowChoice(NightEventNode eventNode)
    {
        background.color = new Color(216f, 216f, 216f);
        ShowNode(eventNode);
    }

    public void ShowSuccess(NightEventNode successNode, Dictionary<int, int> awards, int money)
    {
        background.color = new Color(0, 255f, 0);
        ShowNode(successNode);

        int i = 0;
        foreach (int itemNum in awards.Keys)
        {
            if (awards[itemNum] > 0)
            {
                awardsImages[i].gameObject.SetActive(true);
                awardsImages[i].sprite = WM1.itemsOptions.itemsSprites[itemNum];
                awardsTexts[i].text = awards[itemNum].ToString();
            }
            else awardsImages[i].gameObject.SetActive(false);
            i++;
        }

        for (; i < awardsImages.Length; i++)
        {
            awardsImages[i].gameObject.SetActive(false);
        }

        moneyText.text = money.ToString();
        awardsAndMoney.SetActive(true);
    }

    public void ShowFail(NightEventNode failNode)
    {
        background.color = new Color(255f, 0, 0);
        ShowNode(failNode);
    }

    public void CloseWindow()
    {
        gameObject.SetActive(false);
    }
}
