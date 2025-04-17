using UnityEngine;

public class TentacleFollow : MonoBehaviour
{
    public Transform[] tentacleBones; // Assign from root to tip
    public float followSpeed = 5f;
    public float twistAmount = 30f;

    private Vector3[] previousPositions;

    void Start()
    {
        previousPositions = new Vector3[tentacleBones.Length];
        for (int i = 0; i < tentacleBones.Length; i++)
        {
            previousPositions[i] = tentacleBones[i].position;
        }
    }

    void LateUpdate()
    {
        for (int i = 1; i < tentacleBones.Length; i++)
        {
            Vector3 targetPos = previousPositions[i - 1];
            Vector3 currentPos = tentacleBones[i].position;

            // Smoothly follow the previous bone
            Vector3 newPos = Vector3.Lerp(currentPos, targetPos, Time.deltaTime * followSpeed);
            tentacleBones[i].position = newPos;

            // Optional: apply twist/rotation
            Vector3 direction = newPos - tentacleBones[i - 1].position;
            if (direction.sqrMagnitude > 0.0001f)
            {
                tentacleBones[i - 1].rotation = Quaternion.LookRotation(direction) *
                    Quaternion.Euler(0, 0, Mathf.Sin(Time.time * twistAmount));
            }

            previousPositions[i] = newPos;
        }
    }
}
