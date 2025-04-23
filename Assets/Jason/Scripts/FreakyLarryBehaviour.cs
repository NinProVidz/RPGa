using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreakyLarryBehaviour : MonoBehaviour
{
    PlayerManager player = PlayerManager.instance;

    [SerializeField] public float senseRange = 4;
    [SerializeField] public float fieldOfView = 60f;
    [SerializeField] float awarenessLevel = 0;
    [SerializeField] float distanceToAwarenessFactor = 1;
    [SerializeField] float unblockedAwarenessFactor = 1.5f;
    [SerializeField] float FovAwarenessFactor = 2;
    [SerializeField] float playerCrouchedAwarenessFactor = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
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
    }

    private float GetAwarenessFactorDueToDistance()
    {
        float distance = (transform.position - player.transform.position).magnitude;
        float distanceFactor = distance / senseRange;
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
        Physics.Raycast(ray, out hit, senseRange);
        
        if (hit.transform.GetComponent<PlayerManager>())
        {
            awarenessLevel *= unblockedAwarenessFactor;
        }
    }

    private void AwarenessIfPlayerIsInFOV()
    {
        Vector3 directionToTarget = (transform.position - player.transform.position).normalized;
        Vector3 enemyForward = transform.forward;

        // Calculate angle between forward and direction to target
        float angleToTarget = Vector3.Angle(enemyForward, directionToTarget);

        if (angleToTarget < fieldOfView * 0.5f)
        {
            awarenessLevel *= FovAwarenessFactor;
        }
    }

    private void AwarenessBasedOnPlayerState()
    {
        if (player.playerLocomotionManager.isCrouching)
        {

        }
    }
}
