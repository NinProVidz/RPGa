using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainQuestStepStart : QuestStep
{

    [SerializeField] public int mapProgress = 0;
    [SerializeField] public static int mapCompletionRequirement = 5;
    [SerializeField] public static TextMeshProUGUI mapStateText;
    [SerializeField] public static TextMeshProUGUI currentQuestGoalText;

    public MapCollection mapCollection;

    // Start is called before the first frame update
    void Start()
    {
        mapCollection = FindAnyObjectByType<MapCollection>();
    }

    // Update is called once per frame
    void Update()
    {
        mapProgress = mapCollection.GetMapCompletion();
    }

    public void MapProgression()
    {
        if (mapProgress < mapCompletionRequirement)
        {
            UpdateState();
        }

        if (mapProgress >= mapCompletionRequirement)
        {
            FinishQuestStep();
        }
    }

    private void UpdateState()
    {
        string state = mapProgress.ToString();
        ChangeState(state);
    }

    protected override void SetQuestStepState(string state)
    {
        this.mapProgress = System.Int32.Parse(state);
        UpdateState();
    }
}
