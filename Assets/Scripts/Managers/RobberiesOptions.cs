using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;


public enum RobberyType { darkStreet, stall, house, shop, band }
public enum RobberyProperty
{
    description,
    name,
    descriptionFull,
    strenghtInfluence,
    agilityInfluence,
    skillInfluence,
    luckInfluence
}

[System.Serializable]
public class Robbery
{
    public int rang;

    public int strength;
    public int agility;
    public int skill;
    public int luck;

    //public List<CommonCharacter> commonCharacters;
    //public List<SpecialCharacter> specialCharacters;
    public Dictionary<int, int> itemsCount;
}

public class RobberiesOptions : MonoBehaviour
{
    public Sprite[] robberySprites = new Sprite[5];

    
    public static int typesAmount;
    public const int darkStreetsAmount = 3;
    public const int stallsAmount = 3;

    public static TextAsset RobberiesCollectionDataXml;
    public static XmlDocument xmlDoc;

    private static List<Dictionary<RobberyProperty, string>> robberiesCollection = new List<Dictionary<RobberyProperty, string>>();
    public static string GetRobberyData(RobberyType robberyType, RobberyProperty robberyProperty)
    {
        return robberiesCollection[(int)robberyType][robberyProperty];
    }

    private static Dictionary<RobberyProperty, string> robberyDict;

    public static void GetNewRobberies()
    {
        if (DataScript.eData.robberiesData != null) DataScript.eData.robberiesData.Clear();

        DataScript.eData.robberiesData = new Dictionary<RobberyType, Dictionary<int, Robbery>>()
        {
            { RobberyType.darkStreet, new Dictionary<int, Robbery>() },
            { RobberyType.stall, new Dictionary<int, Robbery>() }
        };

        Robbery robbery0 = GetRandomRobbery(RobberyType.darkStreet);
        DataScript.eData.robberiesData[RobberyType.darkStreet].Add(0, robbery0);

        Robbery robbery1 = GetRandomRobbery(RobberyType.stall);
        DataScript.eData.robberiesData[RobberyType.stall].Add(0, robbery1);

        Robbery robbery2 = GetRandomRobbery(RobberyType.darkStreet);
        DataScript.eData.robberiesData[RobberyType.darkStreet].Add(1, robbery2);

        Robbery robbery3 = GetRandomRobbery(RobberyType.darkStreet);
        DataScript.eData.robberiesData[RobberyType.darkStreet].Add(2, robbery3);

        Robbery robbery4 = GetRandomRobbery(RobberyType.stall);
        DataScript.eData.robberiesData[RobberyType.stall].Add(1, robbery4);

        Robbery robbery5 = GetRandomRobbery(RobberyType.stall);
        DataScript.eData.robberiesData[RobberyType.stall].Add(2, robbery5);

    }

    public static Robbery GetRandomRobbery(RobberyType robberyType)
    {
        int rndRang = 0;

        switch (robberyType)
        {
            case RobberyType.darkStreet:
                rndRang = 1;
                break;
            case RobberyType.stall:
                rndRang = Random.Range(1, 3);
                break;
            case RobberyType.house:
                rndRang = Random.Range(2, 4);
                break;
            case RobberyType.shop:
                rndRang = Random.Range(3, 5);
                break;
            case RobberyType.band:
                rndRang = Random.Range(4, 6);
                break;
        }
        int[] stats = GetRandomRobberyStats(rndRang);
        Robbery randomRobbery = new Robbery
        {
            rang = rndRang,
            strength = stats[0],
            agility = stats[1],
            skill = stats[2],
            luck = stats[3],
            itemsCount = new Dictionary<int, int>()
        };
        return randomRobbery;
    }

    private static int[] GetRandomRobberyStats(int rang)
    {
        int[] stats = new int[4] { 0, 0, 0, 0 };
        for (int i = 0; i < rang; i++)
        {
            int[] randStats = CharactersOptions.GetRandomStats(CharactersOptions.GetRandomCharLevelAtCurrentMoment());
            stats[0] += randStats[0];
            stats[1] += randStats[1];
            stats[2] += randStats[2];
            stats[3] += Random.Range(0, 5);
        }
        return stats;
    }

