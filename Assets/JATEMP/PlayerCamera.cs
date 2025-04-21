using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;

    public Camera cameraObject;
    public PlayerManager player;
    public Transform targetTransform;
    [SerializeField] Transform cameraPivotTransform;

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
    [SerializeField] float leftAndRightLookAngle;
    [SerializeField] float upAndDownLookAngle;

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
            //HandleFollowTarget();
            HandleRotations();
        }
    }

    private void HandleFollowTarget()
    {
        Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
        transform.position = targetCameraPosition;
    }

    private void HandleRotations()
    {
        leftAndRightLookAngle += PlayerInputManager.instance.cameraHorizontalInput * leftAndRightRotationSpeed * Time.deltaTime;
        leftAndRightLookAngle = NormalizeAngle360(leftAndRightLookAngle);

        //leftAndRightLookAngle = NormalizeAngle(leftAndRightLookAngle);
        upAndDownLookAngle -= (PlayerInputManager.instance.cameraVerticalInput * upAndDownRotationSpeed) * Time.deltaTime;
        upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

        Vector3 cameraRotation = Vector3.zero;
        Quaternion targetRotation;

        cameraRotation.y = leftAndRightLookAngle;
        targetRotation = Quaternion.Euler(cameraRotation);
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * cameraAngleSmoothSpeed);
        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, Time.deltaTime * cameraAngleSmoothSpeed);

        cameraRotation = Vector3.zero;
        cameraRotation.x = upAndDownLookAngle;
        targetRotation = Quaternion.Euler(cameraRotation);
        cameraPivotTransform.localRotation = Quaternion.Slerp(cameraPivotTransform.localRotation, targetRotation, Time.deltaTime * cameraAngleSmoothSpeed);
    }


    private float NormalizeAngle360(float angle)
    {
        angle %= 360f;
        if (angle < 0f)
            angle += 360f;
        return angle;
    }

    private float NormalizeAngle(float angle)
    {
        // Normalize the angle to the range of -180 to 180
        if (angle > 180f)
        {
            angle -= 360f;
        }
        else if (angle < -180f)
        {
            angle += 360f;
        }
        return angle;
    }
}