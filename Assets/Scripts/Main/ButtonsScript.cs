using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonsScript : MonoBehaviour
{
    public Button loadButton;

    private void Awake()
    {
        loadButton.gameObject.SetActive(DataScript.CheckDataFiles());
    }

    public void LoadGame()
    {
        DataScript.LoadData();
        SceneManager.LoadScene("Map");
    }

    public void NewGame()
    {
        EventButtonDetails yesButton = new EventButtonDetails
        {
            buttonText = "Да",
            action = () => { DataScript.AssignDefaultData(); SceneManager.LoadScene("Map"); }
        };
        EventButtonDetails noButton = new EventButtonDetails
        {
            buttonText = "Нет",
            action = WM0.modalPanel.ClosePanel
        };
        ModalPanelDetails details = new ModalPanelDetails
        {
            button0Details = yesButton,
            button1Details = noButton,
            iconSprite = WM0.modalPanel.questionIcon,
            text = "Вы уверены, что хотите начать новую игру?",
            titletext = "Новая игра"
        };
        WM0.modalPanel.CallModalPanel(details);
    }
}
