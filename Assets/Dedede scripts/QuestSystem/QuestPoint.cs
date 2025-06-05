using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SphereCollider))]
public class QuestPoint : MonoBehaviour
{
    private bool playerIsNear = false;
    private string questId;
    private QuestState currentQuestState;
    private QuestIcon questIcon;

    [Header("Quest")]
    [SerializeField] private QuestInfoSO questInfoForPoint;

    [Header("Config")]
    [SerializeField] private bool startPoint = true;
    [SerializeField] private bool finishPoint = true;

    [Header("Dialogue")]
    [SerializeField] public bool hasDialogue;
    [SerializeField] public bool dialogueChecked;
    [SerializeField] public GameObject dialogueBox;

    private void Awake()
    {
        dialogueBox.SetActive(false);
        hasDialogue = false;
        questId = questInfoForPoint.id;
        questIcon = GetComponentInChildren<QuestIcon>();

        if (dialogueBox == null)
        {
            hasDialogue = false;
        }
        else
        {
            hasDialogue = true;
        }
    }

    private void Update()
    {               
        if(playerIsNear == true && Input.GetKeyDown(KeyCode.F) && hasDialogue == true)
        {
            dialogueBox.SetActive(true);
            dialogueChecked = true;
        }
        if (dialogueChecked == true)
        {
            ActivatedQuest();
            dialogueChecked = false;
            hasDialogue = false;
        }
        if (playerIsNear == true && Input.GetKeyDown(KeyCode.F) && hasDialogue == false)
        {
            ActivatedQuest();
            dialogueChecked = false;
        }
    }

    private void OnEnable()
    {
        GameEventsManager.instance.questEvents.onQuestStateChange += QuestStateChange;
        ActivatedQuest();
    }

    private void OnDisable()
    {
        GameEventsManager.instance.questEvents.onQuestStateChange -= QuestStateChange;
    }

    private void ActivatedQuest()
    {
        if(!playerIsNear)
        {
            return;
        }
        //start or finish a quest
        if(currentQuestState.Equals(QuestState.CAN_START) && startPoint)
        {
            GameEventsManager.instance.questEvents.StartQuest(questId);
        }
        else if(currentQuestState.Equals(QuestState.CAN_FINISH) && finishPoint)
        {
            GameEventsManager.instance.questEvents.FinishQuest(questId);
        }
    }

    private void QuestStateChange(Quest quest)
    {
        Debug.Log("hi");
        //only update the quest state if this point has the corresponding quest
        if(quest.info.id.Equals(questId))
        {
            currentQuestState = quest.state;
            questIcon.SetState(currentQuestState, startPoint, finishPoint);
        }
    }

    private void OnTriggerEnter(Collider otherCollider)
    {
        if(otherCollider.CompareTag("Player"))
        {
            playerIsNear = true;
        }
    }

    private void OnTriggerExit(Collider otherCollider)
    {
        if (otherCollider.CompareTag("Player"))
        {
            playerIsNear = false;
        }
    }
}
