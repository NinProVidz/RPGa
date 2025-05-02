using System.Collections.Generic;
using UnityEngine;

public class FreakyLarryBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerManager player;
    [SerializeField] EnemyAwareness awareness;

    [Header("Movement Settings")]
    [SerializeField] float suspicionMoveSpeed = 3f;
    [SerializeField] float chaseMoveSpeed = 6f;
    [SerializeField] float nodeReachThreshold = 0.5f;
    [SerializeField] LayerMask obstacleLayers;
    [SerializeField] float minFlyHeight = 1f;
    [SerializeField] float maxFlyHeight = 10f;

    [Header("Pathfinding Settings")]
    [SerializeField] float nodeSpacing = 2f;
    [SerializeField] int gridSize = 5; // Defines how large the search space is (gridSize * nodeSpacing)

    private Vector3 targetPosition;
    private Vector3 lastKnownPlayerPosition;
    private List<Vector3> path = new List<Vector3>();
    public bool isSuspicious = false;
    public bool isChasing = false;

    

    private Rigidbody rb;

    void OnDrawGizmosSelected()
    {
        if (path != null && path.Count > 0)
        {
            // Draw path nodes
            Gizmos.color = isChasing ? Color.red : (isSuspicious ? Color.yellow : Color.green);

            for (int i = 0; i < path.Count; i++)
            {
                Gizmos.DrawSphere(path[i], 0.2f);

                if (i < path.Count - 1)
                {
                    Gizmos.DrawLine(path[i], path[i + 1]);
                }
            }
        }

        // Draw surrounding possible nodes (neighbors)
        if (Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            List<Vector3> neighbors = GetNeighborPositions(transform.position);

            foreach (var neighbor in neighbors)
            {
                if (neighbor.y >= minFlyHeight && neighbor.y <= maxFlyHeight)
                {
                    Gizmos.DrawWireSphere(neighbor, 0.15f);
                }
            }
        }
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

    void Start()
    {
        player = PlayerManager.instance;
        awareness = GetComponent<EnemyAwareness>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        HandleStateTransitions();
        
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
            lastKnownPlayerPosition = player.limbs[2].transform.position;
            CalculatePathTo(lastKnownPlayerPosition);
        }
        else if (!isSuspicious && awareness.awarenessLevel >= awareness.minSuspicionLevel)
        {
            isSuspicious = false;
            isSuspicious = true;
            lastKnownPlayerPosition = player.limbs[2].transform.position;
            Vector3 randomOffset = Random.insideUnitSphere * 3f;
            randomOffset.y = Mathf.Clamp(randomOffset.y, -1f, 2f);
            CalculatePathTo(lastKnownPlayerPosition + randomOffset);
        }

    }

    void HandleMovement()
{
    if (path == null || path.Count == 0)
    {
        if (isSuspicious || isChasing)
        {
            Vector3 target = isChasing ? player.limbs[2].transform.position : lastKnownPlayerPosition;

            // Only recalculate if player moved significantly
            if (Vector3.Distance(target, transform.position) > 1f)
            {
                CalculatePathTo(target);
            }
        }
        return;
    }

    Vector3 nextNode = path[0];
    Vector3 direction = (nextNode - transform.position);
    float distance = direction.magnitude;

    // Proactive obstacle avoidance
    //AvoidObstacles(ref direction);

    if (distance < nodeReachThreshold)
    {
        path.RemoveAt(0);
        return;
    }

    direction.Normalize();

    float moveSpeed = isChasing ? chaseMoveSpeed : suspicionMoveSpeed;
    Vector3 move = direction * moveSpeed * Time.fixedDeltaTime;

    if (move.magnitude > distance)
        move = direction * distance;

    rb.MovePosition(rb.position + move);

    if (direction.magnitude > 0.1f)
    {
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, lookRotation, Time.fixedDeltaTime * 5f));
    }
}

