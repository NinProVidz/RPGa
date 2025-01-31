using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class Health : MonoBehaviour
    {
        bool Isdead = false;
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
                Die();
            }
        }

        private void Die()
        {
            if(Isdead)
            {
                return;
            }
            GetComponent<Animator>().SetTrigger("die");
            Isdead = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        public bool GetIsDead()
        {
            //this is to make the bool public (just cal the function)
            return Isdead;
        }
    }
}//e

