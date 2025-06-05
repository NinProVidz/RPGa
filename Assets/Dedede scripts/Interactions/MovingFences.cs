using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class MovingFences : MonoBehaviour
{
    [SerializeField] GameObject fenceGate;
    public bool closeToSwitch;
    public bool alreadyMoved;

    public float duration = 2.0f;
    public float movingTime = 2.0f;

    [SerializeField] Vector3 movingVector;

    private void Start()
    {
        closeToSwitch = false;
        alreadyMoved = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(closeToSwitch && alreadyMoved == false && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(MovingTheFence());
        }
        if(closeToSwitch && alreadyMoved && Input.GetKeyDown(KeyCode.F))
        {
            return;
        }
    }

    public IEnumerator MovingTheFence()
    {
        float startTime = Time.time;
        float movementTime = 0;
        while(movementTime < movingTime)
        {
            fenceGate.transform.position = Vector3.Lerp(fenceGate.transform.position, fenceGate.transform.position + movingVector, Time.deltaTime);
            movementTime = (Time.time - startTime) / duration;
            alreadyMoved = true;
            yield return null;
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            closeToSwitch = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        closeToSwitch = false;
    }
}
