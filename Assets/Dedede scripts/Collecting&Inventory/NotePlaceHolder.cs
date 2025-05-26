using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotePlaceHolder : MonoBehaviour
{

    public int noteAmount;

    private void OnEnable()
    {
        Notes.OnNoteCollected += NoteCollected;
    }

    private void OnDisable()
    {
        Notes.OnNoteCollected -= NoteCollected;
    }

    public void NoteCollected()
    {
        noteAmount++;
        Debug.Log("Note amount:" + noteAmount);
    }
}
