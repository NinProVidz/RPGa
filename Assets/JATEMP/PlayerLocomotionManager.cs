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

    [Header("Ledge Climb Settings")]
    public float ledgeDetectRange = 1.5f;
    public float ledgeMinHeight = 1f;
    public float ledgeMaxHeight = 2.5f;
    public float ledgeCheckRadius = 0.3f;
    public LayerMask ledgeLayer;
    public float climbUpDuration = 1.0f; // Match animation length
    public float climbUpStandDuration = 1.0f; // Match animation length

    public float climbTime = 0.5f;

    public float ledgeOffset;

    public bool isClimbingLedge = false;
    private Vector3 ledgeClimbTarget;

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

    public Vector3 ledgePoint;

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
            Gizmos.DrawLine(groundCheckTransform.position + Vector3.up * 0.1f, groundCheckTransform.position + Vector3.down * groundCheckDistance);
        }

        if (tiltRaycastOrigin != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(tiltRaycastOrigin.position, tiltRaycastOrigin.right * wallCheckDistance);
            Gizmos.DrawRay(tiltRaycastOrigin.position, -tiltRaycastOrigin.right * wallCheckDistance);
        }

        // SphereCast origin
        Vector3 sphereOrigin = transform.position + Vector3.up * ledgeMaxHeight;
        Vector3 direction = transform.forward;

        // Show SphereCast
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(sphereOrigin, ledgeCheckRadius);
        Gizmos.DrawLine(sphereOrigin, sphereOrigin + direction * ledgeDetectRange);

        Vector3 lowOrigin = transform.position + Vector3.up * ledgeMinHeight;
        Gizmos.DrawLine(lowOrigin, lowOrigin + direction * ledgeDetectRange);

        // Check if wall is hit
        if (Physics.SphereCast(sphereOrigin, ledgeCheckRadius, direction, out RaycastHit wallHit, ledgeDetectRange, ledgeLayer))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(wallHit.point, 0.05f);

            // Step forward off the wall + small upward offset
            Vector3 ledgeCheckOrigin = wallHit.point + wallHit.normal * 0.2f + Vector3.up * 0.1f;

            // Show raycast origin
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(ledgeCheckOrigin, 0.03f);

            // Downward raycast range
            float raycastLength = (ledgeMaxHeight - ledgeMinHeight) + 0.1f;
            Vector3 down = Vector3.down * raycastLength;

            Gizmos.DrawLine(ledgeCheckOrigin, ledgeCheckOrigin + down);

            if (Physics.Raycast(ledgeCheckOrigin, Vector3.down, out RaycastHit ledgeHit, raycastLength, ledgeLayer))
            {
                float heightDifference = ledgeHit.point.y - transform.position.y;

                // Show min/max height range for debug
                Vector3 basePosition = transform.position;
                Vector3 minHeightPos = basePosition + Vector3.up * ledgeMinHeight;
                Vector3 maxHeightPos = basePosition + Vector3.up * ledgeMaxHeight;

                Gizmos.color = Color.blue;
                Gizmos.DrawLine(minHeightPos, maxHeightPos);
                Gizmos.DrawWireSphere(minHeightPos, 0.05f);
                Gizmos.DrawWireSphere(maxHeightPos, 0.05f);

                // If climbable
                if (heightDifference > ledgeMinHeight && heightDifference < ledgeMaxHeight)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(ledgeHit.point, 0.05f);

                    // Target position the player will go to
                    Vector3 climbTarget = ledgeHit.point + Vector3.up * 1.1f + transform.forward * 0.3f;
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireSphere(climbTarget, 0.1f);
                }
            }
        }
    }

    private void LateUpdate()
    {
        HandleGroundCheck();
        player.animator.SetBool("isGrounded", player.isGrounded);

        if (player.isGrounded)
        {
            if (!wasGrounded && !player.isJumping)
            {
                hasLanded = true;
                player.playerAnimatorManager.PlayTargetActionAnimation("Land", false, false);
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
        hasLanded = false;
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
        Vector3 origin = groundCheckTransform.position + Vector3.up * 0.05f;
        player.isGrounded = Physics.CheckSphere(origin, groundCheckSphereRadius, groundLayer, QueryTriggerInteraction.Ignore);
    }

    public void HandleAllMovement()
    {
        HandleGroundedMovemnt();
    }

    private void HandleTilt()
    {
        float input = PlayerInputManager.instance.tiltInput;
        float desiredTilt = 0f;
        bool blockedByWall = false;

        if (input != 0)
        {
            Vector3 checkDirection = input < 0 ? -tiltRaycastOrigin.right : tiltRaycastOrigin.right;
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

        float targetTiltZ = blockedByWall ? 0f : desiredTilt;
        currentTiltZ = Mathf.Lerp(currentTiltZ, targetTiltZ, Time.deltaTime * tiltSpeed);
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

        moveDirection = player.transform.forward * verticalMovement + player.transform.right * horizontalMovement;
        moveDirection.Normalize();
        moveDirection.y = 0;

        float speed = walkingSpeed;
        if (isCrouching)
        {
            speed = horizontalMovement != 0 ? crouchStrafeSpeed : crouchSpeed;
        }
        else if (isSprinting)
        {
            speed = sprintingSpeed;
        }
        else if (isRunning)
        {
            speed = runningSpeed;
        }

        player.characterController.Move(moveDirection * speed * Time.smoothDeltaTime);
    }

    public void HandleRunning()
    {
        isRunning = moveAmount > 0;
    }

    public void HandleSprinting()
    {
        isSprinting = !player.isPerformingAction && verticalMovement > 0;
    }

    public void AttemptToPerformJump()
    {
        if (!player.isPerformingAction && !player.isJumping && player.isGrounded)
        {
            player.playerAnimatorManager.PlayTargetActionAnimation("Jump", false, false);
            player.isJumping = true;
        }
    }

    public void ApplyJumpingVelocity()
    {
        yVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravityForce);
    }

    public void TryClimbLedge(Vector3 ledgePoint)
    {
        this.ledgePoint = ledgePoint;
        StartCoroutine(ClimbLedgeCoroutine(ledgePoint));
    }

    public bool DetectLedge(out Vector3 ledgePoint)
    {
        ledgePoint = Vector3.zero;
        float startHeight = ledgeMaxHeight;
        float castHeight = ledgeMaxHeight - ledgeMinHeight;

        Vector3 forward = transform.forward;
        Vector3 baseOrigin = transform.position + Vector3.up * startHeight;

        int forwardSteps = 6; // More steps = more coverage
        float totalDistance = ledgeDetectRange;

        float bestY = float.MinValue;
        Vector3 bestHit = Vector3.zero;
        bool foundLedge = false;

        for (int i = 0; i <= forwardSteps; i++)
        {
            float t = i / (float)forwardSteps;
            Vector3 stepOrigin = baseOrigin + forward * (t * totalDistance);

            if (Physics.Raycast(stepOrigin, Vector3.down, out RaycastHit hit, castHeight, ledgeLayer))
            {
                float heightDifference = transform.position.y - hit.point.y;

                if (heightDifference < -ledgeMinHeight && heightDifference > -ledgeMaxHeight)
                {
                    Vector3 clearanceCheck = hit.point + Vector3.up * 1f;
                    if (!Physics.CheckSphere(clearanceCheck, 0.25f, ledgeLayer))
                    {
                        if (hit.point.y > bestY)
                        {
                            bestY = hit.point.y;
                            bestHit = hit.point;
                            foundLedge = true;
                        }
                    }
                }
            }
        }

        if (foundLedge)
        {
            ledgePoint = bestHit;
            return true;
        }

        return false;
    }


    private IEnumerator ClimbLedgeCoroutine(Vector3 targetPoint)
    {
        isClimbingLedge = true;
        player.isPerformingAction = true;
        player.characterController.enabled = false;

        player.playerAnimatorManager.PlayTargetActionAnimation("ClimbUp", true, false);

        Vector3 startPosition = transform.position;
        Vector3 climbPosition = targetPoint + Vector3.up * ledgeOffset;
        Vector3 endPosition = targetPoint + Vector3.up * 1.1f + transform.forward * 0.3f;
        float elapsed = 0f;
        
        while (elapsed < climbUpDuration)
        {
            transform.position = Vector3.Lerp(startPosition, climbPosition, elapsed / climbUpDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(climbTime);

        elapsed = 0f;

        while (elapsed < climbUpStandDuration)
        {
            transform.position = Vector3.Lerp(climbPosition, endPosition, elapsed / climbUpStandDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;

        player.characterController.enabled = true;
        player.isPerformingAction = false;
        isClimbingLedge = false;
    }
}
