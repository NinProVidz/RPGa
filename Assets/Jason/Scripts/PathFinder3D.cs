using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder3D : MonoBehaviour
{
    [SerializeField] Grid3D grid;

    public PathFinder3D(Grid3D grid)
    {
        this.grid = grid;
    }

    public List<Vector3> FindPath(Vector3Int start, Vector3Int end)
    {
        Debug.Log("sus");
        var openSet = new PriorityQueue<GridNode>();
        var closedSet = new HashSet<GridNode>();

        GridNode startNode = grid.GetNode(start);
        GridNode endNode = grid.GetNode(end);
        
        if (startNode == null || endNode == null) return null;
        Debug.Log("sigma");
        openSet.Enqueue(startNode, 0);

        startNode.gCost = 0;
        startNode.hCost = Heuristic(startNode, endNode);

        while (openSet.Count > 0)
        {
            var currentNode = openSet.Dequeue();

            if (currentNode == endNode)
                Debug.Log("grrr");
                return RetracePath(startNode, endNode);

            closedSet.Add(currentNode);

            foreach (var neighbor in grid.GetNeighbors(currentNode))
            {
                if (closedSet.Contains(neighbor)) continue;

                float tentativeG = currentNode.gCost + Vector3Int.Distance(currentNode.position, neighbor.position);
                if (tentativeG < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = tentativeG;
                    neighbor.hCost = Heuristic(neighbor, endNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Enqueue(neighbor, neighbor.fCost);
                }
            }
        }
        Debug.Log("diddy");
        return null;
    }

    float Heuristic(GridNode a, GridNode b)
    {
        return Vector3Int.Distance(a.position, b.position); // Can use Manhattan or Euclidean
    }

    List<Vector3> RetracePath(GridNode start, GridNode end)
    {
        var path = new List<Vector3>();
        var current = end;
        while (current != start)
        {
            path.Add(current.position);
            current = current.parent;
        }
        path.Reverse();
        return path.Select(p => (Vector3)p * grid.nodeSize).ToList();
    }
}
