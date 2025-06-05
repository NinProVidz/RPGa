using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NotePlaceHolder : MonoBehaviour
{

    public int noteAmount;

    private void OnEnable()
    {
        //Items.OnItemCollected += NoteCollected;
    }

    private void OnDisable()
    {
        //Items.OnItemCollected -= NoteCollected;
    }

    public void NoteCollected()
    {
        noteAmount++;
        Debug.Log("Note amount:" + noteAmount);
    }
}
