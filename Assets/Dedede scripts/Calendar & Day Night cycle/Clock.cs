using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public event EventHandler<TimeSpan> ClockChange;

    [SerializeField] public float dayLength;

    private TimeSpan currentTime;
    private float minuteLength => dayLength / ClockConstant.MinutesInDay;

    private IEnumerator AddMinute()
    {
        currentTime += TimeSpan.FromMinutes(1);
        ClockChange?.Invoke(this, currentTime);
        yield return new WaitForSeconds(minuteLength);
        StartCoroutine(AddMinute());
    }
    private void Start()
    {
        StartCoroutine(AddMinute());
    }
}
