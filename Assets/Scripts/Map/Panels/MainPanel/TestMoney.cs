using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TestMoney : MonoBehaviour
{
    public InputField moneyInputField;

    private void Start()
    {
        moneyInputField.text = DataScript.sData.money.ToString();
    }

    private void Update()
    {
        moneyInputField.text = DataScript.sData.money.ToString();
    }

    public void OnEndEditMoney()
    {
        int.TryParse(moneyInputField.text, out DataScript.sData.money);
        DataScript.SaveSourcesData();
    }
}
