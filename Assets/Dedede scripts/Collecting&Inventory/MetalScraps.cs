using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(SphereCollider))]
public class MetalScraps : MonoBehaviour, ICollectable
{
    public static event HandleScrapCollected OnScrapCollected;
    public delegate void HandleScrapCollected(ItemData itemData);
    public ItemData scrapData;

    public bool isPlayerNearMetalScrap = false;

    private void Update()
    {
        FindObjectOfType<Collector>().playerCheck = isPlayerNearMetalScrap;
    }

    public void Collect()
    {
        Debug.Log(OnScrapCollected);
        Debug.Log(scrapData);
        OnScrapCollected?.Invoke(scrapData);
        isPlayerNearMetalScrap = false;
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider otherCollider)
    {
        FindObjectOfType<Collector>().possibleCollectable = this;
        if (otherCollider.CompareTag("Player"))
        {
            isPlayerNearMetalScrap = true;
        }
    }

    private void OnTriggerExit(Collider otherCollider)
    {
        FindObjectOfType<Collector>().possibleCollectable = null;
        if (otherCollider.CompareTag("Player"))
        {
            isPlayerNearMetalScrap = false;
        }
    }
}
