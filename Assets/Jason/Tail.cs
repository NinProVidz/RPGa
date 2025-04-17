using UnityEngine;

public class Tail : MonoBehaviour
{
    public Transform[] segs;        // Tail bones, ordered from root to tip
    public Transform body;          // The main moving object

    public float stiffness = 5f;    // How fast it follows ghost target
    public float maxDist = 0.5f;    // Max distance each bone can stretch from parent
    public float rotSpeed = 10f;    // How fast bones rotate to look at target
    public float returnStrength = 0.5f; // How fast it returns to default pose

    private Vector3[] defaultLocalPositions; // Local-to-body default offsets
    private Vector3[] ghostPositions;        // World-space "desired" positions

    void Start()
    {
        int count = segs.Length;
        defaultLocalPositions = new Vector3[count];
        ghostPositions = new Vector3[count];

        for (int i = 0; i < count; i++)
        {
            // Store original local position relative to the body
            defaultLocalPositions[i] = body.InverseTransformPoint(segs[i].position);
            ghostPositions[i] = segs[i].position;
        }
    }

    void LateUpdate()
    {
        for (int i = 0; i < segs.Length; i++)
        {
            Transform seg = segs[i];
            Transform parent = seg.parent;
            if (parent == null) continue;

            // Calculate default world-space position (body-relative)
            Vector3 worldDefaultPos = body.TransformPoint(defaultLocalPositions[i]);

            // Blend ghost position toward the default position
            ghostPositions[i] = Vector3.Lerp(ghostPositions[i], worldDefaultPos, Time.deltaTime * returnStrength);

            // Constrain ghost to maxDist from parent
            Vector3 parentPos = parent.position;
            Vector3 toTarget = ghostPositions[i] - parentPos;

            if (toTarget.magnitude > maxDist)
            {
                toTarget = toTarget.normalized * maxDist;
                ghostPositions[i] = parentPos + toTarget;
            }

            // Smoothly follow ghost position
            seg.position = Vector3.Lerp(seg.position, ghostPositions[i], Time.deltaTime * stiffness);

            // Rotate the bone to face its target (Z forward assumed)
            Vector3 lookDir = ghostPositions[i] - parent.position;
            if (lookDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(lookDir, parent.up);
                seg.rotation = Quaternion.Slerp(seg.rotation, targetRot, Time.deltaTime * rotSpeed);
            }
        }
    }
}
