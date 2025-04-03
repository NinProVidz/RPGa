using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public event EventHandler<TimeSpan> ClockChange;

    [SerializeField] public float dayLength;
   
    [SerializeField] public int counterTillDayChange = 0;
    [SerializeField] public int maxCounter = 2880;
    [SerializeField] public int counterReset = 0;

    Date date;

    [SerializeField] public TimeSpan currentTime;
    [SerializeField] public float minuteLength => dayLength / ClockConstant.MinutesInDay;

    private IEnumerator AddMinute()
    {
        currentTime += TimeSpan.FromMinutes(1);
        ClockChange?.Invoke(this, currentTime);
        counterTillDayChange++;
        yield return new WaitForSeconds(minuteLength);
        StartCoroutine(AddMinute());
    }
    private void Start()
    {
        date = FindObjectOfType<Date>();
        StartCoroutine(AddMinute());
    }

    private void Update()
    {
        if(counterTillDayChange == maxCounter)
        {
            date.day++;
            counterTillDayChange = counterReset;
        }
    }
}
