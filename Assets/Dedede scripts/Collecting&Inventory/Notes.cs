using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(SphereCollider))]
public class Notes : MonoBehaviour, ICollectable
{

    public static event Action OnNoteCollected;

    public bool isPlayerNear = false;

    private void Update()
    {
        FindObjectOfType<Collector>().playerCheck = isPlayerNear;
    }

    public void Collect()
    {
        OnNoteCollected?.Invoke();
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider otherCollider)
    {
        FindObjectOfType<Collector>().possibleCollectable = this;
        if (otherCollider.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit(Collider otherCollider)
    {
        FindObjectOfType<Collector>().possibleCollectable = null;
        if (otherCollider.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }
}
