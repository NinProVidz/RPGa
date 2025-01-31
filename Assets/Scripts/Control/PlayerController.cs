using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using System;
using RPG.Core;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        Mover mover;
        Health health;

        // Start is called before the first frame update
        void Start()
        {
            mover = GetComponent<Mover>();
            health = GetComponent<Health>();
        }

        // Update is called once per frame
        void Update()
        {
            if(health.GetIsDead() == true) return;

            if (InteractWithCombat() == true)
            {
                return;
            }

            if(InteractWithMovement() == true)
            {
                return;
            }

            Debug.Log("Do Nothing");
        }

        private bool InteractWithCombat()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            foreach(RaycastHit hit in hits)
            {
                GameObject target = hit.transform.gameObject;
                if (target == null) continue;
                if(GetComponent<Fighter>().CanAttack(target.gameObject) == false)
                {
                    continue; 
                }

                if(Input.GetMouseButton(0))
                {
                    transform.LookAt(target.transform);
                    GetComponent<Fighter>().Attack(target.gameObject);
                }

                return true;
            }
            return false;
        }

        private bool InteractWithMovement()
        {
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit); //stores if it hit into the var "hit"
            if (hasHit == true)
            {
                if(Input.GetMouseButton(0))
                {
                    mover.StartMoveAction(hit.point);
                }
                return true;
            }
            return false;
        }

        private Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}


