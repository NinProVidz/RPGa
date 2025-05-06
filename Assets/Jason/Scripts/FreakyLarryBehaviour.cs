using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FreakyLarryBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerManager player;
    [SerializeField] EnemyAwareness awareness;

    [Header("Attack Settings")]
    [SerializeField] float cameraTurnSpeed = 2f;
    [SerializeField] float gazeDamageInterval = 1f;
    [SerializeField] float cameraLookStrength = 0.5f;
    [SerializeField] float pitchOffset = -30f;
    [SerializeField] float yawOffset = -30f;

    float lastGazeDamageTime = -Mathf.Infinity;

    [SerializeField] Transform larryLookTarget;

    [Header("Movement Settings")]
    [SerializeField] float suspicionMoveSpeed = 3f;
    [SerializeField] float chaseMoveSpeed = 6f;
    [SerializeField] float nodeReachThreshold = 0.5f;
    [SerializeField] LayerMask obstacleLayers;

    [Header("Pathfinding Settings")]
    [SerializeField] float nodeSpacing = 2f;
    [SerializeField] int gridSize = 5;
    [SerializeField] bool debugDrawNeighbors = true;

    private Vector3 targetPosition;
    private Vector3 lastKnownPlayerPosition;
    private List<Vector3> path = new List<Vector3>();
    public bool isSuspicious = false;
    public bool isChasing = false;

    private bool wasVisibleLastFrame = false;

    float repathCooldown = 1f;
    float lastPathTime = -Mathf.Infinity;
    float minDistanceToRepath = 2f;

    private Vector3 pendingPathTarget;
    private bool hasPendingPathRequest = false;
    private const float destinationChangeThreshold = 1.5f;

    private Rigidbody rb;

    private static readonly List<Vector3> neighborOffsets = GenerateOffsets();

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying || !debugDrawNeighbors) return;

        if (path != null && path.Count > 0)
        {
            Gizmos.color = isChasing ? Color.red : (isSuspicious ? Color.yellow : Color.green);

            for (int i = 0; i < path.Count; i++)
            {
                Gizmos.DrawSphere(path[i], 0.2f);
                if (i < path.Count - 1)
                    Gizmos.DrawLine(path[i], path[i + 1]);
            }
        }

        Gizmos.color = Color.cyan;
        foreach (var neighbor in GetNeighborPositions(transform.position))
        {
            Gizmos.DrawWireSphere(neighbor, 0.15f);
        }


    }

    void Start()
    {
        player = PlayerManager.instance;
        awareness = GetComponent<EnemyAwareness>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        HandlePathRequest();
        HandleStateTransitions();

        ForcePlayerLook(); // Always run this

        if (PlayerCanSeeLarry())
        {
            if (LarryIsVisibleToPlayer())
            {
                TryDamagePlayer();
            }
        }
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleStateTransitions()
    {
        if (!isChasing && awareness.awarenessLevel >= awareness.minFoundLevel)
        {
            isChasing = true;
            isSuspicious = false;
            lastKnownPlayerPosition = player.transform.position;
            RequestPath(lastKnownPlayerPosition);
        }
        else if (!isSuspicious && awareness.awarenessLevel >= awareness.minSuspicionLevel)
        {
            isSuspicious = true;
            lastKnownPlayerPosition = player.transform.position;
            Vector3 safeOffset = GetSafeRandomOffset(lastKnownPlayerPosition, 3f, 5);
            RequestPath(lastKnownPlayerPosition + safeOffset);
        }

        if (!isChasing && !isSuspicious)
        {
            PlayerCamera.instance.ClearExternalInfluences();
        }
    }

    void HandleMovement()
    {
        bool isVisibleNow = PlayerCanSeeLarry();

        if (isVisibleNow)
        {
            rb.velocity = Vector3.zero;
            

            if (!wasVisibleLastFrame)
            {
                path.Clear();
            }

            wasVisibleLastFrame = true;
            return;
        }
        else if (wasVisibleLastFrame)
        {
            lastKnownPlayerPosition = player.transform.position;
            Vector3 safeTarget = GetSafePositionNear(lastKnownPlayerPosition);

            // Force immediate path calculation
            lastPathTime = -Mathf.Infinity;
            RequestPath(safeTarget);
        }


        wasVisibleLastFrame = false;

        if (path == null || path.Count == 0)
        {
            if (isSuspicious || isChasing)
            {
                Vector3 target = isChasing ? player.limbs[2].transform.position : lastKnownPlayerPosition;

                if (Vector3.Distance(target, transform.position) > 1f)
                {
                    Vector3 safeTarget = GetSafePositionNear(target);
                    TryCalculatePath(safeTarget);
                }
            }
            return;
        }

        Vector3 nextNode = path[0];
        Vector3 direction = (nextNode - transform.position);
        float distance = direction.magnitude;

        if (distance < nodeReachThreshold)
        {
            path.RemoveAt(0);
            return;
        }

        direction.Normalize();
        float moveSpeed = isChasing ? chaseMoveSpeed : suspicionMoveSpeed;
        rb.velocity = direction * moveSpeed;

        if (direction.magnitude > 0.1f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction.normalized);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, lookRotation, Time.fixedDeltaTime * 5f));
        }
    }

    void LookAtPlayer()
    {
        if (player != null && Camera.main != null)
        {
            Vector3 directionToCamera = Camera.main.transform.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToCamera.normalized);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * 5f);
        }
    }

    Vector3 GetSafePositionNear(Vector3 pos)
    {
        const float stepSize = 0.5f;
        const int maxTries = 10;

        for (int i = 0; i < maxTries; i++)
        {
            Vector3 testPos = pos + Random.insideUnitSphere * stepSize * i;
            if (Physics.OverlapSphere(testPos, 0.4f, obstacleLayers).Length == 0)
                return testPos;
        }

        return transform.position;
    }

    Vector3 GetSafeRandomOffset(Vector3 origin, float radius, int maxAttempts)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 offset = Random.insideUnitSphere * radius;
            Vector3 testPos = origin + offset;

            if (Physics.OverlapSphere(testPos, 0.5f, obstacleLayers).Length == 0)
                return offset;
        }

        return Vector3.zero;
    }

    void ForcePlayerLook()
    {
        bool shouldInfluence = false;

        // Check conditions
        bool playerIsInRange = Vector3.Distance(player.transform.position, transform.position) < 25f;
        bool larryKnowsPlayer = isChasing;
        bool playerHasLineOfSight = PlayerCanSeeLarry();

        if (playerIsInRange && larryKnowsPlayer && playerHasLineOfSight)
        {
            shouldInfluence = true;
        }

        if (shouldInfluence)
        {
            LookAtPlayer();
            // Get the camera's pivot position (this should be the part that rotates to follow)
            Transform cameraPivot = PlayerCamera.instance.cameraPivotTransform;
            Vector3 pivotWorldPos = cameraPivot.position;

            // Get the direction vector from the camera pivot to Larry
            Vector3 toLarry = (larryLookTarget.position - pivotWorldPos).normalized;

            // Debugging: Show the direction the camera is trying to look
            Debug.DrawRay(pivotWorldPos, toLarry * 10, Color.red); // Visualize the direction towards Larry

            // Calculate the desired rotation to look at Larry
            Quaternion lookRotation = Quaternion.LookRotation(toLarry, Vector3.up);

            // Extract the yaw (horizontal rotation) and pitch (vertical rotation)
            float targetYaw = lookRotation.eulerAngles.y + yawOffset;
            float targetPitch = lookRotation.eulerAngles.x + pitchOffset ;

            // Debugging: Log the target yaw and pitch
            Debug.Log($"Target Yaw: {targetYaw}, Target Pitch: {targetPitch}");

            // Calculate delta rotation (the difference from the current camera angles)
            float yawDelta = Mathf.DeltaAngle(PlayerCamera.instance.leftAndRightLookAngle, targetYaw);
            float pitchDelta = Mathf.DeltaAngle(PlayerCamera.instance.upAndDownLookAngle, targetPitch);

            // Debugging: Log the delta yaw and pitch
            Debug.Log($"Yaw Delta: {yawDelta}, Pitch Delta: {pitchDelta}");

            // Apply external look influence to smooth the camera's rotation
            PlayerCamera.instance.ApplyExternalLookInfluence(
                this,
                yawDelta * cameraTurnSpeed * Time.deltaTime, // Horizontal influence
                pitchDelta * cameraTurnSpeed * Time.deltaTime, // Vertical influence
                cameraLookStrength // Strength of the camera look influence
            );

            // Optional: Draw a debug line to visualize the direction to Larry
            Debug.DrawLine(pivotWorldPos, larryLookTarget.position, Color.green);

            // Debugging: Show the final rotation of the camera pivot
            Debug.Log($"Final Camera Pivot Rotation: {cameraPivot.rotation.eulerAngles}");

        }
        else
        {
            // Remove external influence if the conditions are not met
            PlayerCamera.instance.RemoveExternalLookInfluence(this);
        }
    }





    bool LarryIsVisibleToPlayer()
    {
        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(transform.position);
        bool isInView = viewportPoint.z > 0 &&
                        viewportPoint.x > 0 && viewportPoint.x < 1 &&
                        viewportPoint.y > 0 && viewportPoint.y < 1;

        return isInView && awareness.awarenessLevel >= awareness.minFoundLevel;
    }

    bool PlayerCanSeeLarry()
    {
        Vector3 playerHeadPos = PlayerCamera.instance.transform.position; // Adjust as needed for player height
        Vector3 larryPos = transform.position;

        Vector3 direction = larryPos - playerHeadPos;
        float distance = direction.magnitude;

        // Raycast from player to Larry to check for obstacles
        if (!Physics.Raycast(playerHeadPos, direction.normalized, distance, obstacleLayers))
        {
            if (Vector3.Distance(larryPos, playerHeadPos) < awareness.senseRange)
            {
                return true; // Player has line of sight to Larry
            }
        }

        return false; // Obstructed
    }

    void HandlePathRequest()
    {
        if (!hasPendingPathRequest)
            return;

        if (Time.time - lastPathTime < repathCooldown)
            return;

        lastPathTime = Time.time;
        hasPendingPathRequest = false;

        List<Vector3> newPath = CalculatePath(pendingPathTarget);
        if (newPath != null && newPath.Count > 0)
        {
            path = newPath;
        }
    }

    void TryDamagePlayer()
    {
        if (Time.time - lastGazeDamageTime >= gazeDamageInterval)
        {
            lastGazeDamageTime = Time.time;
            Debug.Log("player takes damage");
        }
    }

    public void RequestPath(Vector3 destination)
    {
        if (!hasPendingPathRequest || Vector3.Distance(destination, pendingPathTarget) > destinationChangeThreshold)
        {
            pendingPathTarget = destination;
            hasPendingPathRequest = true;
        }
    }

    void TryCalculatePath(Vector3 destination)
    {
        if (Vector3.Distance(destination, transform.position) > minDistanceToRepath &&
            Time.time - lastPathTime >= repathCooldown)
        {
            lastPathTime = Time.time;

            List<Vector3> newPath = CalculatePath(destination);

            if (newPath != null && newPath.Count > 0)
            {
                path = newPath;
            }
        }
    }



    List<Vector3> CalculatePath(Vector3 destination)
    {
        List<Vector3> resultPath = new List<Vector3>();
        Node startNode = new Node(transform.position);
        Node goalNode = new Node(destination);

        List<Node> openSet = new List<Node> { startNode };
        HashSet<Node> closedSet = new HashSet<Node>();

        startNode.gCost = 0;
        startNode.hCost = Vector3.Distance(startNode.position, goalNode.position);

        int loopLimit = 300; // Maximum iterations
        int loopCount = 0;

        bool pathFound = false;

        while (openSet.Count > 0 && loopCount++ < loopLimit)
        {
            Node currentNode = openSet[0];
            foreach (Node node in openSet)
            {
                if (node.fCost < currentNode.fCost || (node.fCost == currentNode.fCost && node.hCost < currentNode.hCost))
                    currentNode = node;
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // Reached destination
            if (Vector3.Distance(currentNode.position, goalNode.position) < nodeSpacing)
            {
                goalNode.parent = currentNode;
                pathFound = true;
                break;
            }

            // Check all neighbors
            foreach (Vector3 offset in GetNeighborOffsets())
            {
                Vector3 candidatePos = currentNode.position + offset * nodeSpacing;

                if (!IsPositionValid(candidatePos, out Vector3 adjustedPos))
                    continue;

                if (!HasClearPath(currentNode.position, adjustedPos))
                    continue;

                Node neighbor = new Node(adjustedPos);

                if (closedSet.Contains(neighbor))
                    continue;

                float tentativeGCost = currentNode.gCost + Vector3.Distance(currentNode.position, adjustedPos);
                bool isInOpenSet = openSet.Contains(neighbor);

                if (!isInOpenSet || tentativeGCost < neighbor.gCost)
                {
                    neighbor.gCost = tentativeGCost;
                    neighbor.hCost = Vector3.Distance(adjustedPos, goalNode.position);
                    neighbor.parent = currentNode;

                    if (!isInOpenSet)
                        openSet.Add(neighbor);
                }
            }
        }

        path.Clear();

        if (pathFound)
        {
            Node current = goalNode;
            while (current != null)
            {
                resultPath.Insert(0, current.position);
                current = current.parent;
            }
        }
        else
        {
            Debug.LogWarning("No valid path found to destination.");
        }

        return resultPath;
    }

    bool IsPositionValid(Vector3 position, out Vector3 adjustedPosition)
    {
        adjustedPosition = position;

        if (Physics.CheckSphere(position, 0.4f, obstacleLayers))
            return false;

        if (Physics.Raycast(position + Vector3.up * 5f, Vector3.down, out RaycastHit groundHit, 10f, obstacleLayers))
        {
            adjustedPosition = new Vector3(position.x, groundHit.point.y + 1f, position.z);
            return true;
        }

        return false;
    }

    bool HasClearPath(Vector3 from, Vector3 to)
    {
        Vector3 direction = to - from;
        float distance = direction.magnitude;

        return !Physics.Raycast(from, direction.normalized, distance, obstacleLayers);
    }

    static List<Vector3> GenerateOffsets()
    {
        List<Vector3> offsets = new List<Vector3>();

        for (int x = -1; x <= 1; x++)
            for (int y = -1; y <= 1; y++)
                for (int z = -1; z <= 1; z++)
                {
                    if (x == 0 && y == 0 && z == 0)
                        continue;

                    offsets.Add(new Vector3(x, y, z));
                }

        return offsets;
    }

    List<Vector3> GetNeighborOffsets()
    {
        List<Vector3> dirs = new List<Vector3>();
        for (int x = -1; x <= 1; x++)
            for (int y = -1; y <= 1; y++)
                for (int z = -1; z <= 1; z++)
                {
                    if (x == 0 && y == 0 && z == 0)
                        continue;
                    dirs.Add(new Vector3(x, y, z));
                }
        return dirs;
    }

    List<Vector3> GetNeighborPositions(Vector3 center)
    {
        List<Vector3> positions = new List<Vector3>();
        foreach (Vector3 offset in GetNeighborOffsets())
        {
            Vector3 pos = center + offset * nodeSpacing;
            positions.Add(pos);
        }
        return positions;
    }
}
