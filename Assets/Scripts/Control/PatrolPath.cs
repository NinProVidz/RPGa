using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public class PatrolPath : MonoBehaviour
    {
        [SerializeField] float waypointSphereRadius = 0.3f;

        private void OnDrawGizmos()
        {
            for(int i = 0; 1 < transform.childCount; i++)
            {
                
                int j = GetNextWaypoint(i);
                Gizmos.DrawSphere(GetWaypoint(i), waypointSphereRadius);
                Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(j));
            }
        }

        private int GetNextWaypoint(int i)
        {
            return (i + 1) % transform.childCount;
        }

        private Vector3 GetWaypoint(int i)
        {
            return transform.GetChild(i).position;
        }
    }
}

