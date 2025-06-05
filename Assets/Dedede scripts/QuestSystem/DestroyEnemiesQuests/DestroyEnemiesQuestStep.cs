using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEnemiesQuestStep : QuestStep
{
    private int enemiesDefeated = 0;
    private int enemiesToComplete = 2;

    DummyHealth dummyHealth;

    private void Start()
    {
       dummyHealth = FindObjectOfType<DummyHealth>();
        dummyHealth.QuestActivated();
    }


    public void EnemyDefeated()
    {
        if(enemiesDefeated < enemiesToComplete)
        {
            enemiesDefeated++;
            UpdateState();
        }

        if(enemiesDefeated >= enemiesToComplete)
        {
            FinishQuestStep();
        }
    }

    private void UpdateState()
    {
        string state = enemiesDefeated.ToString();
        ChangeState(state);
    }

    protected override void SetQuestStepState(string state)
    {
        this.enemiesDefeated = System.Int32.Parse(state);
        UpdateState();
    }
}
