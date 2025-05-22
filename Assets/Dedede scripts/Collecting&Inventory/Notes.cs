using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Notes : MonoBehaviour, ICollectable
{
    public bool isPlayerNear = false;

    private void Update()
    {
        FindObjectOfType<Collector>().playerCheck = isPlayerNear;
    }

    public void Collect()
    {
        Debug.Log("Wow");
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
