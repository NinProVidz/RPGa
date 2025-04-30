using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyHealth : MonoBehaviour
{
    [SerializeField] int eHealth = 100;
    [SerializeField] int takeDamage = 40;
    [SerializeField] int dmgThreshHold = 0;
    [SerializeField] PushingEnvironment pEnvironment;
    public bool isGrounded;
    DestroyEnemiesQuestStep destroyEnemiesQuestStep;

    private void Start()
    {
        isGrounded = false;
        destroyEnemiesQuestStep = FindObjectOfType<DestroyEnemiesQuestStep>();
    }

    private void Update()
    {
        isGrounded = pEnvironment.isGrounded;
        if(eHealth <= dmgThreshHold)
        {
            eHealth = 0;
            destroyEnemiesQuestStep.EnemyDefeated();
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if(isGrounded == false && other.gameObject.CompareTag("Pushable"))
        {
            eHealth -= takeDamage;
            Destroy(other.gameObject);
        }
        else
        {
            return;
        }
    }
}
