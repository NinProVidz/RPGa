using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering;

public class FreakyLarryBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerManager player;
    [SerializeField] EnemyAwareness awareness;

    [Header("Attack Settings")]
    [SerializeField] float gazeDamage = 7f;
    [SerializeField] float cameraTurnSpeed = 2f;
    [SerializeField] float gazeDamageInterval = 1f;
    [SerializeField] float cameraLookStrength = 0.5f;
    [SerializeField] float pitchOffset = -30f;
    [SerializeField] float yawOffset = -30f;

    [SerializeField] private PostProcessVolume larryVolume; // Assign in Inspector
    [SerializeField] private float postFXLerpSpeed = 2f;

    float lastGazeDamageTime = -Mathf.Infinity;

    [SerializeField] Transform larryLookTarget;

    [Header("Movement Settings")]
    [SerializeField] private float turnSpeed = 5f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float accelerationFactor = 10f;
    [SerializeField] private float nodeReachThreshold = 0.5f;
    [SerializeField] private float idleDamping = 2f;
    [SerializeField] private float idleDrag = 5f;
    [SerializeField] private float movingDrag = 0.5f;
    [SerializeField] LayerMask obstacleLayers;

    [Header("Pathfinding Settings")]
    [SerializeField] float nodeSpacing = 2f;
    [SerializeField] float obstacleOffset = 0.8f;
    [SerializeField] bool debugDrawNeighbors = true;
    [SerializeField] int loopTimes = 1000;
    [SerializeField] float suspicionSphereOffset = 5f;
    [SerializeField] int maxSafeOffsetAttempts = 5;
    [SerializeField] private float wanderInterval = 5f; // seconds between wanders
    private float wanderTimer = 0f;
    [SerializeField] private float personalSpaceDistance = 3f; // too close
    [SerializeField] private float maximumComfortDistance = 10f; // too far
    [SerializeField] private float retreatDistance = 2f; // how far to move back
    [SerializeField] private float approachDistance = 2f; // how far to move forward


    private Vector3 targetPosition;
    public Vector3 lastKnownPlayerPosition;
    public List<Vector3> path = new List<Vector3>();
    public bool isSuspicious = false;
    public bool isChasing = false;

    private bool hadLineOfSightLastFrame = false;
    private Vector3 lastSeenPlayerPosition;
    private bool hasReactedToVisibilityLoss = false;

    public bool wasVisibleLastFrame = false;

    float repathCooldown = 1f;
    float lastPathTime = -Mathf.Infinity;
    float minDistanceToRepath = 2f;

    private Vector3 pendingPathTarget;
    private bool hasPendingPathRequest = false;
    private const float destinationChangeThreshold = 1.5f;

    private Rigidbody rb;

    private static readonly List<FreakyLarryBehaviour> activeForcingLarrys = new();

    public bool isForcingLook = false;

    private static readonly List<Vector3> neighborOffsets = GenerateOffsets();

     enum LarryState { Idle, Suspicious, Chasing, Watching }
    [SerializeField] LarryState currentState = LarryState.Idle;

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

        //Gizmos.color = Color.cyan;
        //foreach (var neighbor in GetNeighborPositions(transform.position))
        //{
        //    Gizmos.DrawWireSphere(neighbor, 0.15f);
        //}
        //
        //Gizmos.color = Color.gray;
        //foreach (var nodePos in debugEvaluatedNodes)
        //{
        //    Gizmos.DrawSphere(nodePos, 0.2f);
        //}
        //
        //Gizmos.color = Color.green;
        //foreach (var pathPos in debugPathNodes)
        //{
        //    Gizmos.DrawSphere(pathPos, 0.25f);
        //}


    }

    private void OnEnable()
    {
        // Optional: reset state on enable
        isForcingLook = false;
    }

    private void OnDisable()
    {
        if (isForcingLook)
            activeForcingLarrys.Remove(this);
    }

    void SetForcingLook(bool value)
    {
        bool wasForcing = activeForcingLarrys.Count > 0;
        if (value)
        {
            if (!activeForcingLarrys.Contains(this))
                activeForcingLarrys.Add(this);
        }
        else
        {
            activeForcingLarrys.Remove(this);
        }

        bool isForcing = activeForcingLarrys.Count > 0;

        // If we just lost the very last forcing Larry, clear everything
        if (wasForcing && !isForcing)
        {
            PlayerCamera.instance.ClearExternalInfluences();
        }
        // If we changed primaries, remove the old one
        else if (isForcing && activeForcingLarrys[0] != this)
        {
            // The new primary will apply its own influence in ForcePlayerLook(),
            // so we clear any old influence from the previous primary right now:
            PlayerCamera.instance.ClearExternalInfluences();
        }
    }


    public static FreakyLarryBehaviour GetPrimaryForcingLarry()
    {
        return activeForcingLarrys.Count > 0 ? activeForcingLarrys[0] : null;
    }

    public static bool AnyLarryForcingLook()
    {
        return activeForcingLarrys.Count > 0;
    }

    void Start()
    {
        player = PlayerManager.instance;
        awareness = GetComponent<EnemyAwareness>();
        rb = GetComponent<Rigidbody>();
        larryVolume.weight = 0f;
        //Invoke("susTest", 4);
        //Invoke("susTest", 4.5f);
        //Invoke("susTest", 5);
        //Invoke("susTest", 5.5f);
        //Invoke("susTest", 6);
    }

    public void susTest()
    {
        CalculatePathTo(player.limbs[2].transform.position);
    }

    void Update()
    {
        bool canSeePlayer = PlayerCanSeeLarry();
        SetForcingLook(currentState != LarryState.Watching && !isChasing);
        float awarenessLevel = awareness.awarenessLevel;

        // 1. Always remember where you last saw the player
        if (canSeePlayer && awarenessLevel >= awareness.minFoundLevel)
        {
            lastSeenPlayerPosition = player.limbs[2].transform.position;
        }

        // 2. Detect that instant of sight *loss*
        bool justLostSight = hadLineOfSightLastFrame && !canSeePlayer;

        hadLineOfSightLastFrame = canSeePlayer;

        // --- State transitions ---
        if (canSeePlayer && awarenessLevel >= awareness.minFoundLevel)
        {
            // go into Watching
            if (currentState != LarryState.Watching)
            {
                currentState = LarryState.Watching;
                path.Clear();
                hasReactedToVisibilityLoss = false;
            }
        }
        else if (awarenessLevel >= awareness.minFoundLevel)
        {
            // transition into Chasing exactly once, right when sight breaks
            currentState = LarryState.Chasing;
            if (justLostSight && !hasReactedToVisibilityLoss)
            {
                // queue path to lastSeen
                CalculatePathTo(lastSeenPlayerPosition);
                hasReactedToVisibilityLoss = true;
                lastPathTime = Time.time;
            }
            else if (!justLostSight && ShouldRepathTo(lastSeenPlayerPosition))
            {
                // optional: repath if he moved since
                CalculatePathTo(lastSeenPlayerPosition);
                lastPathTime = Time.time;
            }
        }
        else if (awarenessLevel >= awareness.minSuspicionLevel)
        {
            // suspicion…
            if (currentState != LarryState.Suspicious || path.Count == 0)
            {
                lastKnownPlayerPosition = player.limbs[2].transform.position;
                CalculatePathTo(GetSafePositionNear(lastKnownPlayerPosition));
            }
            currentState = LarryState.Suspicious;
        }
        else
        {
            // idle or continue old path
            currentState = (path.Count > 0) ? LarryState.Suspicious : LarryState.Idle;
        }

        // --- State behaviors ---
        switch (currentState)
        {
            case LarryState.Watching:
                LookAtPlayer();
                ForcePlayerLook();
                HandleDriftAwayOrCloser();
                break;

            case LarryState.Chasing:
                ForcePlayerLook();
                FollowPath();
                break;

            case LarryState.Suspicious:
                FollowPath();
                break;

            case LarryState.Idle:
                wanderTimer += Time.deltaTime;
                if (wanderTimer >= wanderInterval)
                {
                    WanderRandomly();
                    wanderTimer = 0f;
                }
                break;
        }

        UpdatePostFXWeight();
    }

    private void HandleDriftAwayOrCloser()
    {
        Vector3 playerPos = player.limbs[2].transform.position;
        float distanceToPlayer = Vector3.Distance(transform.position, playerPos);

        if (distanceToPlayer < personalSpaceDistance)
        {
            Vector3 dirAway = (transform.position - playerPos).normalized;
            float drift = retreatDistance * Time.deltaTime;
            transform.position += dirAway * drift;
        }
        else if (distanceToPlayer > maximumComfortDistance)
        {
            Vector3 dirTo = (playerPos - transform.position).normalized;
            float drift = approachDistance * Time.deltaTime;
            transform.position += dirTo * drift;
        }
    }

    void UpdatePostFXWeight()
    {
        if (!AnyLarryForcingLook())
            larryVolume.weight = 0f;
        else if (GetPrimaryForcingLarry() == this)
        {
            float target = (currentState == LarryState.Watching) ? 1f : 0f;
            larryVolume.weight = Mathf.Clamp01(
                Mathf.Lerp(larryVolume.weight, target, Time.deltaTime * postFXLerpSpeed)
            );
        }
    }

    bool ShouldRepathTo(Vector3 target)
    {
        return Time.time - lastPathTime > repathCooldown &&
               Vector3.Distance(transform.position, target) > minDistanceToRepath;
    }

    void WanderRandomly()
    {
        if (path.Count == 0 && Time.time - lastPathTime > repathCooldown)
        {
            // 1) Find the floor point under Larry
            Vector3 origin = transform.position + Vector3.up * 2f; // start a bit above
            RaycastHit hit;
            Vector3 floorPoint = transform.position;
            if (Physics.Raycast(origin, Vector3.down, out hit, 20f, obstacleLayers))
            {
                floorPoint = hit.point;
            }

            // 2) Pick a safe random offset around that floor point
            Vector3 randomOffset = GetSafeRandomOffset(floorPoint, suspicionSphereOffset, maxSafeOffsetAttempts);

            // 3) Build your destination and path to it
            Vector3 destination = floorPoint + randomOffset;
            CalculatePathTo(destination);
            lastPathTime = Time.time;
        }
    }


    void FixedUpdate()
    {

        float targetWeight = 0f;
        if (FreakyLarryBehaviour.GetPrimaryForcingLarry() == this)
        {
            targetWeight = currentState == LarryState.Watching ? 1f : 0f;
        }

        larryVolume.weight = Mathf.Lerp(larryVolume.weight, targetWeight, Time.deltaTime * postFXLerpSpeed);


        if (currentState == LarryState.Idle && path.Count == 0)
        {
            rb.drag = (currentState == LarryState.Idle && path.Count == 0) ? idleDrag : movingDrag;
        }
    }

    private void RotateTowardsMovement()
    {
        Vector3 flatVelocity = rb.velocity;
        flatVelocity.y = 0f;

        if (flatVelocity.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(flatVelocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * turnSpeed);
        }
    }

    private void FollowPath()
    {
        if (path == null || path.Count == 0)
            return;

        Vector3 nextNode = path[0];
        Vector3 direction = nextNode - transform.position;
        //direction.y = 0f; // Ignore vertical movement if needed

        if (direction.magnitude <= nodeReachThreshold)
        {
            path.RemoveAt(0);
            return;
        }

        Vector3 desiredVelocity = direction.normalized * moveSpeed;
        Vector3 steering = desiredVelocity - rb.velocity;

        rb.AddForce(steering * accelerationFactor, ForceMode.Acceleration);
        RotateTowardsMovement();
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
            Vector3 testPos = pos + UnityEngine.Random.insideUnitSphere * stepSize * i;
            if (Physics.OverlapSphere(testPos, 0.4f, obstacleLayers).Length == 0)
                return testPos;
        }

        return transform.position;
    }

    Vector3 GetSafeRandomOffset(Vector3 origin, float radius, int maxAttempts)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 offset = UnityEngine.Random.insideUnitSphere * radius;
            Vector3 testPos = origin + offset;

            if (Physics.OverlapSphere(testPos, 0.5f, obstacleLayers).Length == 0)
                return offset;
        }

        return Vector3.zero;
    }

    void ForcePlayerLook()
    {
        bool canInfluence = PlayerCanSeeLarry()
                            && (currentState == LarryState.Watching || isChasing)
                            && Vector3.Distance(transform.position, player.limbs[2].transform.position) < 25f;

        SetForcingLook(canInfluence);
        

        // Only the primary Larry actually applies its influence
        if (activeForcingLarrys.Count == 0 || activeForcingLarrys[0] != this)
            return;

        // ... now apply influence as before ...
        LookAtPlayer();
        PlayerCamera.instance.ForceLookAtTarget(
            larryLookTarget,
            cameraTurnSpeed,
            cameraLookStrength, this
        );

        if (LarryIsVisibleToPlayer())
            TryDamagePlayer();
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

        //CalculatePathTo(pendingPathTarget);
    }

    void TryDamagePlayer()
    {
        if (Time.time - lastGazeDamageTime >= gazeDamageInterval)
        {
            lastGazeDamageTime = Time.time;
            player.GetComponent<Health>().TakeDamage(gazeDamage, DamageType.Mental);
        }
    }

    void CalculatePathTo(Vector3 destination)
    {
        path.Clear();

        Node startNode = new Node(transform.position);
        Node goalNode = new Node(destination);

        var openSet = new List<Node> { startNode };
        var openSetLookup = new Dictionary<Vector3, Node> { [startNode.position] = startNode };
        var closedSet = new HashSet<Vector3>();

        startNode.gCost = 0;
        startNode.hCost = Vector3.Distance(startNode.position, goalNode.position);

        var neighborOffsets = GetNeighborOffsets();

        int loopLimit = loopTimes;
        int loopCount = 0;

        Node finalNode = null;

        while (openSet.Count > 0 && loopCount++ < loopLimit)
        {
            Debug.Log("susys");
            // Find node with lowest fCost
            Node currentNode = openSet[0];
            foreach (var node in openSet)
            {
                if (node.fCost < currentNode.fCost ||
                    (Mathf.Approximately(node.fCost, currentNode.fCost) && node.hCost < currentNode.hCost))
                {
                    currentNode = node;
                }
            }

            openSet.Remove(currentNode);
            openSetLookup.Remove(currentNode.position);
            closedSet.Add(currentNode.position);

            // Destination reached
            if (Vector3.Distance(currentNode.position, goalNode.position) < nodeSpacing)
            {
                Debug.Log("reached");
                finalNode = currentNode;
                break;
            }

            foreach (var offset in neighborOffsets)
            {
                Vector3 neighborPos = currentNode.position + offset * nodeSpacing;

                if (closedSet.Contains(neighborPos))
                    continue;

                if (!IsPositionValid(neighborPos, out Vector3 adjustedPos))
                    continue;

                if (!HasClearPath(currentNode.position, adjustedPos))
                    continue;

                float tentativeG = currentNode.gCost + Vector3.Distance(currentNode.position, adjustedPos);

                if (!openSetLookup.TryGetValue(adjustedPos, out Node neighborNode))
                {
                    neighborNode = new Node(adjustedPos);
                    neighborNode.gCost = tentativeG;
                    neighborNode.hCost = Vector3.Distance(adjustedPos, goalNode.position);
                    neighborNode.parent = currentNode;

                    openSet.Add(neighborNode);
                    openSetLookup[adjustedPos] = neighborNode;
                }
                else if (tentativeG < neighborNode.gCost)
                {
                    neighborNode.gCost = tentativeG;
                    neighborNode.parent = currentNode;
                }
            }


        }

        Debug.Log("Exited A* loop.");

        if (finalNode != null)
        {
            Node current = finalNode;
            Node prev = null;
            while (current != null)
            {
                path.Insert(0, current.position);

                prev = current;
                current = current.parent;
            }
        }
        else
        {
            Debug.LogWarning("No valid path found to destination.");
        }
    }

    bool IsPositionValid(Vector3 position, out Vector3 adjustedPosition)
    {
        adjustedPosition = position;

        if (Physics.CheckSphere(position, obstacleOffset, obstacleLayers))
            return false;

        return true; // No snapping to ground
    }

    bool HasClearPath(Vector3 start, Vector3 end)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        direction.Normalize();

        float radius = 0.5f; // Match Larry’s collider radius
        float height = 2f;   // Match Larry’s height if capsule
        Vector3 point1 = start + Vector3.up * 0.5f;
        Vector3 point2 = start + Vector3.up * (height - 0.5f);

        return !Physics.CapsuleCast(point1, point2, radius, direction, distance, obstacleLayers);
    }

    static List<Vector3> GenerateOffsets()
    {
        List<Vector3> offsets = new List<Vector3>();
        for (int x = -1; x <= 1; x++)
            for (int y = -1; y <= 1; y++)
                for (int z = -1; z <= 1; z++)
                    if (x != 0 || y != 0 || z != 0)
                        offsets.Add(new Vector3(x, y, z));
        return offsets;
    }

    List<Vector3> GetNeighborOffsets()
    {
        // Cache this if you call it frequently
        return GenerateOffsets();
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

    public class Node
    {
        public Vector3 position;
        public float gCost;
        public float hCost;
        public float fCost => gCost + hCost;
        public Node parent;

        public Node(Vector3 pos)
        {
            position = pos;
        }

        public override bool Equals(object obj)
        {
            return obj is Node node && Vector3.Distance(position, node.position) < 0.1f;
        }

        public override int GetHashCode()
        {
            return position.GetHashCode();
        }
    }
}