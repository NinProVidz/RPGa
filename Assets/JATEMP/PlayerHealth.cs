using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    bool Isdead = false;

    [SerializeField] TextMeshProUGUI text;

    [Header("Health UI")]
    [SerializeField] Image healthBar;

    [Header("Health Settings")]
    public float health;
    public float maxhealth;

    private void Start()
    {
        health = maxhealth;
    }
    public void TakeDamage(float damage)
    {
        health = Mathf.Max(health - damage, 0f);
        Debug.Log("DAMAGED!!! AHHHH");
    }

    private void Update()
    {
        HealthBar();
        EnemyHealthDisplay();

        if (health <= 0)
        {
            Die();
        }
        if (health <= 2)
        {
            EnableChromaticAberration(true);
        }
        if (health > 2)
        {
            EnableChromaticAberration(false);
        }
    }

    private void HealthBar()
    {
        healthBar.fillAmount = Mathf.Clamp(health / maxhealth, 0, 1);
    }

    private void Die()
    {
        if (Isdead)
        {
            return;
        }
        GetComponent<Animator>().SetTrigger("die");
        Isdead = true;
        //GetComponent<ActionScheduler>().CancelCurrentAction();
    }

    public bool GetIsDead()
    {
        //this is to make the bool public (just cal the function)
        return Isdead;
    }
    void EnableChromaticAberration(bool enable)
    {
        /*if (chromaticAberration != null)
        {
            chromaticAberration.enabled.value = enable; // Enable or disable the Chromatic Aberration effect
        }*/
    }

    private void EnemyHealthDisplay()
    {
        if (gameObject.CompareTag("Enemy"))
        {
            text.text = health.ToString() + "/" + maxhealth.ToString();
        }
    }
}
