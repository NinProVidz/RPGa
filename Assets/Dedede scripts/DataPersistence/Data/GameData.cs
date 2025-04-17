using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class GameData 
{
    public int month;
    public int day;
    public float timeOfDay;
    public Vector3 playerPosition;
    public int timeCounter;
    public TimeSpan currentTime;

    public GameData()
    {
        day = 0;
        month = 0;
        timeOfDay = 0;
        playerPosition = Vector3.zero;
        timeCounter = 0;
        currentTime = TimeSpan.Zero;
    }
}
