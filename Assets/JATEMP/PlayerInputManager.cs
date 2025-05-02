using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;
    public PlayerManager player;

    public bool inputEnabled = true;

    private PlayerControls playerControls; 

    [Header("Player Movement Input")]
    [SerializeField] Vector2 movementInput;
    public float horizontalInput;
    public float verticalInput;
    public float moveAmount;

    public float tiltInput;

    [Header("Camera Movement Input")]
    [SerializeField] float smoothingSpeed = 10f;
    [SerializeField] Vector2 cameraInput;
    public float cameraHorizontalInput;
    public float cameraVerticalInput;

    [Header("Player Action Input")]
    [SerializeField] bool runInput = false;
    [SerializeField] bool sprintInput = false;
    [SerializeField] bool jumpInput = false;
    [SerializeField] bool crouchInput = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        SceneManager.activeSceneChanged += OnSceneChange;

        instance.enabled = true;
    }

    private void OnSceneChange(Scene oldScene, Scene newScene)
    {
        
        if (newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
        {
            instance.enabled = true;
        }
        else
        {
            instance.enabled = false;
        }
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();  
        }

        playerControls.Enable();

        if (inputEnabled)
        {
            //playerControls.PlayerMovement.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
            //playerControls.PlayerMovement.Movement.canceled += ctx => movementInput = Vector2.zero;

            playerControls.PlayerCamera.CameraControls.performed += i => cameraInput = i.ReadValue<Vector2>();
            playerControls.PlayerCamera.CameraControls.canceled += i => cameraInput = Vector2.zero;

            playerControls.PlayerActions.Jump.performed += i => jumpInput = true;
            //playerControls.PlayerActions.Jump.canceled += i => jumpInput = false;

            playerControls.PlayerActions.Run.performed += i => runInput = true;
            playerControls.PlayerActions.Run.canceled += i => runInput = false;

            playerControls.PlayerActions.Sprint.performed += i => sprintInput = true;
            playerControls.PlayerActions.Sprint.canceled += i => sprintInput = false;

            playerControls.PlayerActions.Crouch.performed += i => crouchInput = true;
            playerControls.PlayerActions.Crouch.canceled += i => crouchInput = false;

            //playerControls.PlayerMovement.Tilt.performed += ctx => tiltInput = ctx.ReadValue<Vector2>().x;
            //playerControls.PlayerMovement.Tilt.canceled += ctx => tiltInput = 0;
        }
    }

    private void OnDisable()
    {
        playerControls.Disable(); 
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChange;
    }

    private void Update()
    {
        if (inputEnabled)
        {
            movementInput = playerControls.PlayerMovement.Movement.ReadValue<Vector2>();
            tiltInput = playerControls.PlayerMovement.Tilt.ReadValue<Vector2>().x;
        }
            

        HandlePlayerMovementInput();
        HandleCameraMovementInput();
        HandleRunInput();
        HandleSprintInput();
        HandleJumpInput();
        HandleCrouchInput();
    }

    private void HandlePlayerMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

        if (moveAmount <= 0.5 && moveAmount > 0)
        {
            moveAmount = 0.5f;
        }
        else if (moveAmount > 0.5f && moveAmount <= 1)
        {
            moveAmount = 1;
        }

        if (player == null)
        {
            return;
        }

        player.playerAnimatorManager.UpdateAnimatorMovementParameters(horizontalInput, verticalInput, player.playerLocomotionManager.isSprinting, player.playerLocomotionManager.isRunning, player.playerLocomotionManager.isCrouching);
    }

    private float smoothCameraHorizontalInput;
    private float smoothCameraVerticalInput;

    private float deadZone = 0.1f;  // Small dead zone to ignore minor inputs

    private void HandleCameraMovementInput()
    {
        // Apply dead zone to filter out tiny movements
        cameraHorizontalInput = Mathf.Abs(cameraInput.x) < deadZone ? 0f : cameraInput.x;
        cameraVerticalInput = Mathf.Abs(cameraInput.y) < deadZone ? 0f : cameraInput.y;

        // Smooth the input values to prevent sudden jumps or jittering
        smoothCameraHorizontalInput = Mathf.Lerp(smoothCameraHorizontalInput, cameraHorizontalInput, Time.deltaTime * smoothingSpeed);  // 10f is smoothing speed, adjust as needed
        smoothCameraVerticalInput = Mathf.Lerp(smoothCameraVerticalInput, cameraVerticalInput, Time.deltaTime * smoothingSpeed);

        // Use the smoothed inputs in the rest of your logic
        cameraHorizontalInput = smoothCameraHorizontalInput;
        cameraVerticalInput = smoothCameraVerticalInput;

    }

    private void HandleCrouchInput()
    {
        player.playerLocomotionManager.isCrouching = crouchInput;
    }

    private void HandleRunInput()
    {
        if (runInput)
        {
            player.playerLocomotionManager.HandleRunning();
        }
        else
        {
            player.playerLocomotionManager.isRunning = false;
        }
    }

    private void HandleSprintInput()
    {
        if (sprintInput)
        {
            player.playerLocomotionManager.HandleSprinting();
        }
        else
        {
            player.playerLocomotionManager.isSprinting = false;
        }
    }

    private void HandleJumpInput()
    {
        if (jumpInput)
        {
            jumpInput = false;

            player.playerLocomotionManager.AttemptToPerformJump();
        }
    }
}
