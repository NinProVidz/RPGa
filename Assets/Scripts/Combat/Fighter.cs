using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;

namespace RPG.Combat
{
 
    public class Fighter : MonoBehaviour, IAction
    {
        Mover mover;
        public float timeBetweenAttacks;
        public float timeSinceLastAttack;

        [SerializeField] float weaponRange = 2f;

        Transform target;

        public float damage;

        private void Start()
        {
            mover = GetComponent<Mover>();
        }
        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            if (target == null)
            {
                return;
            }

            if (GetIsInRange() == false)
            {
                mover.MoveTo(target.position);
            }
            else
            {
                
                AttackBehaviour();
                mover.Cancel();

            }
        }

        private void AttackBehaviour()
        {
            if (timeSinceLastAttack >= timeBetweenAttacks)
            {
                GetComponent<Animator>().SetTrigger("attack");
                timeSinceLastAttack = 0f;
            }
                
        }

        private bool GetIsInRange()
        {
            // my current distance from the enemy < stopping distance
            return Vector3.Distance(transform.position, target.position) < weaponRange;
        }

        public void Attack(CombatTarget combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.transform;
        }

        public void Cancel()
        {
            target = null;
        }

        void Hitterman()
        {
            target.GetComponent<Health>().TakeDamage(damage);
        }
    }

}//eeeeee


