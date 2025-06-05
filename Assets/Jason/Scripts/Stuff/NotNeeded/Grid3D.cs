using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid3D : MonoBehaviour
{
    public int width, height, depth;
    public GridNode[,,] grid;
    public float nodeSize;
    public LayerMask obstacleMask;

    private void Start()
    {
        grid = new GridNode[width, height, depth];
        CreateGrid();
    }

    public Grid3D(int width, int height, int depth, float nodeSize, LayerMask obstacleMask)
    {
        this.width = width;
        this.height = height;
        this.depth = depth;
        this.nodeSize = nodeSize;
        this.obstacleMask = obstacleMask;
        grid = new GridNode[width, height, depth];
        CreateGrid();
    }

    void CreateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    Vector3 worldPos = new Vector3(x, y, z) * nodeSize;
                    bool walkable = !Physics.CheckBox(worldPos, Vector3.one * nodeSize / 2f, Quaternion.identity, obstacleMask);
                    grid[x, y, z] = new GridNode(new Vector3Int(x, y, z), walkable);
                }
            }
        }
    }

    public GridNode GetNode(Vector3Int pos)
    {
        if (IsInsideBounds(pos))
            return grid[pos.x, pos.y, pos.z];
        return null;
    }

    public bool IsInsideBounds(Vector3Int pos) =>
        pos.x >= 0 && pos.x < width &&
        pos.y >= 0 && pos.y < height &&
        pos.z >= 0 && pos.z < depth;

    public List<GridNode> GetNeighbors(GridNode node)
    {
        var neighbors = new List<GridNode>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if (x == 0 && y == 0 && z == 0) continue;
                    var neighborPos = node.position + new Vector3Int(x, y, z);
                    var neighbor = GetNode(neighborPos);
                    if (neighbor != null && neighbor.isWalkable)
                        neighbors.Add(neighbor);
                }
            }
        }
        return neighbors;
    }
}
