using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;


[Serializable]
public class SData
{
    public int[] itemsCount;
    public int money;
}

[Serializable]
public class ChData
{
    public List<Character> panelCharacters;
    //public List<CommonCharacter> panelComCharacters, campComCharacters;
    //public List<SpecialCharacter> panelSpCharacters, campSpCharacters;
    public List<Character> campCharacters;

    public void RemoveCharacter(Character character)
    {
        character.CallOnKickEvent();
        panelCharacters.Remove(character);
    }

    public void AddCharacter(Character character)
    {

    }
}

[Serializable]
public class PData
{
    public bool[] isItemAvailable;
    public bool isBlackMarketAvailable;
    public bool isBanditCampAvailable;
    public bool isHospitalAvailable;
    public bool isPoliceStationAvailable;
    public bool isRobberyAvailable; //В дальнейшем добавить виды ограблений

    public int authority;
    public int day;
}

[Serializable]
public class EData
{
    public int policeKnowledge;

    public Dictionary<RobberyType, Dictionary<int, Robbery>> robberiesData;

    //public IEnumerable<CommonCharacter> GetCommonCharactersForRobbery(RobberyType robberyType, int locationNum)
    //{
    //    foreach (CommonCharacter comChar in DataScript.chData.panelComCharacters)
    //    {
    //        if (comChar.Status == CharacterStatus.robbery)
    //            if (comChar.LocationNum == locationNum && comChar.RobberyType == (int)robberyType)
    //                yield return comChar;
    //    }
    //}
    //public IEnumerable<SpecialCharacter> GetSpecialCharactersForRobbery(RobberyType robberyType, int locationNum)
    //{
    //    foreach (SpecialCharacter spChar in DataScript.chData.panelSpCharacters)
    //    {
    //        if (spChar.Status == CharacterStatus.robbery)
    //            if (spChar.LocationNum == locationNum && spChar.RobberyType == (int)robberyType)
    //                yield return spChar;
    //    }
    //}
    public IEnumerable<Character> GetCharactersForRobbery(RobberyType robberyType, int locationNum)
    {
        foreach (Character character in DataScript.chData.panelCharacters)
        {
            if (character.Status == CharacterStatus.robbery)
                if (character.LocationNum == locationNum && character.RobberyType == (int)robberyType)
                    yield return character;
        }
    }


    public bool IsRobberyEmpty(RobberyType robberyType, int locationNum)
    {
        return DataScript.eData.GetCharactersForRobbery(robberyType, locationNum).GetEnumerator().MoveNext();
    }
}

public class DataScript : MonoBehaviour
{
    public static SData sData = new SData();
    public static ChData chData = new ChData
    {
        panelCharacters = new List<Character>(),
        campCharacters = new List<Character>(),
    };
    public static PData pData = new PData();
    public static EData eData = new EData();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        ItemsOptions.GetItemsCollectionData();
        CharactersOptions.GetCharactersCollectionData();
        RobberiesOptions.GetRobberiesCollectionData();
        NightEventsOptions.GetNightEventsCollectionData();
        TraitsOptions.GetTraitsCollectionData();
    }

    public static bool CheckDataFiles()
    {
        return
            (File.Exists(Application.persistentDataPath + "/sourcesDataFile.dat") &&
            File.Exists(Application.persistentDataPath + "/charactersDataFile.dat") &&
            File.Exists(Application.persistentDataPath + "/progressDataFile.dat")) &&
            File.Exists(Application.persistentDataPath + "/eventsDataFile.dat");
    }

    public static void AssignDefaultData()
    {
        //sData
        sData.itemsCount = new int[ItemsOptions.totalAmount];
        for (int i = 0; i < ItemsOptions.totalAmount; i++) sData.itemsCount[i] = 5;
        sData.money = 1000000;

        //chData
        CommonCharacter arrestedChar = CharactersOptions.GetRandomCommonCharacter(5);
        arrestedChar.Status = CharacterStatus.arrested;
        arrestedChar.StatusValue = 62;
        chData.panelCharacters.Add(arrestedChar);

        CommonCharacter hospitalChar = CharactersOptions.GetRandomCommonCharacter(6);
        hospitalChar.Status = CharacterStatus.hospital;
        hospitalChar.StatusValue = 20;
        chData.panelCharacters.Add(hospitalChar);

        chData.panelCharacters.Add(CharactersOptions.GetRandomCommonCharacter(8));
        chData.panelCharacters.Add(CharactersOptions.GetRandomCommonCharacter(9));
        chData.panelCharacters.Add(CharactersOptions.GetSpecialCharacter(9, 0));
        chData.panelCharacters.Add(CharactersOptions.GetSpecialCharacter(9, 1));

        //eData
        eData.policeKnowledge = 0;

        RobberiesOptions.GetNewRobberies();

        //pData
        pData.isItemAvailable = new bool[ItemsOptions.totalAmount];
        for (int i = 0; i < ItemsOptions.totalAmount; i++) pData.isItemAvailable[i] = true;
        pData.authority = 9;

        CharactersOptions.FillCampCells();
        SaveAll();
    }

    public static void LoadData()
    {
        sData = (SData)LoadData("/sourcesDataFile.dat");
        chData = (ChData)LoadData("/charactersDataFile.dat");
        pData = (PData)LoadData("/progressDataFile.dat");
        eData = (EData)LoadData("/eventsDataFile.dat");
    }

    public static void SaveSourcesData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream dataFile = File.Create(Application.persistentDataPath + "/sourcesDataFile.dat");
        bf.Serialize(dataFile, sData);
        dataFile.Close();
    }

    public static void SaveCharactersData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream dataFile = File.Create(Application.persistentDataPath + "/charactersDataFile.dat");
        bf.Serialize(dataFile, chData);
        dataFile.Close();
    }

    public static void SaveProgressData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream dataFile = File.Create(Application.persistentDataPath + "/progressDataFile.dat");
        bf.Serialize(dataFile, pData);
        dataFile.Close();
    }

    public static void SaveEventsData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream dataFile = File.Create(Application.persistentDataPath + "/eventsDataFile.dat");
        bf.Serialize(dataFile, eData);
        dataFile.Close();
    }

    public static void SaveAll()
    {
        SaveCharactersData();
        SaveEventsData();
        SaveProgressData();
        SaveSourcesData();
    }

    private static object LoadData(string fileName)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream dataFile = File.Open(Application.persistentDataPath + fileName, FileMode.Open);
        object data = bf.Deserialize(dataFile);
        dataFile.Close();
        return data;
    }
}
