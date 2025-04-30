using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid3D : MonoBehaviour
{
    private float nodeSize;
    private LayerMask obstacleMask;

    private Dictionary<Vector3Int, Node3D> nodes = new Dictionary<Vector3Int, Node3D>();

    public Grid3D(float nodeSize, LayerMask obstacleMask)
    {
        this.nodeSize = nodeSize;
        this.obstacleMask = obstacleMask;
    }

    public Node3D GetOrCreateNode(Vector3 worldPosition)
    {
        Vector3Int gridPos = WorldToGrid(worldPosition);

        if (!nodes.ContainsKey(gridPos))
        {
            Vector3 nodeWorldPos = GridToWorld(gridPos);
            bool walkable = !Physics.CheckBox(nodeWorldPos, Vector3.one * nodeSize * 0.5f, Quaternion.identity, obstacleMask);
            nodes[gridPos] = new Node3D(nodeWorldPos, walkable);
        }

        return nodes[gridPos];
    }

    public List<Node3D> GetNeighbors(Node3D node)
    {
        List<Node3D> neighbors = new List<Node3D>();

        Vector3Int basePos = WorldToGrid(node.worldPosition);

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if (x == 0 && y == 0 && z == 0)
                        continue;

                    Vector3Int neighborPos = basePos + new Vector3Int(x, y, z);
                    Node3D neighborNode = GetOrCreateNode(GridToWorld(neighborPos));

                    if (neighborNode.walkable)
                        neighbors.Add(neighborNode);
                }
            }
        }

        return neighbors;
    }

    private Vector3Int WorldToGrid(Vector3 worldPos)
    {
        return Vector3Int.RoundToInt(worldPos / nodeSize);
    }

    private Vector3 GridToWorld(Vector3Int gridPos)
    {
        return gridPos;
    }
}
