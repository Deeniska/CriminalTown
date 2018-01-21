using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;


//using ItemProperty = System.String;

public enum ItemProperty
{
    influence0 = 0,
    influence1 = 1,
    influence2 = 2,
    influence3 = 3,
    influence4 = 4,
    isSpecial,
    name,
    description,
    price
}

public enum ItemName
{
    lockpick = 0,
    knife = 1,
    pistol = 2,
    shotgun = 3
}

public class ItemsOptions : MonoBehaviour
{
    static TextAsset ItemsCollectionDataXml;
    static XmlDocument xmlDoc;


    public static int totalAmount = 0;


    public Sprite[] itemsSprites = new Sprite[totalAmount];

    private static Dictionary<int, Dictionary<ItemProperty, string>> itemsCollection = new Dictionary<int, Dictionary<ItemProperty, string>>();
    private static Dictionary<ItemProperty, string> itemDict;


    public static Dictionary<ItemProperty, string> GetItemData(int num)
    {
        return itemsCollection[num];
    }



    public static void GetItemsCollectionData()
    {
        ItemsCollectionDataXml = Resources.Load("ItemsCollectionData") as TextAsset;

        xmlDoc = new XmlDocument();
        if (ItemsCollectionDataXml)
        {
            xmlDoc.LoadXml(ItemsCollectionDataXml.text);
            XmlNode allItems = xmlDoc.SelectSingleNode("./items");
            foreach (XmlNode item in allItems)
            {
                itemDict = new Dictionary<ItemProperty, string>
                {
                    { ItemProperty.isSpecial, item.Attributes["isSpecial"].Value }
                };
                XmlNodeList itemOptions = item.ChildNodes;
                foreach (XmlNode iOption in itemOptions)
                {
                    if (iOption.Name == "name") itemDict.Add(ItemProperty.name, iOption.InnerText);
                    if (iOption.Name == "price") itemDict.Add(ItemProperty.price, iOption.InnerText);
                    if (iOption.Name == "influence") itemDict.Add((ItemProperty)System.Enum.Parse(typeof(ItemProperty), iOption.Attributes["place"].Value), iOption.InnerText);
                }
                itemsCollection.Add(int.Parse(item.Attributes["number"].Value), itemDict);
                totalAmount++;
            }
        }
        else
        {
            Debug.LogError("Ошибка загрузки XML файла с данными об игровых предметах!");
        }
    }
}
