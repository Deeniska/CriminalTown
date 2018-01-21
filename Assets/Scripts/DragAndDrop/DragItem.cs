using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragItem : Drag, IPointerExitHandler
{

    public static int itemNumber;


    // Update is called once per frame
    public override void DragUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.currentSelectedGameObject == gameObject)// && transform.parent.GetComponent<CharacterCustomization>().status == Status.normal)
            {
                //Debug.Log("Mouse Button Down");
                scrollRect = customerScrollRect.GetComponent<ScrollRect>();
                isPointerOverGameObject = true;
                isHolding = true;
                StartCoroutine(Holding());
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (EventSystem.current.currentSelectedGameObject == gameObject)
            {
                //Debug.Log("Mouse Button Up");
                isHolding = false;
                if (isVerticalScrollActive) customerScrollRect.GetComponent<ScrollRect>().vertical = true;
                if (isHorizontalScrollActive) customerScrollRect.GetComponent<ScrollRect>().horizontal = true;

                if (canDrag)
                {
                    itemBeingDragged = null;
                    isCustomerDragged = false;
                    if (transform.parent == dragParent)
                    {
                        canvasGroup.blocksRaycasts = true;
                        transform.SetParent(startParent);
                        transform.localPosition = startPos;
                    }
                    canDrag = false;
                    timer = holdTime;
                }
                //WM1.itemsPanel.UpdateSingleItemWithAnimation(itemNumber);
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (EventSystem.current.currentSelectedGameObject == gameObject)
            {
                if (canDrag)
                {
                    //Debug.Log("Mouse Button");
                    transform.position = Input.mousePosition;
                }
                else
                {
                    if (!isPointerOverGameObject)
                    {
                        isHolding = false;
                    }
                }
            }
        }
    }


    public override IEnumerator Holding()
    {
        while (timer > 0)
        {
            if (scrollRect.velocity.x >= maxScrollVelocityInDrag)
            {
                isHolding = false;
            }

            if (!isHolding)
            {
                timer = holdTime;
                yield break;
            }

            timer -= Time.deltaTime;
            //Debug.Log("Time : " + timer);
            yield return null;
        }

        if (customerScrollRect.GetComponent<ScrollRect>().vertical == true)
        {
            customerScrollRect.GetComponent<ScrollRect>().vertical = false;
            isVerticalScrollActive = true;
        }
        else isVerticalScrollActive = false;

        if (customerScrollRect.GetComponent<ScrollRect>().horizontal == true)
        {
            customerScrollRect.GetComponent<ScrollRect>().horizontal = false;
            isHorizontalScrollActive = true;
        }
        else isHorizontalScrollActive = false;

        isCustomerDragged = true;
        itemBeingDragged = gameObject;
        startPos = transform.localPosition;
        startParent = transform.parent;
        canDrag = true;
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(dragParent);
        itemNumber = startParent.GetComponent<ItemCustomization>().number;
    }
}