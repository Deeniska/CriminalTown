using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public enum EventStatus { success, fail, inProgress }

public class RobberyCustomization : MonoBehaviour
{
    public Image nightEvent;
    public Button nightEventButton;
    public Slider timerSlider;
    public Transform counter;
    public GameObject charCounterIconPrefab;

    private List<GameObject> charCounterIcons = new List<GameObject>();
    public void CounterMinus() { Destroy(charCounterIcons[0].gameObject); charCounterIcons.RemoveAt(0); }
    public void CounterPlus() { charCounterIcons.Add(Instantiate(charCounterIconPrefab, counter)); }

    public RobberyType robberyType;
    public int number;
    public bool isAvailable;



    public void OnClick()
    {
        WM1.robberyWindow.SetRobberyWindow(robberyType, number);
    }

    public void CustomizeRobbery(int num, RobberyType robType)
    {
        number = num;
        robberyType = robType;
        SetCounter();
    }

    private void SetCounter()
    {
        if (DataScript.eData.IsRobberyEmpty(robberyType, number))
        {
            foreach (Character character in DataScript.eData.GetCharactersForRobbery(robberyType, number))
                charCounterIcons.Add(Instantiate(charCounterIconPrefab, counter));
        }
    }

    public void AddNightEvent(UnityAction windowSettings, EventStatus eventStatus, float eventTime)
    {
        nightEventButton.onClick.AddListener(windowSettings);
        switch (eventStatus)
        {
            case EventStatus.success:
                nightEvent.color = Color.green;
                break;
            case EventStatus.fail:
                nightEvent.color = Color.red;
                break;
            case EventStatus.inProgress:
                nightEvent.color = Color.yellow;
                break;
        }

        StartCoroutine(Timer(eventTime));
        nightEvent.gameObject.SetActive(true);
    }

    private IEnumerator Timer(float eventTime)
    {
        float timer = eventTime;
        timerSlider.maxValue = eventTime;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            timerSlider.value = timer;
            yield return null;
        }
    }

    public void ResetNightButton()
    {
        nightEvent.gameObject.SetActive(false);
        nightEventButton.onClick.RemoveAllListeners();
    }
}
