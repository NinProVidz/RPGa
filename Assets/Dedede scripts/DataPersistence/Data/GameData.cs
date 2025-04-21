using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class GameData 
{
    public int month;
    public int day;
    public Vector3 playerPosition;
    

    public GameData()
    {
        day = 0;
        month = 0;
        playerPosition = Vector3.zero;
    }
}
