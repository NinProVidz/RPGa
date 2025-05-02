using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAwareness : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerManager player;

    [Header("Detection Settings")]
    [SerializeField] public float senseRange = 4;
    [SerializeField] public float fieldOfView = 60f;

    [Header("Awareness Settings")]
    [SerializeField] public float minSuspicionLevel = 1.5f;
    [SerializeField] public float minFoundLevel = 5.0f;
    [Space(1)]
    [SerializeField] float distanceAwarenessFactor = 2.0f;
    [Space(1)]
    [SerializeField] float fovAwarenessFactor = 5.0f;
    [SerializeField] float fovAngleAwarenessFactor = 5.0f;
    [Space(1)]
    [SerializeField] float visibleAwarenessFactor = 3.0f;
    [SerializeField] float visibleLimbsAwarenessFactor = 3.0f;
    [Space(1)]
    [SerializeField] float playerCrouchedAwarenessFactor = 0.5f;
    [SerializeField] float playerRunningAwarenessFactor = 2f;
    [SerializeField] float playerSprintingAwarenessFactor = 5f;
    [SerializeField] float playerMoveAwarenessFactor = 2f;
    [Space(1)]
    [SerializeField] LayerMask obstacleLayers;

    [Header("Awareness Stats")]
    public float awarenessLevel = 0;
    public bool unblocked;
    public float limbsUnblocked;
    public bool isMoving;
    public bool inFOV;

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
            foreach (GameObject limb in player.limbs)
            {
                Vector3 dir = (limb.transform.position - transform.position);
                if (Physics.Raycast(transform.position, dir.normalized, out RaycastHit hit, dir.magnitude, obstacleLayers))
                {

                }
                else
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(origin, limb.transform.position);
                }
                
            }

        }

        Gizmos.color = new Color(1, 0, 0, 1);
        Gizmos.DrawWireSphere(origin, senseRange);
    }

    void Start()
    {
        player = PlayerManager.instance;
    }

    void Update()
    {
        inFOV = false;
        limbsUnblocked = 0;
        HandleAwareness();

    }

    void HandleAwareness()
    {
        float distance = Vector3.Distance(transform.position, player.limbs[2].transform.position);

        if (distance > senseRange)
        {
            awarenessLevel = 0;
            return;
        }

        awarenessLevel = (senseRange - distance) * distanceAwarenessFactor / senseRange;

        
        HandlePlayerState();
        HandlePlayerVisible();


    }

    void HandlePlayerInFOV()
    {
        Vector3 dirToPlayer = (player.limbs[2].transform.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if( angle < fieldOfView * 0.5f)
        {
            inFOV = true;
            awarenessLevel *= fovAwarenessFactor;
            
        }
    }

    void HandlePlayerVisible()
    {
        unblocked = false;

        int playerLayer = LayerMask.NameToLayer("Player");

        foreach (GameObject limb in player.limbs)
        {
            Vector3 dir = (limb.transform.position - transform.position);
            float distance = Vector3.Distance(transform.position, limb.transform.position);

            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, distance))
            {
                if (hit.collider.gameObject.layer == playerLayer)
                {
                    Debug.Log("Unblocked limb: " + hit.collider.gameObject.name);
                    limbsUnblocked++;
                    unblocked = true;
                }
            }
        }

        

        if (unblocked)
        {
            HandlePlayerInFOV();
            awarenessLevel *= visibleAwarenessFactor + visibleLimbsAwarenessFactor * (limbsUnblocked / player.limbs.Length);
        }

    }

    private void HandlePlayerState()
    {
        if (player.playerLocomotionManager.isCrouching)
        {
            awarenessLevel *= playerCrouchedAwarenessFactor;
        }
        else
        {
            if (player.playerLocomotionManager.isSprinting)
            {
                awarenessLevel *= playerSprintingAwarenessFactor;
            }
            else
            {
                if (player.playerLocomotionManager.isRunning)
                {
                    awarenessLevel *= playerRunningAwarenessFactor;
                }
            }

        }

        if (player.playerLocomotionManager.moveAmount != 0)
        {
            isMoving = true;
            awarenessLevel *= playerMoveAwarenessFactor;
        }
    }
}
