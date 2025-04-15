using UnityEngine;

public class Tail : MonoBehaviour
{
    [Tooltip("Total links count")]
    public int count = 10;
    [Tooltip("Single link rotate time")]
    public float delay = 0.1f;
    [Tooltip("Tail look target")]
    public Transform target;
    [Tooltip("Tail links")]
    public Transform[] links;
    [Tooltip("Link forwards")]
    public Vector3[] forwards;
    [Tooltip("Link forward velocities")]
    public Vector3[] velocities;

    //private void OnValidate()
    //{
    //    if (!Application.isPlaying && gameObject.scene != null)
    //        Invoke(nameof(Create), 0);
    //}

    private void Create()
    {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);

        if (target == null)
        {
            target = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            target.name = "Target";
            target.parent = transform.parent;
            target.localScale *= 0.5f;
            target.localPosition = Vector3.forward;
        }

        links = new Transform[count];
        var parent = transform;
        for (int i = 0; i < count; i++)
        {
            links[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
            links[i].name = $"Link{i}";
            links[i].parent = parent;
            links[i].localScale = Vector3.one * (1f - 1f * i / count) / parent.lossyScale.x;
            links[i].localPosition = Vector3.back / 2f;
            parent = links[i];
        }
        links[0].localPosition = Vector3.zero;

        forwards = new Vector3[count];
        for (int i = 0; i < count; i++)
            forwards[i] = links[i].forward;

        velocities = new Vector3[count];
    }

    private void Update()
    {
        Vector3 directionToTarget = (target.position - transform.position).normalized;

        // Create a rotation that aligns -Y with the direction to the target
        Quaternion alignDown = Quaternion.FromToRotation(Vector3.down, directionToTarget);
        transform.rotation = alignDown;

        // Step 2: Smooth each link's forward vector toward the root's forward direction
        Vector3 targetForward = transform.forward;

        for (int i = 0; i < count; i++)
        {
            forwards[i] = Vector3.SmoothDamp(forwards[i], targetForward, ref velocities[i], delay);
            links[i].forward = forwards[i];
        }
    }
}