using RPG.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushingEnvironmentFloorCollision : MonoBehaviour
{
    public void OnCollisionEnter(Collision other)
    {
        PushingEnvironment pEnvironment = other.collider.GetComponent<PushingEnvironment>();

        if (pEnvironment.isGrounded == false && pEnvironment.CompareTag("Pushable"))
        {
            Destroy(other.gameObject);
            Debug.Log("That hit the floor ):");
        }
        else
        {
            return;
        }
    }
}
