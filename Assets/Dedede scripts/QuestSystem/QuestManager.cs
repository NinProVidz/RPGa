using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private Dictionary<string, Quest> questMap;

    private void Awake()
    {
        questMap = CreateQuestMap();   
    }

    private Dictionary<string, Quest> CreateQuestMap()
    {
        //loads all QuestInfoSO ScriptableObjects under the Assets/Resources/Quests folder
        QuestInfoSO[] allQuests = Resources.LoadAll<QuestInfoSO>("Quests");
        //Create the quest map
        Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();
        foreach(QuestInfoSO questInfo in allQuests)
        {
            if(idToQuestMap.ContainsKey(questInfo.id))
            {
                Debug.LogWarning("Dupicate ID found when creating quest map:" + questInfo.id);
            }
            idToQuestMap.Add(questInfo.id, new Quest(questInfo));
        }
        return idToQuestMap;
    }

    private Quest GetQuestById(string id)
    {
        Quest quest = questMap[id];
        if(quest == null)
        {
            Debug.LogError("ID not found in the Quest Map:" + id);
        }
        return quest;
    }
}
