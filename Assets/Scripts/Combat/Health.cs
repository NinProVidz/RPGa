using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float health;

        public void TakeDamage(float damage)
        {
            health = Mathf.Max(health - damage, 0f);
            Debug.Log("DAMAGED!!! AHHHH");
        }

        private void Update()
        {
            if(health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}

