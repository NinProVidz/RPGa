using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEnemiesQuestStep : QuestStep
{
    private int enemiesDefeated = 0;
    private int enemiesToComplete = 1;

    private void OnEnable()
    {
        GameEventManager.instance.miscEvents.onEnemyDefeated += EnemyDefeated;
    }

    private void OnDisable()
    {
        GameEventManager.instance.miscEvents.onEnemyDefeated -= EnemyDefeated;
    }

    private void EnemyDefeated()
    {
        if(enemiesDefeated < enemiesToComplete)
        {
            enemiesDefeated++;
        }

        if(enemiesDefeated >= enemiesToComplete)
        {
            FinishQuestStep();
        }
    }
}
