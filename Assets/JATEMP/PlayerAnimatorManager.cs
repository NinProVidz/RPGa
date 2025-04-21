using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorManager : MonoBehaviour
{
    PlayerManager player;

    int vertical;
    int horizontal;

    public LayerMask groundLayer; // Set to "Default" or "Terrain"

    private void OnDrawGizmos()
    {
        Ray lookAtRay = new Ray(PlayerCamera.instance.cameraObject.transform.position, PlayerCamera.instance.cameraObject.transform.forward);
        Gizmos.DrawRay(lookAtRay);
    }

    private void Awake()
    {
        player = GetComponent<PlayerManager>();

        vertical = Animator.StringToHash("Vertical");
        horizontal = Animator.StringToHash("Horizontal");
    }

    private void Start()
    {

    }

    private void OnAnimatorIK(int layerIndex)
    {
        player.animator.SetLookAtWeight(1f, 0.2f, 1f, 1f, 0f);
        Ray lookAtRay = new Ray(PlayerCamera.instance.cameraObject.transform.position, PlayerCamera.instance.cameraObject.transform.forward);
        player.animator.SetLookAtPosition(lookAtRay.GetPoint(100));


    }
    public void UpdateAnimatorMovementParameters(float horizontalMovement, float verticalMovement, bool isSprinting, bool isRunning)
    {
        float horizontalAmount = horizontalMovement;
        float verticalAmount = verticalMovement;

        if (isRunning)
        {
            verticalAmount *= 2;
            horizontalAmount *= 2;
        }

        if (isSprinting)
        {
            verticalAmount = 3;
        }
        player.animator.SetFloat(horizontal, horizontalAmount, 0.3f, Time.deltaTime);
        player.animator.SetFloat(vertical, verticalAmount, 0.3f, Time.deltaTime);

    }

    public virtual void PlayTargetActionAnimation(string targetAnimation, bool isPerformingAction, bool applyRootmotion = true)
    {
        player.animator.applyRootMotion = applyRootmotion;
        player.animator.CrossFade(targetAnimation, 0.2f);

        player.isPerformingAction = isPerformingAction;
    }
}
