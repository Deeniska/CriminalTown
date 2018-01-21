using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketItemCustomization: MonoBehaviour
{
    public int number;
    public Image itemImage;
    public Text itemName;
    public Text itemPrice;


    public void OnMarketItemClick()
    {
        WM1.buyWindow.SetBuyWindow(number);
    }
}
