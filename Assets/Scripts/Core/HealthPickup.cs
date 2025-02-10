using RPG.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class HealthPickup : MonoBehaviour
    {
        public float healthamount;
        public float damageIncrease;

        Health health;
        Fighter fighter;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if(gameObject.CompareTag("Health"))
            {
                if (other.tag == ("Player"))
                {
                    health = other.GetComponent<Health>();
                    health.health = Mathf.Min(health.health + healthamount, health.maxhealth);
                    Destroy(gameObject);
                }
            }
            if (gameObject.CompareTag("DamageUpgrade"))
            {
                if (other.tag == ("Player"))
                {
                    fighter = other.GetComponent<Fighter>();
                    fighter.damage += damageIncrease;
                    Destroy(gameObject);
                }
            }
        }


    }
}//eee