    public static int GetRobberyMoneyRewardAtTheCurrentMoment(RobberyType robberyType)
    {
        return 1000;
    }

    public static Dictionary<int, int> GetRobberyAwardsAtTheCurrentMoment(RobberyType robberyType)
    {
        return new Dictionary<int, int> { { Random.Range(0, 3), Random.Range(0, 3) } };
    }

    public static float CalculatePreliminaryChance(RobberyType robberyType, int locationNum)
    {
        //Avoid NULL REFERENCE
        if (!DataScript.eData.robberiesData.ContainsKey(robberyType))
        {
            Debug.LogWarning("Ошибка! Данного ограбления нет в базе");
            return 0;
        }
        else if (!DataScript.eData.robberiesData[robberyType].ContainsKey(locationNum))
        {
            Debug.LogWarning("Ошибка! Данного ограбления нет в базе");
            return 0;
        }

        float chance;
        Robbery robbery = DataScript.eData.robberiesData[robberyType][locationNum];
        List<Trait> chanceTraits;

        int rStrength = robbery.strength;
        int rAgility = robbery.agility;
        int rSkill = robbery.skill;
        int rLuck = robbery.luck;

        float banditsStats = CalculateBanditsStats(robberyType, locationNum, out chanceTraits);
        float robberyStats = rStrength + rAgility + rSkill + rLuck;
        float equipmentStats = CalcucateEquipmentStats(robberyType, locationNum);

        Debug.Log(robberyType + " " + locationNum);
        Debug.Log("banditsStats: " + banditsStats);
        Debug.Log("robberyStats: " + robberyStats);
        Debug.Log("equipmentStats: " + equipmentStats);

        chance = (banditsStats + equipmentStats) / (banditsStats + equipmentStats + robberyStats);

        Debug.Log(chance);

        foreach (Trait chanceTrait in chanceTraits)
        {
            switch (chanceTrait.stat)
            {
                case Stat.chance:
                    chance *= chanceTrait.value;
                    break;
                default:
                    break;
            }
        }

        chance = Random.Range(0, 100);
        return chance;
    }

