using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(SphereCollider))]
public class Items : MonoBehaviour, ICollectable
{
    public static event HandleItemCollected OnItemCollected;
    public delegate void HandleItemCollected(ItemData itemData);
    public ItemData itemData;

    public bool isPlayerNear = false;

    public void Collect()
    {
        Debug.Log(OnItemCollected);
        Debug.Log(itemData);
        OnItemCollected?.Invoke(itemData);
        isPlayerNear = false;
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