void AvoidObstacles(ref Vector3 direction)
{
    // Raycast in the direction of movement to detect obstacles ahead
    RaycastHit hit;
    if (Physics.Raycast(transform.position, direction, out hit, nodeSpacing, obstacleLayers))
    {
        // If an obstacle is detected, we push away from it
        Vector3 avoidanceDirection = Vector3.Reflect(direction, hit.normal);
        avoidanceDirection.y = 0f; // Keep horizontal movement for flying

        // Apply a steering force to avoid the obstacle
        direction = avoidanceDirection;
    }
}

    

    void CalculatePathTo(Vector3 destination)
    {
        Node startNode = new Node(transform.position);
        Node goalNode = new Node(destination);

        List<Node> openSet = new List<Node> { startNode };
        HashSet<Node> closedSet = new HashSet<Node>();

        startNode.gCost = 0;
        startNode.hCost = Vector3.Distance(startNode.position, goalNode.position);

        int loopLimit = 1000;
        int loopCount = 0; 

        while (openSet.Count > 0 && loopCount++ < loopLimit)
        {
            Node currentNode = openSet[0];

            foreach (Node node in openSet)
            {
                if (node.fCost < currentNode.fCost || (node.fCost == currentNode.fCost && node.hCost < currentNode.hCost))
                {
                    currentNode = node;
                }
            }

            if (Vector3.Distance(currentNode.position, goalNode.position) < nodeSpacing)
            {
                goalNode.parent = currentNode;
                break;
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            foreach (Vector3 dir in GetNeighborOffsets())
            {
                Vector3 neighborPos = currentNode.position + dir * nodeSpacing;

                if (Physics.CheckSphere(neighborPos, 0.5f, obstacleLayers))
                    continue;

                // Height clamping (optional)
                //if (neighborPos.y < minFlyHeight || neighborPos.y > maxFlyHeight)
                //continue;

                // First check if it's blocked in movement direction
                if (Physics.Raycast(currentNode.position, dir.normalized, nodeSpacing, obstacleLayers))
                    continue;

                //  Raycast DOWN to find ground
                Ray groundCheck = new Ray(neighborPos + Vector3.up * 5f, Vector3.down);
                if (Physics.Raycast(groundCheck, out RaycastHit groundHit, 10f, obstacleLayers))
                {
                    neighborPos.y = groundHit.point.y + 1f; // Stay 1 meter above ground
                }

                Node neighbor = new Node(neighborPos);

                if (closedSet.Contains(neighbor))
                    continue;

                float tentativeGCost = currentNode.gCost + Vector3.Distance(currentNode.position, neighbor.position);
                bool isInOpenSet = openSet.Contains(neighbor);


                if (!isInOpenSet || tentativeGCost < neighbor.gCost)
                {
                    neighbor.gCost = tentativeGCost;
                    neighbor.hCost = Vector3.Distance(neighbor.position, goalNode.position);
                    neighbor.parent = currentNode;

                    if(!isInOpenSet)
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        if (loopCount >= loopLimit)
        {
            Debug.LogWarning("Pathfinding loop hit limit — possible unreachable destination.");
        }

        // Build final path
        path.Clear();
        Node pathNode = goalNode;
        while (pathNode != null)
        {
            path.Insert(0, pathNode.position);
            pathNode = pathNode.parent;
        }
    }

    Node GetLowestCostNode(List<Node> nodes, Node goal)
    {
        Node bestNode = nodes[0];
        float bestCost = (nodes[0].position - goal.position).sqrMagnitude;

        foreach (Node node in nodes)
        {
            float cost = (node.position - goal.position).sqrMagnitude;
            if (cost < bestCost)
            {
                bestCost = cost;
                bestNode = node;
            }
        }
        return bestNode;
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

    class Node
    {
        public Vector3 position;
        public Node parent;
        public float gCost;
        public float hCost;

        public float fCost => gCost + hCost;

        public Node(Vector3 pos)
        {
            position = pos;
        }

        public override bool Equals(object obj)
        {
            if (obj is Node other)
                return Vector3.Distance(position, other.position) < 0.1f;
            return false;
        }

        public override int GetHashCode()
        {
            return position.GetHashCode();
        }
    }
}
