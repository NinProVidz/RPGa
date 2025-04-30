using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEnemiesQuestStep : QuestStep
{
    private int enemiesDefeated = 0;
    private int enemiesToComplete = 1;

    public void EnemyDefeated()
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
