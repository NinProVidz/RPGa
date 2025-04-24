using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreakyLarryBehaviour : MonoBehaviour
{
    [SerializeField] PlayerManager player = PlayerManager.instance;
    [SerializeField] LayerMask playerLayer;

    [SerializeField] public float senseRange = 4;
    [SerializeField] public float fieldOfView = 60f;
    
    [Header("Awareness")]
    [SerializeField] float awarenessLevel = 0;
    [Space(10)]
    [Header("Awareness Settings")]
    [SerializeField] float distanceToAwarenessFactor = 1;
    [SerializeField] float unblockedAwarenessFactor = 1.5f;
    [SerializeField] float FovAwarenessFactor = 2;
    [SerializeField] float playerCrouchedAwarenessFactor = 0.5f;
    [SerializeField] float playerSprintAwarenessFactor = 2f;
    [SerializeField] float playerRunAwarenessFactor = 1.5f;
    [SerializeField] float playerMoveAwarenessFactor = 1.25f;
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position;
        Vector3 forward = transform.forward;

        // Set color for FOV cone
        Gizmos.color = new Color(1, 1, 0, 1); // Yellow-ish
        Quaternion leftRayRotation = Quaternion.AngleAxis(-fieldOfView * 0.5f, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(fieldOfView * 0.5f, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * forward;
        Vector3 rightRayDirection = rightRayRotation * forward;

        // Draw field of view arc lines
        Gizmos.DrawRay(origin, leftRayDirection * senseRange);
        Gizmos.DrawRay(origin, rightRayDirection * senseRange);

        // Optional: Draw a wire arc
        int segments = 30;
        Vector3 lastPoint = origin + (leftRayDirection * senseRange);
        for (int i = 1; i <= segments; i++)
        {
            float angle = -fieldOfView / 2f + (fieldOfView / segments) * i;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 nextPoint = origin + (rotation * forward * senseRange);
            Gizmos.DrawLine(lastPoint, nextPoint);
            lastPoint = nextPoint;
        }

        Quaternion upRayRotation = Quaternion.AngleAxis(-fieldOfView * 0.5f, transform.right);
        Quaternion downRayRotation = Quaternion.AngleAxis(fieldOfView * 0.5f, transform.right);

        Vector3 upRayDirection = upRayRotation * forward;
        Vector3 downRayDirection = downRayRotation * forward;

        // Draw up/down FOV lines
        Gizmos.DrawRay(origin, upRayDirection * senseRange);
        Gizmos.DrawRay(origin, downRayDirection * senseRange);

        // Optional: Vertical FOV arc (wireframe)
        segments = 20;
        lastPoint = origin + (upRayDirection * senseRange);
        for (int i = 1; i <= segments; i++)
        {
            float angle = -fieldOfView / 2f + (fieldOfView / segments) * i;
            Quaternion rotation = Quaternion.AngleAxis(angle, transform.right);
            Vector3 nextPoint = origin + (rotation * forward * senseRange);
            Gizmos.DrawLine(lastPoint, nextPoint);
            lastPoint = nextPoint;
        }

        // Line to target (optional)
        if (player != null)
        {
            Vector3 toTarget = player.transform.position - origin;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin, player.transform.position);
        }

        Gizmos.color = new Color(1, 0, 0, 1);
        Gizmos.DrawWireSphere(origin, senseRange);
    }

    // Update is called once per frame
    void Update()
    {
        HandleEnemyAwareness();
    }

    private void HandleEnemyAwareness()
    {
        AwarenessIfPlayerIsInRange();
        AwarenessIfPlayerIsUnblocked();
        AwarenessIfPlayerIsInFOV();
        AwarenessBasedOnPlayerState();
    }

    private float GetAwarenessFactorDueToDistance()
    {
        float distance = (transform.position - player.transform.position).magnitude;
        float distanceFactor = (senseRange - distance) / senseRange;
        return distanceFactor * distanceToAwarenessFactor;
    }

    private void AwarenessIfPlayerIsInRange()
    {
        float distance = (transform.position - player.transform.position).magnitude;
        if (distance <= senseRange)
        {
            awarenessLevel = 1 + GetAwarenessFactorDueToDistance();
        }
        else
        {
            awarenessLevel = 0;
        }
    }

    private void AwarenessIfPlayerIsUnblocked()
    {
        Vector3 direction = (transform.position - player.transform.position).normalized;
        Ray ray = new Ray(transform.position, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, senseRange))
        {
            if (hit.transform.gameObject.layer == playerLayer)
            {
                awarenessLevel *= unblockedAwarenessFactor;
            }
        }
    }

    private void AwarenessIfPlayerIsInFOV()
    {
        Vector3 directionToTarget = (transform.position - player.transform.position).normalized;
        Vector3 enemyForward = -transform.forward;

        // Calculate angle between forward and direction to target
        float angleToTarget = Vector3.Angle(enemyForward, directionToTarget);

        Debug.Log(angleToTarget);

        if (angleToTarget < fieldOfView * 0.5f)
        {
            awarenessLevel *= FovAwarenessFactor;
        }
    }

    private void AwarenessBasedOnPlayerState()
    {
        if (player.playerLocomotionManager.isCrouching)
        {
            awarenessLevel *= playerCrouchedAwarenessFactor;
        }
        else
        {
            if(player.playerLocomotionManager.isSprinting)
            {
                awarenessLevel *= playerCrouchedAwarenessFactor;
            }
            else
            {
                if (player.playerLocomotionManager.isRunning)
                {
                    awarenessLevel *= playerRunAwarenessFactor;
                }
            }

        }

        if (player.playerLocomotionManager.moveAmount != 0)
        {
            awarenessLevel *= playerMoveAwarenessFactor;
        }
    }
}
