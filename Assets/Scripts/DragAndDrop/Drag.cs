using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour
{
    public static GameObject itemBeingDragged;

    public static bool isCustomerDragged;

    public Transform customerScrollRect;
    public Transform dragParent;

    public float holdTime;
    public float maxScrollVelocityInDrag;

    protected Transform startParent;

    protected ScrollRect scrollRect;

    protected float timer;

    protected bool isHolding;
    protected bool canDrag;
    protected bool isPointerOverGameObject;

    protected CanvasGroup canvasGroup;

    protected Vector3 startPos;

    public Transform StartParent
    {
        get { return startParent; }
    }

    public Vector3 StartPos
    {
        get { return startPos; }
    }

    protected bool isHorizontalScrollActive;
    protected bool isVerticalScrollActive;

    public virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        customerScrollRect = transform.parent.parent.parent.parent;
        dragParent = FindObjectOfType<Canvas>().transform;
    }

    // Use this for initialization
    public virtual void Start()
    {
        timer = holdTime;
    }

    // Update is called once per frame
    public virtual void DragUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.currentSelectedGameObject == gameObject)
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
                        gameObject.GetComponent<Animator>().SetTrigger("Dropped");
                        transform.SetParent(startParent);
                        transform.localPosition = startPos;
                    }
                    canDrag = false;
                    timer = holdTime;
                }
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

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOverGameObject = false;
    }

    public virtual IEnumerator Holding()
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

        gameObject.GetComponent<Animator>().SetTrigger("Dragged");

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
    }

    public void Reset()
    {
        isHolding = false;
        canDrag = false;
        isPointerOverGameObject = false;
    }
}
