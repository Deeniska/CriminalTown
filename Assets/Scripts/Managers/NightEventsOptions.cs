using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System;

public enum SpriteType { characters, items, robberies, people, places }
public enum CharacterSpriteType { comMale, comFemale, special }
//public enum EventStatus { success, fail, choice }

public class NightEventButtonDetails
{
    public string buttonText;
    public NightEventNode nextEventNode;
    public int effect;
    public int policeEffect;
    public int hospitalEffect;
    public int healthAffect;
    public int policeKnowledge;
    public Dictionary<int, int> awards;
    public int money;
}

public class NightEventNode
{
    public string titleText;
    public string description;
    public SpriteType spriteType;
    public CharacterSpriteType charSpriteType;
    public int spriteId;
    public List<NightEventButtonDetails> buttons;
}

public class NightEvent
{
    public NightEventNode rootNode;
    public NightEventNode success;
    public NightEventNode fail;
}

public class NightRobberyData
{
    public bool result;

    public RobberyType robberyType;
    public int locationNum;

    public NightEvent nightEvent;
    public int money;
    public Dictionary<int, int> awards;
    
    public float chance;
    public float hospitalChance;
    public float policeChance;
    public int healthAffect;
    public int policeKnowledge;

    public List<Character> characters;
    //public List<CommonCharacter> commonCharacters;
    //public List<SpecialCharacter> specialCharacters;
}

public class NightEventsOptions : MonoBehaviour
{
    public Sprite[] placesSprites = new Sprite[5];
    public Sprite[] peopleSprites = new Sprite[10];

    private static TextAsset nightEventsCollectionDataXml;
    private static XmlDocument xmlDoc;
    private static Dictionary<RobberyType, List<int>> usedEvents;

    public static Sprite GetNightEventSprite(SpriteType spriteType, int spriteId, CharacterSpriteType charSpriteType = 0)
    {
        switch (spriteType)
        {
            case SpriteType.characters:
                switch (charSpriteType)
                {
                    case CharacterSpriteType.comMale:
                        return WM1.charactersOptions.comMaleSprites[spriteId];
                    case CharacterSpriteType.comFemale:
                        return WM1.charactersOptions.comFemaleSprites[spriteId];
                    case CharacterSpriteType.special:
                        return WM1.charactersOptions.specialSprites[spriteId];
                    default:
                        return null;
                }
            case SpriteType.items:
                return WM1.itemsOptions.itemsSprites[spriteId];
            case SpriteType.robberies:
                return WM1.robberiesOptions.robberySprites[spriteId];
            case SpriteType.people:
                return WM1.nightEventsOptions.peopleSprites[spriteId];
            case SpriteType.places:
                return WM1.nightEventsOptions.placesSprites[spriteId];
            default:
                return null;
        }
    }

    public int GetCharacterSpriteId(RobberyType robberyType, int locationNum)
    {
        return 0;
    }

    public static NightEvent GetRandomEvent(RobberyType robberyType)
    {
        NightEvent nightEvent = new NightEvent();
        if (xmlDoc == null) GetNightEventsCollectionData();
        XmlNode eventsOfNeededType = xmlDoc.SelectSingleNode("./eventsCollection/robbery[@type='" + robberyType.ToString("d") + "']");

        int eventsCount = eventsOfNeededType.ChildNodes.Count;
        int rndEventNum = UnityEngine.Random.Range(0, eventsCount);

        //Avoid repetitions while it is possible
        if (usedEvents.ContainsKey(robberyType))
        {
            if (usedEvents[robberyType].Count < eventsCount)
                while (usedEvents[robberyType].Contains(rndEventNum))
                {
                    rndEventNum++;
                    if (rndEventNum >= eventsCount) rndEventNum = 0;
                }
            usedEvents[robberyType].Add(rndEventNum);
        }
        else
        {
            usedEvents.Add(robberyType, new List<int> { rndEventNum });
        }

        XmlNode eventRootNode = eventsOfNeededType.SelectSingleNode("./event[@id='" + rndEventNum.ToString() + "']");

        try
        {
            if (eventRootNode.SelectSingleNode("./node") != null) nightEvent.rootNode = ParseEventRandomTree(eventRootNode.SelectSingleNode("./node"), robberyType);
            nightEvent.success = ParseEventRandomTree(eventRootNode.SelectSingleNode("./success"), robberyType);
            nightEvent.fail = ParseEventRandomTree(eventRootNode.SelectSingleNode("./fail"), robberyType);
        }
        catch(FormatException)
        {
            Debug.LogWarning("Ошибка в XML-файле. RobberyType: " + robberyType + ", eventNum: " + rndEventNum);
            nightEvent = GetRandomEvent(robberyType);
        }
        catch(ArgumentException)
        {
            Debug.LogWarning("Ошибка в XML-файле. RobberyType: " + robberyType + ", eventNum: " + rndEventNum);
            nightEvent = GetRandomEvent(robberyType);
        }
        return nightEvent;
    }

