using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;
using System.Runtime.InteropServices.WindowsRuntime;

namespace RPG.Combat
{
 
    public class Fighter : MonoBehaviour, IAction
    {
        Mover mover;
        public float timeBetweenAttacks;
        public float timeSinceLastAttack = Mathf.Infinity;

        [SerializeField] float weaponRange = 2f;

        Health target;
     
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

            if(target.GetIsDead() == true)
            {
                return;
            }

            if (GetIsInRange() == false)
            {
                mover.MoveTo(target.transform.position);
            }
            else
            {
                
                mover.Cancel();
                AttackBehaviour();

            }
        }

        private void AttackBehaviour()
        {
            if (timeSinceLastAttack >= timeBetweenAttacks)
            {
                TriggerAttack();
                timeSinceLastAttack = 0f;
            }

        }

        private void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }

        private bool GetIsInRange()
        {
            // my current distance from the enemy < stopping distance
            return Vector3.Distance(transform.position, target.transform.position) <= weaponRange;
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
            transform.LookAt(target.transform.position);
        }

        public void Cancel()
        {
            StopAttack();
            target = null;
        }

        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }

        void Hitterman()
        {
            if(target == null) return;
            target.TakeDamage(damage);
        }
        public bool CanAttack(GameObject combattarget)
        {
            if(combattarget == null) { return false; }
            Health targetToTest = combattarget.GetComponent<Health>();
            return targetToTest != null && targetToTest.GetIsDead() == false;
        }

    }


}//eeeeee


