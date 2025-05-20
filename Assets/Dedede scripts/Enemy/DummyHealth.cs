using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DummyHealth : MonoBehaviour
{
    [SerializeField] int eHealth = 100;
    [SerializeField] int takeDamage = 40;
    [SerializeField] int dmgThreshHold = 0;
    public bool isGrounded;
    public static bool questActive;
    DestroyEnemiesQuestStep destroyEnemiesQuestStep;

    private void Start()
    {
        isGrounded = false;
        questActive = false;
    }

    private void Update()
    {
        isGrounded = FindObjectOfType<PushingEnvironment>().isGrounded;
        if (eHealth <= dmgThreshHold)
        {
            eHealth = 0;
            Destroy(gameObject);
        }
        if (questActive == false)
        {
            return;
        }
        if(questActive == true)
        {
            destroyEnemiesQuestStep = FindObjectOfType<DestroyEnemiesQuestStep>();
        }
        if(eHealth <= dmgThreshHold && questActive == true)
        {
            eHealth = 0;
            destroyEnemiesQuestStep.EnemyDefeated();
            Destroy(gameObject);
        }
    }

    public void OnCollisionEnter(Collision other)
    {
        PushingEnvironment pEnvironment = other.collider.GetComponent<PushingEnvironment>();

        if(pEnvironment.isGrounded == false && pEnvironment.CompareTag("Pushable"))
        {
            eHealth -= takeDamage;
            Destroy(other.gameObject);
        }
        else
        {
            return;
        }
    }

    public void QuestActivated()
    {
        questActive = true;
    }
}
