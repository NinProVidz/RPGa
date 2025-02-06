using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class HealthPickup : MonoBehaviour
    {
        public float healthamount;

        Health health;

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
            if(other.tag == ("Player"))
            {
                health = other.GetComponent<Health>();
                health.health = Mathf.Min(health.health + healthamount, health.maxhealth);
                Destroy(gameObject);
            }
        }
    }
}//e

