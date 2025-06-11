using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class MapCollection : MonoBehaviour
{
    [SerializeField] public static float mapCompletion;
    [SerializeField] public static float mapCompleted = 5;
    [SerializeField] public static float mapPercentage;
    [SerializeField] public bool isPlayerNear;

    private void Start()
    {
        isPlayerNear = false;
    }

    private void Update()
    {
        Debug.Log(mapCompletion);
        if(isPlayerNear && Input.GetKeyDown(KeyCode.F))
        {
            CollectTheMap();
        }
    }

    private void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit(Collider otherCollider)
    {
        if (otherCollider.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }

    public void CollectTheMap()
    {
        mapCompletion++;
        mapPercentage = (mapCompletion / mapCompleted) * 100;
        Debug.Log("map is " + mapPercentage + "% " + "Complete.");
        if (mapCompletion >= mapCompleted)
        {
            Debug.Log("You win :)");
        }
        isPlayerNear = false;
        Destroy(gameObject);
    }
}
