using System.Collections.Generic;
using UnityEngine;

public class AStar3D : MonoBehaviour
{
    public Transform target; // Target destination
    public float speed = 2f; // Movement speed
    public float nodeSize = 1f; // Size of each node
    public LayerMask obstacles; // Layer mask for obstacles

    private Vector3[] path;
    private int currentWaypointIndex = 0;

    void Update()
    {
        if (path == null || path.Length == 0)
            return;

        MoveAlongPath();
    }

    public void FindPath(Vector3 startPosition, Vector3 targetPosition)
    {
        Node startNode = new Node(startPosition);
        Node targetNode = new Node(targetPosition);

        // Initialize open and closed lists
        List<Node> openList = new List<Node> { startNode };
        HashSet<Node> closedList = new HashSet<Node>();

        while (openList.Count > 0)
        {
            // Get the node with the lowest fCost
            Node currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].fCost < currentNode.fCost ||
                    (openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost))
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // If the target node is reached
            if (currentNode.position == targetNode.position)
            {
                RetracePath(startNode, currentNode);
                return;
            }

            // Get neighbors
            List<Node> neighbors = GetNeighbors(currentNode);
            foreach (Node neighbor in neighbors)
            {
                if (closedList.Contains(neighbor) || !IsWalkable(neighbor.position))
                    continue;

                float newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newMovementCostToNeighbor < neighbor.gCost || !openList.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);
                }
            }
        }
    }

    private void MoveAlongPath()
    {
        if (currentWaypointIndex < path.Length)
        {
            Vector3 targetWaypoint = path[currentWaypointIndex];
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);
            if (transform.position == targetWaypoint)
                currentWaypointIndex++;
        }
    }

    private void RetracePath(Node startNode, Node endNode)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Node currentNode = endNode;
        while (currentNode != startNode)
        {
            waypoints.Add(currentNode.position);
            currentNode = currentNode.parent;
        }
        waypoints.Reverse();
        path = waypoints.ToArray();
    }

    private List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        Vector3[] directions = {
            Vector3.forward, Vector3.back, Vector3.left, Vector3.right, Vector3.up, Vector3.down
        };

        foreach (Vector3 direction in directions)
        {
            Vector3 neighborPos = node.position + direction * nodeSize;
            neighbors.Add(new Node(neighborPos));
        }

        return neighbors;
    }

    private bool IsWalkable(Vector3 position)
    {
        RaycastHit hit;
        return !Physics.Raycast(position + Vector3.up * 0.5f, Vector3.down, out hit, nodeSize, obstacles);
    }

    private float GetDistance(Node a, Node b)
    {
        return Vector3.Distance(a.position, b.position);
    }
}

public class Node
{
    public Vector3 position;
    public float gCost; // Cost from start to this node
    public float hCost; // Heuristic cost to target
    public Node parent;

    public Node(Vector3 position)
    {
        this.position = position;
    }

    public float fCost
    {
        get { return gCost + hCost; }
    }
}
