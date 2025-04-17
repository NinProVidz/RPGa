using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour, IDataPersistence
{
    public event EventHandler<TimeSpan> ClockChange;

    [SerializeField] public float dayLength;
   
    [SerializeField] public int counterTillDayChange;
    [SerializeField] public int maxCounter = 2880;
    [SerializeField] public int counterReset = 0;

    Date date;

    [SerializeField] public TimeSpan currentTime;
    [SerializeField] public int seconds;
    [SerializeField] public float minuteLength => dayLength / ClockConstant.MinutesInDay;

    public void LoadData(GameData data)
    {
        this.counterTillDayChange = data.timeCounter;
        this.seconds = data.currentTime;
    }

    public void SaveData(ref GameData data)
    {
        data.timeCounter = this.counterTillDayChange;
        data.currentTime = this.seconds;
    }

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
        if(counterTillDayChange >= maxCounter)
        {
            date.day++;
            date.namesIndex++;
            counterTillDayChange = counterReset;
        }
    }
    private void OnApplicationQuit()
    {
        int seconds = (int)Math.Round(currentTime.TotalSeconds);
    }
}
