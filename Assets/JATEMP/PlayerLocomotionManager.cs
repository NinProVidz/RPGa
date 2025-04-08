using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotionManager : MonoBehaviour
{
    PlayerManager player;

    public float verticalMovement;
    public float horizontalMovement;
    public float moveAmount;

    [Header("Movement Settings")]
    private Vector3 moveDirection;
    private Vector3 targetRotationDirection;
    [SerializeField] float jumpHeight = 4f;
    [SerializeField] float walkingSpeed = 2;
    [SerializeField] float runningSpeed = 5;
    [SerializeField] float sprintingSpeed = 6.5f;
    [SerializeField] float rotationSpeed = 15f;

    public bool isSprinting;
    public bool isRunning;

    [Header("Ground Check & Jumping")]
    [SerializeField] float gravityForce = -5.55f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundCheckSphereRadius = 1;
    [SerializeField] protected Vector3 yVelocity;
    [SerializeField] protected float groundedVelocity = -20;
    [SerializeField] protected float fallStartVelocity = -5;
    bool fallingVelocityHasBeenSet = false;
    public float inAirTimer = 0;

    private void Awake()
    {
        player = GetComponent<PlayerManager>();
    }

    private void Update()
    {
        HandleGroundCheck();
        player.animator.SetBool("isGrounded", player.isGrounded);

        if (player.isGrounded)
        {
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

            inAirTimer = inAirTimer + Time.deltaTime;
            player.animator.SetFloat("inAirTimer", inAirTimer);
            yVelocity.y += gravityForce * Time.deltaTime;

            
        }

        player.characterController.Move(yVelocity * Time.deltaTime);


    }

    private void HandleGroundCheck()
    {
        player.isGrounded = Physics.CheckSphere(player.transform.position, groundCheckSphereRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(player.transform.position, groundCheckSphereRadius);
    }
    public void HandleAllMovement()
    {
        HandleGroundedMovemnt();
        //HandleRotation();
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

        moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
        moveDirection = moveDirection + PlayerCamera.instance.transform.right * horizontalMovement;
        moveDirection.Normalize();
        moveDirection.y = 0;

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
