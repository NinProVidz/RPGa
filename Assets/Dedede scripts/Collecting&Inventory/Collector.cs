using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        var notes = collision.GetComponent<Notes>();
        if(notes != null)
        {
            notes.Collect();
        }
    }
}