    /// <summary>Get night event tree with random nodes from "NightEventsCollectionData.xml"</summary>
    /// <param name="eventNode">A root node of event in "NightEventsCollectionData.xml"</param>
    public static NightEventNode ParseEventRandomTree(XmlNode eventNode, RobberyType robberyType)
    {
        NightEventNode nightEvent = new NightEventNode();
        foreach (XmlNode info in eventNode)
        {
            //Debug.Log(info.Name + " " + info.InnerText);
            nightEvent.titleText = RobberiesOptions.GetRobberyData(robberyType, RobberyProperty.name);
            if (info.Name == "text") nightEvent.description = info.InnerText;
            if (info.Name == "sprite")
                foreach (XmlNode spriteInfo in info)
                {
                    if (spriteInfo.Name == "spriteType")
                    {
                        SpriteType sType = (SpriteType)Enum.Parse(typeof(SpriteType), spriteInfo.InnerText);
                        if (Enum.IsDefined(typeof(SpriteType), sType))
                            nightEvent.spriteType = sType;
                        else Debug.LogError("No \"" + sType + "\" sprite type exists! Check NightEventsCollectionData.xml or SpriteType enum");
                    };
                    if (spriteInfo.Name == "spriteId") nightEvent.spriteId = int.Parse(spriteInfo.InnerText);
                }
            if (info.Name == "button")
            {
                if (nightEvent.buttons == null) nightEvent.buttons = new List<NightEventButtonDetails>();
                NightEventButtonDetails button = new NightEventButtonDetails();
                foreach (XmlNode buttonInfo in info)
                {
                    if (buttonInfo.Name == "text") button.buttonText = buttonInfo.InnerText;
                    if (buttonInfo.Name == "effect") button.effect = int.Parse(buttonInfo.InnerText);
                    if (buttonInfo.Name == "policeEffect") button.policeEffect = int.Parse(buttonInfo.InnerText);
                    if (buttonInfo.Name == "hospitalEffect") button.hospitalEffect = int.Parse(buttonInfo.InnerText);
                    if (buttonInfo.Name == "healthAffect") button.healthAffect = int.Parse(buttonInfo.InnerText);
                    if (buttonInfo.Name == "policeKnowledge") button.policeKnowledge = int.Parse(buttonInfo.InnerText);
                    if (buttonInfo.Name == "award")
                    {
                        int itemId;
                        if (int.TryParse(buttonInfo.Attributes["itemId"].Value, out itemId))
                        {
                            if (button.awards == null) button.awards = new Dictionary<int, int>();
                            {
                                if (!button.awards.ContainsKey(itemId))
                                    button.awards.Add(itemId, int.Parse(buttonInfo.InnerText));
                                else button.awards[itemId] += int.Parse(buttonInfo.InnerText);
                            };
                        }
                    }
                    if (buttonInfo.Name == "money") button.money = int.Parse(buttonInfo.InnerText);
                }
                if (info.SelectSingleNode("./node") != null) //is button contains next nodes
                {
                    Debug.Log("COUNT: " + info.SelectNodes("./node").Count);
                    button.nextEventNode = ParseEventRandomTree(info.SelectNodes("./node")[UnityEngine.Random.Range(0, info.SelectNodes("./node").Count)], robberyType); //Recursion
                }
                nightEvent.buttons.Add(button);
            };
        }
        return nightEvent;
    }

    public static void GetNightEventsCollectionData()
    {
        if (xmlDoc == null)
        {
            nightEventsCollectionDataXml = Resources.Load("NightEventsCollectionData") as TextAsset;
            if (nightEventsCollectionDataXml)
            {
                xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(nightEventsCollectionDataXml.text);
                usedEvents = new Dictionary<RobberyType, List<int>>();
            }
            else Debug.LogError("Ошибка загрузки XML файла с данными о ночных событиях!");
        }
    }

    public static void ClearUsedEvents()
    {
        usedEvents.Clear();
    }
}