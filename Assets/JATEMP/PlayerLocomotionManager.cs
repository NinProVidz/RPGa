using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerLocomotionManager : MonoBehaviour, IDataPersistence
{
    PlayerManager player;
    Rigidbody rb;

    public float verticalMovement;
    public float horizontalMovement;
    public float moveAmount;

    [Header("Movement Settings")]
    private Vector3 moveDirection;
    private Vector3 targetRotationDirection;
    [SerializeField] float jumpHeight = 4f;
    [SerializeField] float walkingSpeed = 10f;
    [SerializeField] float runningSpeed = 18f;
    [SerializeField] float sprintingSpeed = 24f;
    [SerializeField] float rotationSpeed = 15f;

    public bool isSprinting;
    public bool isRunning;

    [Header("Ground Check & Jumping")]
    [SerializeField] private List<GroundDetector> groundDetectors = new List<GroundDetector>();
    [SerializeField] float gravityForce = -9.81f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform groundCheckPoint;
    [SerializeField] float groundCheckRadius = 0.3f;
    [SerializeField] float groundCheckDistance = 0.6f;

    public float inAirTimer = 0;
    private float jumpBufferTime = 0.15f;
    private float jumpBufferCounter = 0f;
    private bool isJumpQueued = false;

    public void LoadData(GameData data)
    {
        this.transform.position = data.playerPosition;
    }

    public void SaveData(ref GameData data)
    {
        data.playerPosition = this.transform.position;
    }

    private void Awake()
    {
        player = GetComponent<PlayerManager>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.drag = 3f;
    }

    private void Update()
    {
        HandleGroundCheck();
        player.animator.SetBool("isGrounded", player.isGrounded);

        if (!player.isGrounded)
        {
            inAirTimer += Time.deltaTime;
            player.animator.SetFloat("inAirTimer", inAirTimer);
        }
        else
        {
            inAirTimer = 0;
            player.animator.SetFloat("inAirTimer", 0);
        }

        // Jump buffering
        if (isJumpQueued)
        {
            jumpBufferCounter += Time.deltaTime;
            if (jumpBufferCounter > jumpBufferTime)
            {
                isJumpQueued = false;
            }
        }
    }

    private void FixedUpdate()
    {
        HandleAllMovement();

        if (isJumpQueued && player.isGrounded)
        {
            ExecuteJump();
        }

        if (!player.isGrounded)
        {
            rb.AddForce(Vector3.down * 30f, ForceMode.Force); // Extra gravity
        }
    }

    private void HandleGroundCheck()
    {
        player.isGrounded = false;

        foreach (var detector in groundDetectors)
        {
            if (detector.isTouchingGround)
            {
                player.isGrounded = true;
                break; // One contact is enough
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheckPoint.position + Vector3.down * groundCheckDistance, groundCheckRadius);
    }

    public void HandleAllMovement()
    {
        HandleMovementForces();
        HandleRotation();
    }

    private void GetVerticalAndHorizontalInputs()
    {
        verticalMovement = PlayerInputManager.instance.verticalInput;
        horizontalMovement = PlayerInputManager.instance.horizontalInput;
        moveAmount = PlayerInputManager.instance.moveAmount;
    }

    private void HandleMovementForces()
    {
        GetVerticalAndHorizontalInputs();

        moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
        moveDirection += PlayerCamera.instance.transform.right * horizontalMovement;
        moveDirection.Normalize();
        moveDirection.y = 0;

        float currentSpeed = walkingSpeed;
        if (isSprinting) currentSpeed = sprintingSpeed;
        else if (isRunning) currentSpeed = runningSpeed;

        if (moveDirection.magnitude > 0.1f)
        {
            Vector3 force = moveDirection * currentSpeed;
            rb.AddForce(force, ForceMode.Force);
        }
    }

    private void HandleRotation()
    {
        targetRotationDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
        targetRotationDirection += PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
        targetRotationDirection.Normalize();
        targetRotationDirection.y = 0;

        if (targetRotationDirection == Vector3.zero) return;

        Quaternion targetRot = Quaternion.LookRotation(targetRotationDirection);
        Quaternion smoothedRot = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        transform.rotation = smoothedRot;
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
        if (player.isPerformingAction || player.isJumping)
            return;

        // Queue the jump to be executed in FixedUpdate when grounded
        isJumpQueued = true;
        jumpBufferCounter = 0;
    }

    private void ExecuteJump()
    {
        player.isJumping = true;
        isJumpQueued = false;
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // Reset Y
        float jumpForce = Mathf.Sqrt(jumpHeight * -2f * gravityForce);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        player.playerAnimatorManager.PlayTargetActionAnimation("Jump", false, false);
    }
}
