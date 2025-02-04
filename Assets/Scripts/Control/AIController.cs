using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using System;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        public PatrolPath patrolPath;
        public Canvas healthCanvas;

        public float chaseDistance = 5f;
        public float suspicionTime = 3f;
        public float timeSinceLastSawPlayer;
        public float waypointTolerance = 1f;
        public int currentWaypointindex;

        Fighter fighter;
        GameObject player;
        Health health;
        Mover mover;

        Vector3 guardPosition;

        // Start is called before the first frame update
        void Start()
        {
            fighter = GetComponent<Fighter>();
            player = GameObject.FindWithTag("Player");
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();

            guardPosition = transform.position;
            
        }

        // Update is called once per frame
        void Update()
        {
            healthCanvas.transform.LookAt(Camera.main.transform);
            if (health.GetIsDead() == true) return;

            if(InAttackRangeOfPlayer() && fighter.CanAttack(player))
            {
                timeSinceLastSawPlayer = 0f;
                AttackBehaviour();
            }
            else if(timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }
            timeSinceLastSawPlayer += Time.deltaTime;
        }

        public void PatrolBehaviour()
        {
            Vector3 nextPosition = guardPosition;
            if(patrolPath != null)
            {
                if(AtWaypoint() == true)
                {
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWaypointPosition();
            }
            mover.StartMoveAction(nextPosition);
        }

        private bool AtWaypoint()
        {
           return Vector3.Distance(transform.position, patrolPath.GetWaypoint(currentWaypointindex)) < waypointTolerance;
        }

        private void CycleWaypoint()
        {
            currentWaypointindex =  patrolPath.GetNextWaypoint(currentWaypointindex); 
        }

        private Vector3 GetCurrentWaypointPosition()
        {
            return patrolPath.GetWaypoint(currentWaypointindex);
        }

        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            fighter.Attack(player);
        }

        private bool InAttackRangeOfPlayer()
        {
            return Vector3.Distance(transform.position, player.transform.position) <= chaseDistance;
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}//eeee

