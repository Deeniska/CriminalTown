using System.Collections.Generic;
using UnityEngine;


public enum CharacterStatus { normal, robbery, hospital, arrested }
public enum CharacterProperty { name, history, spriteId, traitId }
public enum Sex { male, female }

[System.Serializable]
public class Character
{
    public Sex Sex { set; get; }

    public int Level { set; get; }
    public int Tiredness { set; get; }
    public int Health { set; get; }
    public int Exp { set; get; }
    public int Strength { set; get; }
    public int Agility { set; get; }
    public int Skill { set; get; }
    public int Fear { set; get; }
    public int Luck { set; get; }
    public int Points { set; get; }

    public CharacterStatus Status { set; get; }

    private int statusValueOrRobType;
    private int boostCoefOrLocNum;

    public int RobberyType
    {
        get { return statusValueOrRobType; }
        set { statusValueOrRobType = value; }
    }
    public int LocationNum
    {
        get { return boostCoefOrLocNum; }
        set { boostCoefOrLocNum = value; }
    }
    public int StatusValue
    {
        get { return statusValueOrRobType; }
        set { statusValueOrRobType = value; }
    }
    public int BoostCoefficient
    {
        get { return boostCoefOrLocNum; }
        set { boostCoefOrLocNum = value; }
    }
    public int DaysLeft
    {
        get
        {
            if (Status == CharacterStatus.hospital) return Mathf.CeilToInt((CharactersOptions.maxRecovery - StatusValue) / (float)(BoostCoefficient * CharactersOptions.recoveryStep));
            else if (Status == CharacterStatus.arrested) return Mathf.CeilToInt(StatusValue / (float)Fear);
            else return 0;
        }
    }

    public Character()
    {

    }

    public void SetStats(Character character)
    {
        Level = character.Level;
        Health = character.Health;
        Tiredness = character.Tiredness;
        Exp = character.Exp;
        Strength = character.Strength;
        Agility = character.Agility;
        Skill = character.Skill;
        Luck = character.Luck;
        Fear = character.Fear;
        Points = character.Points;

        Status = character.Status;
        statusValueOrRobType = character.statusValueOrRobType;
        boostCoefOrLocNum = character.boostCoefOrLocNum;
    }

    public Character GetStats()
    {
        return new Character()
        {
            Level = this.Level,
            Tiredness = this.Tiredness,
            Health = this.Health,
            Exp = this.Exp,
            Strength = this.Strength,
            Agility = this.Agility,
            Skill = this.Skill,
            Fear = this.Fear,
            Luck = this.Luck,
            Points = this.Points,

            Status = Status,
            statusValueOrRobType = statusValueOrRobType,
            boostCoefOrLocNum = boostCoefOrLocNum
        };
    }

    public void AddExperience(int expToAdd)
    {
        while (Exp + expToAdd > CharactersOptions.GetExperienceMaxValue(Level))
        {
            Health = CharactersOptions.maxHealth;
            Tiredness = 0;
            expToAdd -= (CharactersOptions.GetExperienceMaxValue(Level) - Exp);
            Exp = 0;
            Level++;
            Points++;
        }
        Exp += expToAdd;
    }

    public delegate void MethodContainer(Character character);

    public event MethodContainer OnKickEvent = delegate { };

    public event MethodContainer OnStatsChangedEvent = delegate { };

    public event MethodContainer OnAddEvent = delegate { };

    public void CallOnKickEvent() { OnKickEvent(this); }

    public void CallOnStatsChangedEvent() { OnStatsChangedEvent(this); }

    public void CallOnAddEvent() { OnAddEvent(this); }

    public virtual void LogCharacterData()
    {
        Debug.Log("level: " + Level);
        Debug.Log("health: " + Health);
        Debug.Log("strength: " + Strength);
        Debug.Log("agility: " + Agility);
        Debug.Log("skill: " + Skill);
        Debug.Log("fear: " + Fear);
        Debug.Log("tiredness: " + Tiredness);
        Debug.Log("luck: " + Luck);
        Debug.Log("exp: " + Exp);
        Debug.Log("points: " + Points);
    }
}

[System.Serializable]
public class SpecialCharacter : Character
{
    public int Authority { set; get; }
    public int Id { set; get; }
    public int SpriteId { set; get; }
    public List<int> TraitIds { set; get; }

    public string Name
    {
        get { return CharactersOptions.GetSpecialName(Authority, Id); }
    }
    public string History
    {
        get { return CharactersOptions.GetSpecialHistory(Authority, Id); }
    }
    public Sprite Sprite
    {
        get { return CharactersOptions.GetSpecialSprite(Id); }
    }

    public override void LogCharacterData()
    {
        base.LogCharacterData();
        Debug.Log("authority: " + Authority);
        Debug.Log("id: " + Id);
    }
}

[System.Serializable]
public class CommonCharacter : Character
{
    public int SpriteId { set; get; }
    public int NameId { set; get; }
    public int HistoryId { set; get; }

