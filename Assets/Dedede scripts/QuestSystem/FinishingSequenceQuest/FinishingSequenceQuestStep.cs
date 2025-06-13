using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class FinishingSequenceQuestStep : QuestStep
{
    [SerializeField] public int defeatProgress = 0;
    [SerializeField] public static int enemiesDefeatedRequirement = 20;
    [SerializeField] public static TextMeshProUGUI enemiesDefeatedText;
    [SerializeField] public static TextMeshProUGUI currentQuestGoalText;

    DummyHealth dummyHealth;

    // Start is called before the first frame update
    void Start()
    {
        dummyHealth = FindObjectOfType<DummyHealth>();
    }

    public void EnemyDefeatProgression()
    {
        if (defeatProgress < enemiesDefeatedRequirement)
        {
            UpdateState();
        }

        if (defeatProgress >= enemiesDefeatedRequirement)
        {
            FinishQuestStep();
            SceneManager.LoadScene("End Cutscene");
        }
    }

    private void UpdateState()
    {
        string state = defeatProgress.ToString();
        ChangeState(state);
    }

    protected override void SetQuestStepState(string state)
    {
        this.defeatProgress = System.Int32.Parse(state);
        UpdateState();
    }
}
