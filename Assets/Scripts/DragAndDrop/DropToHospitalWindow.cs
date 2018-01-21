﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropToHospitalWindow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    bool isEntered;
    public GameObject hospital;
    //public Transform charactersLocation;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isEntered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isEntered = false;
    }

    public void DropToHospitalWindowUpdate()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (isEntered)
            {
                if (Drag.itemBeingDragged.GetComponent<DragCharacter>())
                {
                    CharacterCustomization charCust;
                    Drop.DropObject<CharacterCustomization, DragCharacter>(out charCust);

                    if (charCust != null)
                        if (charCust.status == CharacterStatus.normal)
                            hospital.GetComponent<OnHospitalOpen>().TryToAddCharacterToHospital(charCust);
                }
                isEntered = false;
            }
        }
    }
}