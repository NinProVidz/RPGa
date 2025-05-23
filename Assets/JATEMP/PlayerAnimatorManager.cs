﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorManager : MonoBehaviour
{
    PlayerManager player;

    int vertical;
    int horizontal;

    public LayerMask groundLayer; // Set to "Default" or "Terrain"

    public Transform rightHandRayOrigin;
    public float rayDistance = 0.5f;
    public float retreatDistance = 0.1f;
    public LayerMask wallLayer;
    public float w1;
    public float w2;
    public float w3;
    public float w4;
    public float w5;

    public float animRotWeight;
    public float animRotWeight2;

    public float lookAtDist;

    private Vector3 targetHandPosition;

    [SerializeField] Transform head;

    public float angleMultiplier = 1.5f;  // tweak this as needed

    private bool shouldOverrideIK = false;


    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Ray lookAtRay = new Ray(PlayerCamera.instance.cameraObject.transform.position, PlayerCamera.instance.cameraObject.transform.forward);
            Gizmos.DrawRay(lookAtRay);
        }
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

    private void Update()
    {
        //shouldOverrideIK = false;

        //if (Physics.Raycast(rightHandRayOrigin.position, rightHandRayOrigin.forward, out RaycastHit hit, rayDistance, wallLayer))
        //{
        //    // Calculate retreat direction
        //    Vector3 retreatDir = -rightHandRayOrigin.forward;
        //    targetHandPosition = hit.point + retreatDir * retreatDistance;
        //
        //    shouldOverrideIK = true;
        //}
    }

    private void LateUpdate()
    {
       //var hips = player.animator.GetBoneTransform(HumanBodyBones.Hips);
       //hips.localRotation = Quaternion.Euler(hips.localEulerAngles.x, hips.localEulerAngles.y * animRotWeight2, hips.localEulerAngles.z * animRotWeight);
       //
       //Transform spine1 = player.animator.GetBoneTransform(HumanBodyBones.Chest);
       //spine1.localRotation = Quaternion.Euler(spine1.localEulerAngles.x, spine1.localEulerAngles.y * animRotWeight, spine1.localEulerAngles.z * animRotWeight);
       //
       //Transform spine2 = player.animator.GetBoneTransform(HumanBodyBones.UpperChest);
       //spine2.localRotation = Quaternion.Euler(spine2.localEulerAngles.x, spine2.localEulerAngles.y, spine2.localEulerAngles.z * animRotWeight);
       //
       //Transform head = player.animator.GetBoneTransform(HumanBodyBones.Head);
       //head.localRotation = Quaternion.Euler(head.localEulerAngles.x, head.localEulerAngles.y * animRotWeight2, head.localEulerAngles.z * animRotWeight);
       //
       //Transform neck = player.animator.GetBoneTransform(HumanBodyBones.Neck);
       //neck.localRotation = Quaternion.Euler(neck.localEulerAngles.x, neck.localEulerAngles.y, neck.localEulerAngles.z * animRotWeight);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        //player.animator.SetLookAtWeight(1f, 0.5f, 1f, 1f, 0f);
        //int upperLayer = player.animator.GetLayerIndex("UpperBody");


        // Get the camera forward relative to the player (local space)
        Vector3 camInPlayerSpace = player.limbs[5].transform.InverseTransformDirection(PlayerCamera.instance.cameraObject.transform.forward);

        // Calculate the raw pitch angle (positive = looking up)
        float rawPitch = Mathf.Atan2(camInPlayerSpace.y, camInPlayerSpace.z) * Mathf.Rad2Deg;

        // Apply your angle multiplier to exaggerate pitch
        
        float targetPitch = -rawPitch * angleMultiplier;

        // Rotate the player's forward vector by the exaggerated pitch around the player's local right axis
        Quaternion pitchRotation = Quaternion.AngleAxis(targetPitch, player.transform.right);
        Vector3 exaggeratedLookDir = pitchRotation * player.transform.forward;

        // Use the camera position as origin for the IK look at
        Vector3 origin = PlayerCamera.instance.cameraObject.transform.position;

        // Apply the IK Look At with exaggerated pitch
        player.animator.SetLookAtWeight(w1, w2, w3, w4, w5);
        player.animator.SetLookAtPosition(origin + exaggeratedLookDir.normalized * lookAtDist);

        //var spine = player.animator.GetBoneTransform(HumanBodyBones.Spine);
        //if (spine != null)
        //    spine.localRotation *= Quaternion.Euler(15f * 1, 0f, 0f);
        //
        //// Head slight tilt down
        //var head = player.animator.GetBoneTransform(HumanBodyBones.Head);
        //if (head != null)
        //    head.localRotation *= Quaternion.Euler(5f * 1, 0f, 0f);
        //
        //// Thighs rotate backward
        //var leftThigh = player.animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
        //var rightThigh = player.animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
        //if (leftThigh != null)
        //    leftThigh.localRotation *= Quaternion.Euler(-20f * 1, 0f, 0f);
        //if (rightThigh != null)
        //    rightThigh.localRotation *= Quaternion.Euler(-20f * 1, 0f, 0f);
        //
        //// Knees bend forward
        //var leftKnee = player.animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
        //var rightKnee = player.animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
        //if (leftKnee != null)
        //    leftKnee.localRotation *= Quaternion.Euler(40f * 1, 0f, 0f);
        //if (rightKnee != null)
        //    rightKnee.localRotation *= Quaternion.Euler(40f * 1, 0f, 0f);

    }
    public void UpdateAnimatorMovementParameters(float horizontalMovement, float verticalMovement, bool isSprinting, bool isRunning, bool isCrouching)
    {
        float horizontalAmount = horizontalMovement;
        float verticalAmount = verticalMovement;

        if (isCrouching)
        {
            verticalAmount *= 0.5f;
            horizontalAmount *= 0.5f;
            player.characterController.height = 1.3f;
            player.characterController.center = new Vector3(0, 0.65f, 0);
        }
        else
        {
            player.characterController.height = 2f;
            player.characterController.center = new Vector3(0, 1, 0);
            if (isRunning)
            {
                verticalAmount *= 2;
                horizontalAmount *= 2;
            }

            if (isSprinting)
            {
                verticalAmount = 3;
            }
        }

        player.animator.SetBool("isCrouching", isCrouching);
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