    public string Name
    {
        get { return CharactersOptions.GetCommonName((Sex)Sex, NameId); }
    }
    public string History
    {
        get { return CharactersOptions.GetCommonHistory((Sex)Sex, HistoryId); }
    }
    public Sprite Sprite
    {
        get { return CharactersOptions.GetCommonSprite((Sex)Sex, SpriteId); }
    }

    public override void LogCharacterData()
    {
        base.LogCharacterData();
        Debug.Log("\nspriteId: " + SpriteId);
        Debug.Log("nameId" + NameId);
        Debug.Log("historyId: " + HistoryId);
    }
}

public partial class CharactersOptions : MonoBehaviour
{
    #region CharactersSettings - constants
    public const int campComCellsAmount = 5;
    public const int campSpCellsAmount = 2;
    public const int panelCellsMaxAmount = 20;

    public const int maxStat = 10;
    public const int maxAuthority = 10;
    public const int maxTiredness = 100;
    public const int maxHealth = 100;
    public const int maxFear = 100;

    //Hospital
    public const int maxRecovery = 100;
    public const int recoveryStep = 10;
    public const int boostedCoef = 2;

    //PoliceStation
    public const int maxOpposition = 100;

    //Random
    public const int maxRndStat = 8;
    public const int levelsRndDispersion = 3;

    //Leveling
    public const int expStepByLevel = 20;
    public const float pointsPerLevelCoefficient = 1.2f;

    //Sprites
    public const int comMaleSpritesAmount = 10;
    public const int comFemaleSpritesAmount = 10;
    public const int specialSpritesAmount = 10;
    #endregion

    public Sprite[] specialSprites = new Sprite[specialSpritesAmount];
    public Sprite[] comMaleSprites = new Sprite[comMaleSpritesAmount];
    public Sprite[] comFemaleSprites = new Sprite[comFemaleSpritesAmount];


    #region Accessing methods
    public static string GetCommonName(Sex sex, int id)
    {
        switch (sex)
        {
            case Sex.male:
                return commonMaleNames[id];
            case Sex.female:
                return commonFemaleNames[id];
            default:
                return null;
        }
    }
    public static string GetCommonHistory(Sex sex, int id)
    {
        switch (sex)
        {
            case Sex.male:
                return commonMaleHistories[id];
            case Sex.female:
                return commonFemaleHistories[id];
            default:
                return null;
        }
    }
    public static Sprite GetCommonSprite(Sex sex, int id)
    {
        switch (sex)
        {
            case Sex.male:
                return WM1.charactersOptions.comMaleSprites[id];
            case Sex.female:
                return WM1.charactersOptions.comFemaleSprites[id];
            default:
                return null;
        }
    }

    public static string GetSpecialName(int authority, int id)
    {
        return specialCharactersAuthList[authority][id][CharacterProperty.name];
    }
    public static string GetSpecialHistory(int authority, int id)
    {
        return specialCharactersAuthList[authority][id][CharacterProperty.history];
    }
    public static Sprite GetSpecialSprite(int id)
    {
        return WM1.charactersOptions.specialSprites[id];
    }
    #endregion

    //public static void AddExperienceToChar(bool isSpecial, int charNum, int expToAdd)
    //{
    //    if (!isSpecial && charNum > DataScript.chData.panelComCharacters.Count - 1) Debug.LogError("Обычного персонажа с таким номером не существует");
    //    else if (isSpecial && charNum > DataScript.chData.panelSpCharacters.Count - 1) Debug.LogError("Уникального персонажа с таким номером не существует");
    //    else
    //    {
    //        Character chDat = !isSpecial ? DataScript.chData.panelComCharacters[charNum].Copy() :
    //            DataScript.chData.panelSpCharacters[charNum].Copy();

    //        while (chDat.Exp + expToAdd > GetExperienceMaxValue(chDat.Level))
    //        {
    //            if (chDat.Health < maxHealth) chDat.Health = maxHealth;
    //            if (chDat.Tiredness > 0) chDat.Tiredness = 0;
    //            expToAdd -= (GetExperienceMaxValue(chDat.Level) - chDat.Exp);
    //            chDat.Exp = 0;
    //            chDat.Level++;
    //            chDat.Points++;
    //        }
    //        chDat.Exp += expToAdd;
    //        if (!isSpecial) DataScript.chData.panelComCharacters[charNum].SetBaseData(chDat);
    //        else DataScript.chData.panelSpCharacters[charNum].SetBaseData(chDat);
    //    }
    //}
    //Доделать
    public static int[] GetRandomStats(int level)
    {
        int summ = (int)(pointsPerLevelCoefficient * level);
        int minStat = (int)(1f * summ / 6f);
        int maxStat = (int)(4f * summ / 9f);
        int[] stats = new int[3];

        int[] seq = new int[3] { 0, 1, 2 };
        for (int i = 2; i > 0; i--)
        {
            int temp;
            int j = Random.Range(0, i);
            temp = seq[i];
            seq[i] = seq[j];
            seq[j] = temp;
        }

        stats[seq[0]] = Random.Range(minStat, maxStat + 1);

        if (summ - stats[seq[0]] > maxStat) stats[seq[1]] = Random.Range(minStat, maxStat + 1);
        else if (summ - stats[seq[0]] < 0) stats[seq[1]] = 0;
        else stats[seq[1]] = Random.Range(minStat, summ - stats[seq[0]] + 1);

        if (summ < stats[seq[0]] + stats[seq[1]]) stats[seq[2]] = 0;
        else if (summ - stats[seq[0]] - stats[seq[1]] > maxStat) stats[seq[2]] = maxStat;
        else stats[seq[2]] = summ - stats[seq[0]] - stats[seq[1]];

        return stats;
    }

