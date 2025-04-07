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
    private DateTime currentLightTime;
    [SerializeField] public float minuteLength => dayLength / ClockConstant.MinutesInDay;

    [SerializeField] private float sunTimeMultiplier;
    [SerializeField] private float startHour;
    [SerializeField] private Light sunLight;
    [SerializeField] private float sunriseHour;
    [SerializeField] private float sunsetHour;
    [SerializeField] private TimeSpan sunriseTime;
    [SerializeField] private TimeSpan sunsetTime;
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

        currentLightTime = DateTime.Now.Date + TimeSpan.FromHours(startHour);
        sunriseTime = TimeSpan.FromHours(sunriseHour);
        sunsetTime = TimeSpan.FromHours(sunsetHour);

    }

    private void Update()
    {
        RotateSun();
        currentLightTime = currentLightTime.AddSeconds(Time.deltaTime * sunTimeMultiplier);
        if(counterTillDayChange >= maxCounter)
        {
            date.day++;
            date.namesIndex++;
            counterTillDayChange = counterReset;
        }
    }

    private void RotateSun()
    {
        float sunlightRotation;

        if(currentLightTime.TimeOfDay > sunriseTime && currentLightTime.TimeOfDay < sunsetTime)
        {
            TimeSpan sunriseToSunsetDuration = CalculateTimeDifference(sunriseTime, sunsetTime);
            TimeSpan timeSinceSunrise = CalculateTimeDifference(sunriseTime, currentLightTime.TimeOfDay);

            double percentage = timeSinceSunrise.TotalMinutes / sunriseToSunsetDuration.TotalMinutes;

            sunlightRotation = Mathf.Lerp(0, 180, (float)percentage);
        }
        else
        {
            TimeSpan sunsetToSunriseDuration = CalculateTimeDifference(sunsetTime, sunriseTime);
            TimeSpan timeSinceSunset = CalculateTimeDifference(sunsetTime, currentLightTime.TimeOfDay);

            double percentage = timeSinceSunset.TotalMinutes / sunsetToSunriseDuration.TotalMinutes;

            sunlightRotation = Mathf.Lerp(180, 360, (float)percentage);
        }
        sunLight.transform.rotation = Quaternion.AngleAxis(sunlightRotation, Vector3.right);
    }

    private TimeSpan CalculateTimeDifference(TimeSpan fromTime, TimeSpan toTime)
    {
        TimeSpan difference = toTime - fromTime;

        if(difference.TotalSeconds < 0)
        {
            difference += TimeSpan.FromHours(24);
        }

        return difference;
    }
}
