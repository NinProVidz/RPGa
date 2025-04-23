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
    }

    private void Update()
    {
        if(rb.velocity.y != 0)
        {
            isGrounded = false;
        }
        else
        {
            isGrounded = true;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
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
