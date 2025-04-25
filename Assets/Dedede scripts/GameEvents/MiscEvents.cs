using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MiscEvents
{
    public event Action onEnemyDefeated;

    public void EnemyDefeated()
    {
        if(onEnemyDefeated != null)
        {
            onEnemyDefeated();
        }
    }
}
