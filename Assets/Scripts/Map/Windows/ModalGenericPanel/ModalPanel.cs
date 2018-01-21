using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class EventButtonDetails
{
    public string buttonText;
    //public ColorBlock buttonColors;
    public UnityAction action;
}


public class ModalPanelDetails
{
    public string titletext;
    public string text;
    public Sprite iconSprite;
    public Sprite imageSprite;
    public EventButtonDetails button0Details;
    public EventButtonDetails button1Details;
}

public class ModalPanel : MonoBehaviour
{
    public Sprite wrongIcon;
    public Sprite checkIcon;
    public Sprite addIcon;
    public Sprite questionIcon;

    public Text titleText;
    public Image iconImage;
    public GameObject modalPanelObject;

    public Button button0;
    public Button button1;
    public Text button0Text;
    public Text button1Text;

    public Text text;
    public Image image;


    public void CallModalPanel(ModalPanelDetails details)
    {
        button0.gameObject.SetActive(false);
        button1.gameObject.SetActive(false);
        iconImage.gameObject.SetActive(false);
        image.gameObject.SetActive(false);


        titleText.text = details.titletext;
        text.text = details.text;

        if (details.imageSprite)
        {
            image.sprite = details.imageSprite;
            image.gameObject.SetActive(true);
        }

        if (details.iconSprite)
        {
            iconImage.sprite = details.iconSprite;
            iconImage.gameObject.SetActive(true);
        }

        if (details.button0Details != null)
        {
            button0.onClick.RemoveAllListeners();
            button0.onClick.AddListener(details.button0Details.action);
            button0.onClick.AddListener(ClosePanel);
            button0Text.text = details.button0Details.buttonText;
            button0.gameObject.SetActive(true);
        }

        if (details.button1Details != null)
        {
            button1.onClick.RemoveAllListeners();
            button1.onClick.AddListener(details.button1Details.action);
            button1.onClick.AddListener(ClosePanel);
            button1Text.text = details.button1Details.buttonText;
            button1.gameObject.SetActive(true);
        }
        modalPanelObject.transform.SetAsLastSibling();
        modalPanelObject.SetActive(true);
    }

    public void ClosePanel()
    {
        modalPanelObject.SetActive(false);
    }
}