    public static int GetRandomCharLevelAtCurrentMoment()
    {
        int summ = 0;
        foreach (Character character in DataScript.chData.panelCharacters) summ += character.Level;
        int average = summ == 0 ? 0 : summ / DataScript.chData.panelCharacters.Count;
        return average<levelsRndDispersion? Random.Range(0, average + levelsRndDispersion) :
            Random.Range(average - levelsRndDispersion, average + levelsRndDispersion);
    }

public static int GetExperienceMaxValue(int level)
{
    return 100 + level * expStepByLevel;
}
//Доделать 
public static CommonCharacter GetRandomCommonCharacter(int level)
{
    CommonCharacter randomCharacter = new CommonCharacter();
    int sex = Random.Range(0, 2);
    if (sex == 0)
    {
        randomCharacter.HistoryId = Random.Range(0, commonMaleHistories.Count);
        randomCharacter.NameId = Random.Range(0, commonMaleNames.Count);
        randomCharacter.SpriteId = Random.Range(0, comMaleSpritesAmount);
    }
    if (sex == 1)
    {
        randomCharacter.HistoryId = Random.Range(0, commonFemaleHistories.Count);
        randomCharacter.NameId = Random.Range(0, commonFemaleNames.Count);
        randomCharacter.SpriteId = Random.Range(0, comFemaleSpritesAmount);
    }

    randomCharacter.Level = level;

    int[] rndStats = GetRandomStats(randomCharacter.Level);

    randomCharacter.Status = CharacterStatus.normal;
    randomCharacter.StatusValue = 0;
    randomCharacter.LocationNum = 1;

    randomCharacter.Sex = (Sex)sex;
    randomCharacter.Strength = rndStats[0];
    randomCharacter.Agility = rndStats[1];
    randomCharacter.Skill = rndStats[2];
    randomCharacter.Luck = Random.Range(0, 5);
    randomCharacter.Fear = Random.Range(25, 51);

    randomCharacter.Tiredness = Random.Range(0, maxTiredness / 2);
    randomCharacter.Exp = 0;
    randomCharacter.Points = 0;
    randomCharacter.Health = Random.Range(maxHealth * 1 / 2, maxHealth + 1);

    return randomCharacter;
}

public static SpecialCharacter GetSpecialCharacter(int authorityLevel, int charNum)
{
    int rndLevel = GetRandomCharLevelAtCurrentMoment() + levelsRndDispersion;
    int[] rndStats = GetRandomStats(rndLevel);
    //int rndId = Random.Range(0, specialCharactersAuthList[authorityLevel].Count);

    SpecialCharacter specialCharacter = new SpecialCharacter()
    {
        Status = CharacterStatus.normal,
        StatusValue = 0,
        LocationNum = 1,

        Level = rndLevel,
        Authority = authorityLevel,
        Id = charNum,
        SpriteId = int.Parse(specialCharactersAuthList[authorityLevel][charNum][CharacterProperty.spriteId]),
        TraitIds = new List<int>
            {
                int.Parse(specialCharactersAuthList[authorityLevel][charNum][CharacterProperty.traitId])
            },

        Strength = rndStats[0],
        Agility = rndStats[1],
        Skill = rndStats[2],
        Luck = Random.Range(5, 7),
        Fear = Random.Range(0, 26),

        Tiredness = Random.Range(0, maxTiredness / 4),
        Exp = 0,
        Points = 0,
        Health = Random.Range(maxHealth * 3 / 4, maxHealth + 1),
    };

    return specialCharacter;
}

public static int GetComPrice(int lvl)
{
    return 100 * lvl;
}

public static int GetSpPrice(int lvl)
{
    return 1000 * lvl;
}

public static int GetBoostRecoveryPrice(int lvl)
{
    return 20 * lvl;
}

public static int GetBreakOutPrice(int lvl)
{
    return 50 * lvl;
}

public static void FillCampCells()
{
    while (DataScript.chData.panelCharacters.Count < campComCellsAmount)
    {
        CommonCharacter randomComCharacter = GetRandomCommonCharacter(GetRandomCharLevelAtCurrentMoment());
        DataScript.chData.panelCharacters.Add(randomComCharacter);
    }
    while (DataScript.chData.panelCharacters.Count < campSpCellsAmount)
    {
        SpecialCharacter randomSpCharacter = GetSpecialCharacter(DataScript.pData.authority, 0);
        DataScript.chData.panelCharacters.Add(randomSpCharacter);
    }
    //DataScript.SaveCharactersData();
}
}
