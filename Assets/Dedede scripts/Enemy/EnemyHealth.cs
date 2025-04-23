using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] int eHealth = 100;
    [SerializeField] int takeDamage = 40;
    [SerializeField] PushingEnvironment pEnvironment;
    public bool isGrounded;

    private void Start()
    {
        isGrounded = false;
    }

    private void Update()
    {
        isGrounded = pEnvironment.isGrounded;
    }

    private void OnCollisionEnter(Collision other)
    {
        if(isGrounded == false && other.gameObject.CompareTag("Pushable"))
        {
            eHealth -= takeDamage;
        }
        else
        {
            return;
        }
    }
}
