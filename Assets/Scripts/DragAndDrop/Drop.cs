using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Drop
{
    /// <summary>
    /// Set object position after dropping
    /// </summary>
    /// <param name="customization">Reference to customiation instance</param>
    /// <typeparam name="TCust">Object customization script</typeparam>
    /// <typeparam name="TDrag">Drag-script for object</typeparam>
    public static void DropObject<TCust, TDrag>(out TCust customization) where TDrag : Drag
    {
        GameObject draggedItem = Drag.itemBeingDragged;
        TDrag dragHandler = draggedItem.GetComponent<TDrag>();

        draggedItem.GetComponent<Animator>().SetTrigger("Dropped");


        draggedItem.transform.SetParent(dragHandler.StartParent);
        draggedItem.transform.localPosition = dragHandler.StartPos;
        dragHandler.GetComponent<CanvasGroup>().blocksRaycasts = true;

        customization = dragHandler.StartParent.GetComponent<TCust>();
    }

    /// <summary>
    /// Set object position after dropping
    /// </summary>
    /// <param name="customization">Reference to Customiation instance</param>
    /// <param name="itemDragHandler">Reference to Drag handler instance</param>
    /// <typeparam name="TCust">Object customization script</typeparam>
    /// <typeparam name="TDrag">Drag-script for object</typeparam>
    public static void DropObject<TCust, TDrag>(out TCust customization, out TDrag itemDragHandler) where TDrag : Drag
    {
        GameObject draggedItem = Drag.itemBeingDragged;
        TDrag dragHandler = draggedItem.GetComponent<TDrag>();

        draggedItem.GetComponent<Animator>().SetTrigger("Dropped");


        draggedItem.transform.SetParent(dragHandler.StartParent);
        draggedItem.transform.localPosition = dragHandler.StartPos;
        dragHandler.GetComponent<CanvasGroup>().blocksRaycasts = true;

        customization = dragHandler.StartParent.GetComponent<TCust>();
        itemDragHandler = dragHandler;
    }
}
