using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropToRobberyWindow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    bool isEntered;


    public void OnPointerEnter(PointerEventData eventData)
    {
        isEntered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isEntered = false;
    }

    //void Update()
    public void DropToRobberyWindowUpdate()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (isEntered)
            {
                int locNum = WM1.robberyWindow.locationNum;
                RobberyType robType = WM1.robberyWindow.robType;
                if (Drag.itemBeingDragged.GetComponent<DragCharacter>())
                {
                    CharacterCustomization charCust;
                    Drop.DropObject<CharacterCustomization, DragCharacter>(out charCust);

                    if (charCust != null)
                        if (charCust.status == CharacterStatus.normal)
                            WM1.robberyWindow.TryToAddCharacterToRobbery(charCust.character, robType, locNum);
                }
                if (Drag.itemBeingDragged.GetComponent<DragItem>())
                {
                    ItemCustomization iCust;
                    DragItem dragHandler;
                    Drop.DropObject(out iCust, out dragHandler);
                    if (iCust != null)
                        if (dragHandler.StartParent.parent == WM1.itemsPanel.itemsLocation)
                            WM1.robberyWindow.TryToAddItemToRobbery(iCust.number, robType, locNum);
                }
                isEntered = false;
            }
            else
            {
                if (Drag.itemBeingDragged.GetComponent<DragCharacter>())
                {
                    DragCharacter dragHandler;
                    CharacterCustomization charCust;
                    Drop.DropObject(out charCust, out dragHandler);

                    if (charCust.status == CharacterStatus.robbery)
                        if (dragHandler.StartParent.parent == WM1.robberyWindow.GetComponent<RobberyWindow>().charactersLocation)
                            WM1.robberyWindow.RemoveCharacterFromRobberyAndUpdate(charCust.character, WM1.robberyWindow.robType, WM1.robberyWindow.locationNum);
                }
                if (Drag.itemBeingDragged.GetComponent<DragItem>())
                {
                    ItemCustomization iCust;
                    DragItem dragHandler;
                    Drop.DropObject(out iCust, out dragHandler);

                    if (dragHandler.StartParent.parent == WM1.robberyWindow.itemsLocation)
                        WM1.robberyWindow.TryToRemoveItemFromRobbery(iCust.number, WM1.robberyWindow.robType, WM1.robberyWindow.locationNum);
                }
                isEntered = false;
            }
        }
    }
}