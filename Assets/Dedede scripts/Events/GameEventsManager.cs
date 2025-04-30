using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventsManager : MonoBehaviour
{
    public static GameEventsManager instance { get; private set; }

    public QuestEvents questEvents;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Found more than one Game Events Manager in the scene.");
        }
        questEvents = new QuestEvents();
    }
}
