using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;


public partial class CharactersOptions : MonoBehaviour
{
    private static Dictionary<int, string> commonMaleNames;
    private static Dictionary<int, string> commonMaleHistories;
    private static Dictionary<int, string> commonFemaleNames;
    private static Dictionary<int, string> commonFemaleHistories;

    private static Dictionary<CharacterProperty, string> specialCharacter; //2
    private static Dictionary<int, Dictionary<CharacterProperty, string>> specialCharactersDict; //1
    private static List<Dictionary<int, Dictionary<CharacterProperty, string>>> specialCharactersAuthList; //0
    //(0)specialCharactersDict[auth level] -> (1)(id - character) -> (2)(stat - value)

    private static TextAsset CharactersCollectionDataXml;
    private static XmlDocument xmlDoc;

    public static void GetCharactersCollectionData()
    {
        CharactersCollectionDataXml = Resources.Load("CharactersCollectionData") as TextAsset;
        xmlDoc = new XmlDocument();
        if (CharactersCollectionDataXml)
        {
            xmlDoc.LoadXml(CharactersCollectionDataXml.text);
            XmlNode allCharacters = xmlDoc.SelectSingleNode("./charactersCollection");

            foreach (XmlNode collection in allCharacters)
            {
                if (collection.Name == "specialCharacters")
                {
                    specialCharactersAuthList = new List<Dictionary<int, Dictionary<CharacterProperty, string>>>();
                    foreach (XmlNode authority in collection)
                    {
                        specialCharactersDict = new Dictionary<int, Dictionary<CharacterProperty, string>>();
                        foreach (XmlNode character in authority)
                        {
                            specialCharacter = new Dictionary<CharacterProperty, string>();
                            foreach (XmlNode stat in character)
                            {
                                if (stat.Name == "name") specialCharacter.Add(CharacterProperty.name, stat.InnerText);
                                if (stat.Name == "spriteId") specialCharacter.Add(CharacterProperty.spriteId, stat.InnerText);
                                if (stat.Name == "traitId") specialCharacter.Add(CharacterProperty.traitId, stat.InnerText);
                                if (stat.Name == "history") specialCharacter.Add(CharacterProperty.history, stat.InnerText);
                            }
                            specialCharactersDict.Add(int.Parse(character.Attributes["id"].Value), specialCharacter);
                        }
                        specialCharactersAuthList.Add(specialCharactersDict);
                    }
                }
                else if (collection.Name == "commonMales")
                {
                    commonMaleNames = new Dictionary<int, string>();
                    commonMaleHistories = new Dictionary<int, string>();
                    foreach (XmlNode option in collection)
                    {
                        if (option.Name == "names")
                        {
                            foreach (XmlNode name in option)
                            {
                                commonMaleNames.Add(int.Parse(name.Attributes["id"].Value), name.InnerText);
                            }
                        }
                        if (option.Name == "histories")
                        {
                            foreach (XmlNode history in option)
                            {
                                commonMaleHistories.Add(int.Parse(history.Attributes["id"].Value), history.InnerText);
                            }
                        }
                    }
                }
                else if (collection.Name == "commonFemales")
                {
                    commonFemaleNames = new Dictionary<int, string>();
                    commonFemaleHistories = new Dictionary<int, string>();
                    foreach (XmlNode option in collection)
                    {
                        if (option.Name == "names")
                        {
                            foreach (XmlNode name in option)
                            {
                                commonFemaleNames.Add(int.Parse(name.Attributes["id"].Value), name.InnerText);
                            }
                        }
                        if (option.Name == "histories")
                        {
                            foreach (XmlNode history in option)
                            {
                                commonFemaleHistories.Add(int.Parse(history.Attributes["id"].Value), history.InnerText);
                            }
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Ошибка загрузки XML файла с данными об игровых персонажах!");
        }
    }
}
