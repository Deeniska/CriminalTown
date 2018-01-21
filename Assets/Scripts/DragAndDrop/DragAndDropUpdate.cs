using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class DragAndDropUpdate : MonoBehaviour
{
    public GameObject hospitalButton;
    //public GameObject hospitalWindow;
    public GameObject robberies;
    

    public RM robUpdate;


    void Update ()
    {
        if (!NightButton.isNight)
        {
            //Update drop
            if (Drag.itemBeingDragged)
            {
                if (WM1.hospital.gameObject.activeInHierarchy)
                    WM1.hospital.GetComponent<DropToHospitalWindow>().DropToHospitalWindowUpdate();
                hospitalButton.GetComponent<DropToHospital>().DropToHospitalButtonUpdate();

                if (WM1.robberyWindow.gameObject.activeInHierarchy)
                    WM1.robberyWindow.GetComponent<DropToRobberyWindow>().DropToRobberyWindowUpdate();

                foreach (GameObject stall in robUpdate.stalls)
                    if (stall.GetComponent<RobberyCustomization>().isAvailable)
                        stall.GetComponent<DropToRobbery>().DropToRobberyUpdate();
                foreach (GameObject darkStreet in robUpdate.darkStreets)
                    if (darkStreet.GetComponent<RobberyCustomization>().isAvailable)
                        darkStreet.GetComponent<DropToRobbery>().DropToRobberyUpdate();
            }

            //Then update drag
            foreach (GameObject item in GameObject.FindGameObjectsWithTag("DraggableItem"))
                item.GetComponent<DragItem>().DragUpdate();
            foreach (GameObject character in GameObject.FindGameObjectsWithTag("DraggableCharacter"))
                character.GetComponent<DragCharacter>().DragUpdate();
        }
    }
}
