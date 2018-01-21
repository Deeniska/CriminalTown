using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmulatorOptions : MonoBehaviour
{
    public InputField PiclocksAmount;
    public InputField KnifesAmount;
    public InputField GunsAmount;
    public InputField ShotgunsAmount;
    public InputField ArmorAmount;

    public InputField PiclockInfluence;
    public InputField KnifeInfluence;
    public InputField GunInfluence;
    public InputField ShotgunInfluence;
    public InputField ArmorInfluence;

    public InputField StrenghtInfluence;
    public InputField AgilityInfluence;
    public InputField SkillInfluence;
    public InputField LuckInfluence;
    public InputField FearInfluence;

    public Text PlayerStrengthText;
    public Text PlayerAgilityText;
    public Text PlayerSkillText;
    public Text PlayerLuckText;
    public Text PlayerFearText;

    public Slider PlayerStrengthSlider;
    public Slider PlayerAgilitySlider;
    public Slider PlayerSkillSlider;
    public Slider PlayerLuckSlider;
    public Slider PlayerFearSlider;

    public Text RobberyStrengthText;
    public Text RobberyAgilityText;
    public Text RobberySkillText;
    public Text RobberyLuckText;
    public Text RobberyFearText;

    public Slider RobberyStrengthSlider;
    public Slider RobberyAgilitySlider;
    public Slider RobberySkillSlider;
    public Slider RobberyLuckSlider;
    public Slider RobberyFearSlider;

    public Text SuccessChance;
    public Button CalculateButton;

    float strenghtInfluence;
    float agilityInfluence;
    float skillInfluence;
    float luckInfluence;
    float fearInfluence;


    private void OnEnable()
    {
        transform.SetAsLastSibling();
    }

    public float CalcucatePlayerStats()
    {
        int strenght = (int)PlayerStrengthSlider.value;
        int agility = (int)PlayerAgilitySlider.value;
        int skill = (int)PlayerSkillSlider.value;
        int luck = (int)PlayerLuckSlider.value;
        int fear = (int)PlayerFearSlider.value;

        float.TryParse(StrenghtInfluence.text, out strenghtInfluence);
        float.TryParse(AgilityInfluence.text, out agilityInfluence);
        float.TryParse(SkillInfluence.text, out skillInfluence);
        float.TryParse(LuckInfluence.text, out luckInfluence);
        float.TryParse(FearInfluence.text, out fearInfluence);


        float PlayerStatsSum = strenght * strenghtInfluence + agility * agilityInfluence + skill * skillInfluence + luck * luckInfluence - fear * fearInfluence;
        Debug.Log("PLayer stats sum " + PlayerStatsSum);
        return PlayerStatsSum;
    }
    public float CalcucateRobberyStats()
    {
        int strenght = (int)RobberyStrengthSlider.value;
        int agility = (int)RobberyAgilitySlider.value;
        int skill = (int)RobberySkillSlider.value;
        int luck = (int)RobberyLuckSlider.value;
        int fear = (int)RobberyFearSlider.value;

        float.TryParse(StrenghtInfluence.text, out strenghtInfluence);
        float.TryParse(AgilityInfluence.text, out agilityInfluence);
        float.TryParse(SkillInfluence.text, out skillInfluence);
        float.TryParse(LuckInfluence.text, out luckInfluence);
        float.TryParse(FearInfluence.text, out fearInfluence);

        float RobberyStatsSum = strenght * strenghtInfluence + agility * agilityInfluence + skill * skillInfluence + luck * luckInfluence - fear * fearInfluence;
        Debug.Log("Robbery stats sum " + RobberyStatsSum);
        return RobberyStatsSum;
    }

    public float CalcucateEquipmentStats()
    {
        int piclocksAmount;
        int knifesAmount;
        int gunsAmount;
        int shotgunsAmount;
        int armorAmount;

        float piclockInfluence;
        float knifeInfluence;
        float gunInfluence;
        float shotgunInfluence;
        float armorInfluence;

        float.TryParse(PiclockInfluence.text, out piclockInfluence);
        float.TryParse(KnifeInfluence.text, out knifeInfluence);
        float.TryParse(GunInfluence.text, out gunInfluence);
        float.TryParse(ShotgunInfluence.text, out shotgunInfluence);
        float.TryParse(ArmorInfluence.text, out armorInfluence);

        int.TryParse(PiclocksAmount.text, out piclocksAmount);
        int.TryParse(KnifesAmount.text, out knifesAmount);
        int.TryParse(GunsAmount.text, out gunsAmount);
        int.TryParse(ShotgunsAmount.text, out shotgunsAmount);
        int.TryParse(ArmorAmount.text, out armorAmount);

        float EquipmentStats = piclocksAmount * piclockInfluence + knifesAmount * knifesAmount + gunsAmount * gunInfluence + shotgunsAmount * shotgunInfluence + armorAmount * armorInfluence;
        Debug.Log("Equipment stats sum " + EquipmentStats);
        return EquipmentStats;
    }
    public void OnSliderValueChanged()
    {
        PlayerStrengthText.text = PlayerStrengthSlider.value.ToString();
        PlayerAgilityText.text = PlayerAgilitySlider.value.ToString();
        PlayerSkillText.text = PlayerSkillSlider.value.ToString();
        PlayerLuckText.text = PlayerLuckSlider.value.ToString();
        PlayerFearText.text = PlayerFearSlider.value.ToString();

        RobberyStrengthText.text = RobberyStrengthSlider.value.ToString();
        RobberyAgilityText.text = RobberyAgilitySlider.value.ToString();
        RobberySkillText.text = RobberySkillSlider.value.ToString();
        RobberyLuckText.text = RobberyLuckSlider.value.ToString();
        RobberyFearText.text = RobberyFearSlider.value.ToString();
    }
    public void OnCalculateButtonClick()
    {
        float successChance = (CalcucatePlayerStats() + CalcucateEquipmentStats()) / (CalcucatePlayerStats() + CalcucateEquipmentStats() + CalcucateRobberyStats());
        SuccessChance.text = "Шанс успеха:" + successChance.ToString();
    }
}
