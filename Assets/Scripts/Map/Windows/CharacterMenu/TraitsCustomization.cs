using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TraitsCustomization : MonoBehaviour
{
    public Text traitNameText;
    public Image image;
    public Text descriptionText;

    public void CustomizeTrait(int traitId)
    {
        Trait trait = TraitsOptions.GetTrait(traitId);
        traitNameText.text = trait.name;
        descriptionText.text = trait.description;
        image.sprite = WM1.traitsOptions.traitsSprites[traitId];
    }
}
