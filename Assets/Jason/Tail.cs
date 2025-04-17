using UnityEngine;

public class Tail : MonoBehaviour
{
    public Transform[] segs;      // Tail segments (from base to tip)
    public Transform body;        // Moving/rotating parent object

    public float lag = 5f;        // How slowly the ghost follows (lower = more drag)
    public float sway = 1f;       // Sway intensity from body movement
    public float rotSpeed = 10f;  // How fast tail segments align to sway

    private Vector3 ghostPos;
    private Quaternion ghostRot;

    void Start()
    {
        ghostPos = body.position;
        ghostRot = body.rotation;
    }

    void Update()
    {
        // Track the body's world position & rotation, even if it's the parent
        Vector3 currentPos = body.position;
        Quaternion currentRot = body.rotation;

        // Smooth trailing ghost (in world space)
        ghostPos = Vector3.Lerp(ghostPos, currentPos, Time.deltaTime * lag);
        ghostRot = Quaternion.Slerp(ghostRot, currentRot, Time.deltaTime * lag);

        // Calculate sway direction from ghost offset
        Vector3 swayDir = -(currentPos - ghostPos) * sway + (currentRot * Vector3.back - ghostRot * Vector3.back) * sway;

        if (swayDir.sqrMagnitude < 0.0001f) return;

        // Apply to each segment
        for (int i = 0; i < segs.Length; i++)
        {
            // Get parent-local sway direction
            Vector3 localSway = segs[i].parent.InverseTransformDirection(swayDir.normalized);

            // Project onto local XZ plane (remove Y component)
            localSway.y = 0;

            if (localSway.sqrMagnitude > 0.001f)
            {
                // Get current local forward direction (tail direction)
                Vector3 currentDir = segs[i].localRotation * Vector3.up;

                // Create target rotation that only bends in horizontal plane
                Quaternion targetRot = Quaternion.FromToRotation(Vector3.up, localSway.normalized);
                segs[i].localRotation = Quaternion.Slerp(segs[i].localRotation, targetRot, Time.deltaTime * rotSpeed);
            }
        }
    }
}