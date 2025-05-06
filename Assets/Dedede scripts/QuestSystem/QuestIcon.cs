using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestIcon : MonoBehaviour
{
    [Header("Icons")]
    [SerializeField] private GameObject requirementsNotMetToStartIcon;
    [SerializeField] private GameObject canStartIcon;
    [SerializeField] private GameObject requirementsNotMetToFinishIcon;
    [SerializeField] private GameObject canFinishIcon;

    public void SetState(QuestState newState, bool startPoint, bool finishPoint)
    {
        //set all to inactive
        requirementsNotMetToStartIcon.SetActive(false);
        requirementsNotMetToFinishIcon.SetActive(false);
        canStartIcon.SetActive(false);
        canFinishIcon.SetActive(false);
        //set according to current state that is active
        switch(newState)
        {
            case QuestState.REQUIREMENTS_NOT_MET:
                if(startPoint) { requirementsNotMetToStartIcon.SetActive(true); }
                break;
            case QuestState.CAN_START:
                if(startPoint){ canStartIcon.SetActive(true); }
                break;
            case QuestState.IN_PROGRESS:
                if(finishPoint) { requirementsNotMetToFinishIcon.SetActive(true); }
                break;
            case QuestState.CAN_FINISH:
                if(finishPoint) { canFinishIcon.SetActive(true); }
                break;
            case QuestState.FINISHED:
                break;
             default:
                Debug.Log("Quest State not recognized by switch statement for quest icon: " + newState);
                break;
        }
    }
}
