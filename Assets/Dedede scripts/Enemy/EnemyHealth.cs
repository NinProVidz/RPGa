using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] int eHealth = 100;
    [SerializeField] int takeDamage = 40;
    PushingEnvironment pEnvironment;
    public bool isGrounded;

    private void Start()
    {
        isGrounded = false;
        pEnvironment = FindObjectOfType<PushingEnvironment>();
    }

    private void Update()
    {
        isGrounded = pEnvironment.isGrounded;
    }

    private void OnCollisionEnter(Collision other)
    {
        if(isGrounded != true && other.gameObject.CompareTag("Pushable"))
        {
            eHealth -= takeDamage;
        }
    }
}
