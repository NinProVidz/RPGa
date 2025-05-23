using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushingEnvironment : MonoBehaviour
{
    [SerializeField] float forceMagnitude;
    public bool isGrounded;
    Rigidbody rb;

    private void Start()
    {
        isGrounded = true;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if(rb.velocity.z == 0)
        {
            isGrounded = true;
        }
        else
        {
           isGrounded = false;
        } 
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log("sus");
        Rigidbody rb = hit.collider.attachedRigidbody;
        if(rb != null)
        {
            Vector3 forceDirection = hit.gameObject.transform.position - transform.position;
            forceDirection.y = 0f;
            forceDirection.Normalize();


            rb.AddForceAtPosition(forceDirection * forceMagnitude, transform.position, ForceMode.Impulse);
        }
    }
}
