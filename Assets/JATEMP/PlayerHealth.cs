using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    

    

    [Header("Health UI")]
    [SerializeField] Image healthBar;
    [SerializeField] TextMeshProUGUI text;

    [Header("Health Stats")]
    public float health;
    public float maxhealth;

    [Header("Health Info")]
    bool Isdead = false;

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
        }
        if (health > 2)
        {
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
    }

    public bool GetIsDead()
    {
        //this is to make the bool public (just cal the function)
        return Isdead;
    }

    private void EnemyHealthDisplay()
    {
        if (gameObject.CompareTag("Enemy"))
        {
            text.text = health.ToString() + "/" + maxhealth.ToString();
        }
    }
}
