using UnityEngine;

public class HeadCameraFollow : MonoBehaviour
{
    public Transform headBone;         // the head bone in the rig
    public Vector3 positionOffset;     // tweak this for proper eye-level
    public Vector3 rotationOffset;     // if the head bone is angled strangely
    public float positionLerpSpeed = 10f;
    public float rotationLerpSpeed = 10f;

    private void LateUpdate()
    {
        // Follow position
        Vector3 targetPos = headBone.position + headBone.TransformVector(positionOffset);
        transform.position = Vector3.Lerp(transform.position, targetPos, positionLerpSpeed * Time.deltaTime);

        // Follow rotation
        Quaternion targetRot = headBone.rotation * Quaternion.Euler(rotationOffset);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationLerpSpeed * Time.deltaTime);
    }

}