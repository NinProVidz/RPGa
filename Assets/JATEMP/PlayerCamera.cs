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
    private float cameraZPositon;
    private float targetCameraZPosition;

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
        cameraZPositon = cameraObject.transform.localPosition.z;
    }

    public void HandleAllCameraActions()
    {
        if (player != null)
        {
            HandleFollowTarget();
            HandleRotations();
            HandleCollisions();
        }
    }

    private void HandleFollowTarget()
    {
        Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
        transform.position = targetCameraPosition;
    }

    private void HandleRotations()
    {
        leftAndRightLookAngle += (PlayerInputManager.instance.cameraHorizontalInput * leftAndRightRotationSpeed) * Time.deltaTime;

        //leftAndRightLookAngle = NormalizeAngle(leftAndRightLookAngle);
        upAndDownLookAngle -= (PlayerInputManager.instance.cameraVerticalInput * upAndDownRotationSpeed) * Time.deltaTime;
        upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

        Vector3 cameraRotation = Vector3.zero;
        Quaternion targetRotation;

        cameraRotation.y = leftAndRightLookAngle;
        targetRotation = Quaternion.Euler(cameraRotation);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * cameraAngleSmoothSpeed);
        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, Time.deltaTime * cameraAngleSmoothSpeed);

        cameraRotation = Vector3.zero;
        cameraRotation.x = upAndDownLookAngle;
        targetRotation = Quaternion.Euler(cameraRotation);
        cameraPivotTransform.localRotation = Quaternion.Slerp(cameraPivotTransform.localRotation, targetRotation, Time.deltaTime * cameraAngleSmoothSpeed);
    }

    private void HandleCollisions()
    {
        Debug.Log("a");
        targetCameraZPosition = cameraZPositon;
        RaycastHit hit;
        Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
        direction.Normalize();

        if (Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetCameraZPosition), collideWithLayers))
        {
            Debug.Log("s");
            float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
            targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
        }

        if (Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)
        {
            //targetCameraZPosition = -cameraCollisionRadius;
        }

        cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition, 0.1f);
        //cameraObject.transform.localPosition = cameraObjectPosition;
        cameraObject.transform.localPosition = new Vector3(cameraObject.transform.localPosition.x, cameraObject.transform.localPosition.y, cameraObjectPosition.z);
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
