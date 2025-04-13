using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData 
{
    public int month;
    public int day;

    public GameData()
    {
        this.day = 0;
        this.month = 0;
    }
}
