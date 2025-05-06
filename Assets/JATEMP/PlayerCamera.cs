using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;

    public Camera cameraObject;
    public PlayerManager player;
    public Transform targetTransform;
    [SerializeField] public Transform cameraPivotTransform;

    [Header("Camera Settings")]
    [SerializeField] private float cameraSmoothSpeed = 1;
    [SerializeField] private float cameraAngleSmoothSpeed = 1;
    [SerializeField] private float leftAndRightRotationSpeed = 220;
    [SerializeField] private float upAndDownRotationSpeed = 220;
    [SerializeField] float minimumPivot = -30;
    [SerializeField] float maximumPivot = 60;
    [SerializeField] float cameraCollisionRadius = 0.2f;
    [SerializeField] LayerMask collideWithLayers;

    [Header("Camera Values")]
    private Vector3 cameraVelocity;
    private Vector3 cameraObjectPosition;
    [SerializeField] public float leftAndRightLookAngle;
    [SerializeField] public float upAndDownLookAngle;
    public bool isPlayerManuallyControllingCamera { get; private set; }

    public class LookInfluence
    {
        public Vector2 current;
        public Vector2 target;
    }

    public Dictionary<object, LookInfluence> externalLookInfluences = new Dictionary<object, LookInfluence>();

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;

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
    }

    public void HandleAllCameraActions()
    {
        if (player != null)
        {
            HandleRotations();
        }
    }

    private void HandleRotations()
    {
        float horizontalInput = PlayerInputManager.instance.cameraHorizontalInput;
        float verticalInput = PlayerInputManager.instance.cameraVerticalInput;

        leftAndRightLookAngle += horizontalInput * leftAndRightRotationSpeed * Time.deltaTime;
        upAndDownLookAngle -= verticalInput * upAndDownRotationSpeed * Time.deltaTime;

        // Apply external influences (AFTER input & clamping)
        Vector2 totalInfluence = Vector2.zero;
        foreach (var influence in externalLookInfluences.Values)
        {
            totalInfluence += influence.target;
        }

        leftAndRightLookAngle += totalInfluence.x;
        upAndDownLookAngle += totalInfluence.y;

        // Clamp pitch AFTER all changes
        upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

        // Normalize horizontal angle
        leftAndRightLookAngle = NormalizeAngle360(leftAndRightLookAngle);

        // Apply rotation to player object
        Quaternion targetYaw = Quaternion.Euler(0f, leftAndRightLookAngle, 0f);
        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetYaw, Time.deltaTime * cameraAngleSmoothSpeed); ;

        // Apply pitch to camera pivot
        Quaternion targetPitch = Quaternion.Euler(upAndDownLookAngle, 0f, 0f);
        cameraPivotTransform.localRotation = Quaternion.Slerp(cameraPivotTransform.localRotation, targetPitch, Time.deltaTime * cameraAngleSmoothSpeed); ;
    }

    public void ApplyExternalLookInfluence(object source, float horizontal, float vertical, float strength = 1f)
    {
        if (!externalLookInfluences.ContainsKey(source))
            externalLookInfluences[source] = new LookInfluence();

        externalLookInfluences[source].target = new Vector2(horizontal, vertical) * strength;
    }

    public void RemoveExternalLookInfluence(object source)
    {
        if (externalLookInfluences.ContainsKey(source))
        {
            externalLookInfluences[source].target = Vector2.zero;
        }
    }

    public void ClearExternalInfluences()
    {
        externalLookInfluences.Clear();
    }

    private float NormalizeAngle360(float angle)
    {
        angle %= 360f;
        return (angle < 0f) ? angle + 360f : angle;
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        else if (angle < -180f) angle += 360f;
        return angle;
    }
}
