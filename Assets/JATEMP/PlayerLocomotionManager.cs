using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotionManager : MonoBehaviour, IDataPersistence
{
    PlayerManager player;

    public float verticalMovement;
    public float horizontalMovement;
    public float moveAmount;

    [Header("Movement Settings")]
    public Vector3 moveDirection;
    private Vector3 targetRotationDirection;
    [SerializeField] float jumpHeight = 4f;
    [SerializeField] float walkingSpeed = 2;
    [SerializeField] float runningSpeed = 5;
    [SerializeField] float sprintingSpeed = 6.5f;
    [SerializeField] float crouchSpeed = 1f;
    [SerializeField] float crouchStrafeSpeed = 1f;
    [SerializeField] float rotationSpeed = 15f;

    [Header("Tilt settings")]
    [SerializeField] Transform tiltBone;
    [SerializeField] Transform tiltBone2;
    public float tiltAngle = 30f;
    public float tiltSpeed = 5f; // Controls how fast it tilts
    public float maxHoldTime = 1.0f; // Time to reach full tilt

    private float holdTimer = 0f;
    private float currentTiltZ = 0f;

    public Transform tiltRaycastOrigin;
    public float wallCheckDistance = 0.5f;
    public LayerMask wallLayer;

    [Header("Movement Checks")]
    public bool isSprinting;
    public bool isRunning;
    public bool isCrouching;

    [Header("Ground Check & Jumping")]
    [SerializeField] private Transform groundCheckTransform; // Assign an empty GameObject at the feet
    [SerializeField] private float groundCheckDistance = 0.2f; // Small offset from feet
    [SerializeField] float gravityForce = -5.55f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundCheckSphereRadius = 1;
    [SerializeField] protected Vector3 yVelocity;
    [SerializeField] protected float groundedVelocity = -20;
    [SerializeField] protected float fallStartVelocity = -5;
    bool fallingVelocityHasBeenSet = false;
    public float inAirTimer = 0;

    private bool wasGrounded = true;
    private bool hasLanded = false;

    private void Awake()
    {
        player = GetComponent<PlayerManager>();
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheckTransform != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheckTransform.position, groundCheckSphereRadius);
            Gizmos.DrawLine(groundCheckTransform.position + Vector3.up * 0.1f, groundCheckTransform.position + Vector3.down * (groundCheckDistance));
        }

        if (tiltRaycastOrigin == null) return;

        // Set colors for left and right raycasts
        Gizmos.color = Color.red;

        // Draw right ray
        Vector3 rightDir = tiltRaycastOrigin.right;
        Gizmos.DrawRay(tiltRaycastOrigin.position, rightDir * wallCheckDistance);

        // Draw left ray
        Vector3 leftDir = -tiltRaycastOrigin.right;
        Gizmos.DrawRay(tiltRaycastOrigin.position, leftDir * wallCheckDistance);
    }


    private void LateUpdate()
    {
        HandleGroundCheck();
        player.animator.SetBool("isGrounded", player.isGrounded);

        // Detect transition from falling to grounded
        if (player.isGrounded)
        {
            if (!wasGrounded && !player.isJumping)
            {
                hasLanded = true;
                player.playerAnimatorManager.PlayTargetActionAnimation("Land", false, false); // Play landing animation
            }

            if (yVelocity.y < 0)
            {
                inAirTimer = 0;
                player.animator.SetFloat("inAirTimer", inAirTimer);
                player.isJumping = false;
                fallingVelocityHasBeenSet = false;
                yVelocity.y = 0;
            }
        }
        else
        {
            if (!player.isJumping && !fallingVelocityHasBeenSet)
            {
                fallingVelocityHasBeenSet = true;
                yVelocity.y = fallStartVelocity;
            }

            inAirTimer += Time.deltaTime;
            player.animator.SetFloat("inAirTimer", inAirTimer);
            yVelocity.y += gravityForce * Time.deltaTime;
        }

        player.characterController.Move(yVelocity * Time.deltaTime);

        wasGrounded = player.isGrounded;
        hasLanded = false; // Reset after use
        HandleTilt();
        Debug.DrawRay(transform.position, moveDirection);
    }


    public void LoadData(GameData data)
    {
        player.characterController.enabled = false;
        this.transform.position = data.playerPosition;
        player.characterController.enabled = true;
    }

    public void SaveData(ref GameData data)
    {
        data.playerPosition = this.transform.position; 
    }

    private void HandleGroundCheck()
    {
        Vector3 origin = groundCheckTransform.position;

        // Slight upward offset to avoid overlapping inside floor
        origin += Vector3.up * 0.05f;

        player.isGrounded = Physics.CheckSphere(origin, groundCheckSphereRadius, groundLayer, QueryTriggerInteraction.Ignore);
    }

    public void HandleAllMovement()
    {
        HandleGroundedMovemnt();
        
        //HandleRotation();
    }

    private void HandleTilt()
    {
        float input = PlayerInputManager.instance.tiltInput;
        float desiredTilt = 0f;
        bool blockedByWall = false;

        if (input != 0)
        {
            // Determine direction: left or right
            Vector3 checkDirection = input < 0 ? -tiltRaycastOrigin.right : tiltRaycastOrigin.right;

            // Raycast to the side to detect wall
            if (Physics.Raycast(tiltRaycastOrigin.position, checkDirection, out RaycastHit hit, wallCheckDistance, wallLayer))
            {
                blockedByWall = true;
            }

            if (!blockedByWall)
            {
                holdTimer += Time.deltaTime;
                holdTimer = Mathf.Clamp(holdTimer, 0, maxHoldTime);
                float normalizedHold = holdTimer / maxHoldTime;
                desiredTilt = Mathf.Lerp(0, tiltAngle, normalizedHold) * (input < 0 ? 1 : -1);
            }
        }
        else
        {
            holdTimer = 0f;
        }

        // Smooth tilt transition (either toward target or back to 0)
        float targetTiltZ = blockedByWall ? 0f : desiredTilt;
        currentTiltZ = Mathf.Lerp(currentTiltZ, targetTiltZ, Time.deltaTime * tiltSpeed);

        // Apply rotation
        tiltBone.localRotation = Quaternion.Euler(new Vector3(0, 0, currentTiltZ));
        tiltBone2.localRotation = Quaternion.Euler(new Vector3(0, 0, currentTiltZ));
    }
    private void GetVerticalAndHorizontalInputs()
    {
        verticalMovement = PlayerInputManager.instance.verticalInput;
        horizontalMovement = PlayerInputManager.instance.horizontalInput;
        moveAmount = PlayerInputManager.instance.moveAmount;
    }

    private void HandleGroundedMovemnt()
    {
        GetVerticalAndHorizontalInputs();

        //moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
        //moveDirection = moveDirection + PlayerCamera.instance.transform.right * horizontalMovement;
        moveDirection = player.transform.forward * verticalMovement;
        moveDirection = moveDirection + player.transform.right * horizontalMovement;
        moveDirection.Normalize();
        moveDirection.y = 0;

        if (isCrouching)
        {
            if (horizontalMovement != 0)
            {
                player.characterController.Move(moveDirection * crouchStrafeSpeed * Time.smoothDeltaTime);
            }
            else
            {
                player.characterController.Move(moveDirection * crouchSpeed * Time.smoothDeltaTime);
            }   
        }
        else
        {
            if (isSprinting)
            {
                player.characterController.Move(moveDirection * sprintingSpeed * Time.smoothDeltaTime);
            }
            else
            {
                if (isRunning)
                {
                    player.characterController.Move(moveDirection * runningSpeed * Time.smoothDeltaTime);
                }
                else
                {
                    player.characterController.Move(moveDirection * walkingSpeed * Time.smoothDeltaTime);
                }
            }
        }

    }

    private void HandleRotation()
    {
        targetRotationDirection = Vector3.zero;
        targetRotationDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
        targetRotationDirection = targetRotationDirection + PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
        targetRotationDirection.Normalize();
        targetRotationDirection.y = 0;

        if (targetRotationDirection == Vector3.zero)
        {
            targetRotationDirection = transform.forward;
        }

        Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = targetRotation;
    }

    public void HandleRunning()
    {
        if (moveAmount > 0)
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }
    }

    public void HandleSprinting()
    {
        if (player.isPerformingAction)
        {
            isSprinting = false;
        }

        if (verticalMovement > 0)
        {
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }
    }

    public void AttemptToPerformJump()
    {
        if (player.isPerformingAction)
        {
            return;
        }

        if (player.isJumping)
        {
            return;
        }

        if (!player.isGrounded)
        {
            return;
        }

        player.playerAnimatorManager.PlayTargetActionAnimation("Jump", false, false);

        player.isJumping = true;


    }

    public void ApplyJumpingVelocity()
    {
        Debug.Log("jump");
        yVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravityForce);
        Debug.Log(yVelocity.y);
    }
}