    private static float CalculateBanditsStats(RobberyType robberyType, int locationNum, out List<Trait> chanceTraits)
    {
        int count = 0;

        chanceTraits = new List<Trait>();
        List<Trait> groupTraits = new List<Trait>();

        float cStrength = 0;
        float cAgility = 0;
        float cSkill = 0;
        float cLuck = 0;
        float cFear = 0;

        foreach (Character character in DataScript.eData.GetCharactersForRobbery(robberyType, locationNum))
        {
            count++;
            float coefStr = 1, coefAg = 1, coefSk = 1, coefL = 1, coefF = 1;

            if (character.GetType() == typeof(SpecialCharacter))
            {
                SpecialCharacter spChar = (SpecialCharacter)character;
                foreach (int traitId in spChar.TraitIds)
                {
                    Trait tempTrait = TraitsOptions.GetTrait(traitId);

                    //При редактировании трейтов ДОБАВИТЬ ИХ СЮДА!!!!!
                    switch (tempTrait.traitType)
                    {
                        case TraitType.single:
                            switch (tempTrait.stat)
                            {
                                case Stat.strenght:
                                    coefStr = tempTrait.value;
                                    break;
                                case Stat.luck:
                                    coefL = tempTrait.value;
                                    break;
                                case Stat.fear:
                                    coefF = tempTrait.value;
                                    break;
                                case Stat.skill:
                                    coefSk = tempTrait.value;
                                    break;
                                case Stat.agility:
                                    coefAg = tempTrait.value;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case TraitType.group:
                            groupTraits.Add(tempTrait);
                            break;
                        case TraitType.chance:
                            chanceTraits.Add(tempTrait);
                            break;
                    }
                }
            }

            cStrength += (character.Strength * coefStr);
            cAgility += (character.Agility * coefAg);
            cSkill += (character.Skill * coefSk);
            cLuck += (character.Luck * coefL);
            cFear += (character.Fear * coefF);
        }

        foreach (Trait groupTrait in groupTraits)
        {
            switch (groupTrait.stat)
            {
                case Stat.strenght:
                    cStrength *= groupTrait.value;
                    break;
                case Stat.luck:
                    cLuck *= groupTrait.value;
                    break;
                case Stat.fear:
                    cFear *= groupTrait.value;
                    break;
                case Stat.skill:
                    cSkill *= groupTrait.value;
                    break;
                case Stat.agility:
                    cAgility *= groupTrait.value;
                    break;
                default:
                    break;
            }
        }

        return
            (cStrength * float.Parse(GetRobberyData(robberyType, RobberyProperty.strenghtInfluence)) +
            cLuck * float.Parse(GetRobberyData(robberyType, RobberyProperty.luckInfluence)) +
            cAgility * float.Parse(GetRobberyData(robberyType, RobberyProperty.agilityInfluence)) +
            cSkill * float.Parse(GetRobberyData(robberyType, RobberyProperty.skillInfluence)))
            *
            (1 - cFear / (110*count));
    }

    private static float CalcucateEquipmentStats(RobberyType robberyType, int locationNum)
    {
        Dictionary<int, int> items = DataScript.eData.robberiesData[robberyType][locationNum].itemsCount;

        float itemsStats = 0;

        foreach (int itemNum in items.Keys)
        {
            switch (robberyType)
            {

                case RobberyType.darkStreet:
                    itemsStats += items[itemNum] * float.Parse(ItemsOptions.GetItemData(itemNum)[ItemProperty.influence0]);
                    break;
                case RobberyType.stall:
                    itemsStats += items[itemNum] * float.Parse(ItemsOptions.GetItemData(itemNum)[ItemProperty.influence1]);
                    break;
                case RobberyType.house:
                    itemsStats += items[itemNum] * float.Parse(ItemsOptions.GetItemData(itemNum)[ItemProperty.influence2]);
                    break;
                case RobberyType.shop:
                    itemsStats += items[itemNum] * float.Parse(ItemsOptions.GetItemData(itemNum)[ItemProperty.influence3]);
                    break;
                case RobberyType.band:
                    itemsStats += items[itemNum] * float.Parse(ItemsOptions.GetItemData(itemNum)[ItemProperty.influence4]);
                    break;
            }
        }

        return itemsStats;
    }

    public static void GetRobberiesCollectionData()
    {
        RobberiesCollectionDataXml = Resources.Load("RobberiesCollectionData") as TextAsset;

        xmlDoc = new XmlDocument();
        if (RobberiesCollectionDataXml)
        {
            xmlDoc.LoadXml(RobberiesCollectionDataXml.text);
            XmlNode allRobberies = xmlDoc.SelectSingleNode("./robberies");
            foreach (XmlNode robbery in allRobberies)
            {
                robberyDict = new Dictionary<RobberyProperty, string>();
                XmlNodeList robOptions = robbery.ChildNodes;
                foreach (XmlNode rOption in robOptions)
                {
                    if (rOption.Name == "name") robberyDict.Add(RobberyProperty.name, rOption.InnerText);
                    if (rOption.Name == "description") robberyDict.Add(RobberyProperty.description, rOption.InnerText);
                    if (rOption.Name == "descriptionFull") robberyDict.Add(RobberyProperty.descriptionFull, rOption.InnerText);
                    if (rOption.Name == "strenghtInfluence") robberyDict.Add(RobberyProperty.strenghtInfluence, rOption.InnerText);
                    if (rOption.Name == "agilityInfluence") robberyDict.Add(RobberyProperty.agilityInfluence, rOption.InnerText);
                    if (rOption.Name == "skillInfluence") robberyDict.Add(RobberyProperty.skillInfluence, rOption.InnerText);
                    if (rOption.Name == "luckInfluence") robberyDict.Add(RobberyProperty.luckInfluence, rOption.InnerText);
                }
                robberiesCollection.Add(robberyDict);
                typesAmount++;
            }
        }
        else
        {
            Debug.LogError("Ошибка загрузки XML файла с данными об ограблениях!");
        }
    }
}
