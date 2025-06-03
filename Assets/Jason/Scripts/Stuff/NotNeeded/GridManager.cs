using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public Vector3 gridOrigin = Vector3.zero;
    public int gridSizeX = 100;
    public int gridSizeY = 10;
    public int gridSizeZ = 100;
    public float nodeSpacing = 1f;
    public LayerMask unwalkableMask;
    public LayerMask obstacleLayers;

    private Dictionary<Vector3Int, GridCell> grid = new Dictionary<Vector3Int, GridCell>();
    private List<Vector3> neighborOffsets;

    public int dynamicGridSizeX = 50; // Adjusted grid size based on the Larry’s position
    public int dynamicGridSizeZ = 50; // Same here for Z

    private void Awake()
    {
        neighborOffsets = GenerateOffsets();
    }

    public GridCell GetOrCreateCell(Vector3 worldPosition)
    {
        Vector3Int index = WorldToGridIndex(worldPosition);

        if (!grid.TryGetValue(index, out GridCell cell))
        {
            Vector3 cellPosition = GridIndexToWorld(index);
            bool isWalkable = !Physics.CheckSphere(cellPosition, nodeSpacing * 0.45f, unwalkableMask);
            cell = new GridCell(cellPosition, isWalkable);
            grid[index] = cell;
        }

        return cell;
    }

    public bool GetCellFromWorldPosition(Vector3 worldPosition, out GridCell cell)
    {
        Vector3Int index = WorldToGridIndex(worldPosition);
        return grid.TryGetValue(index, out cell);
    }

    public Vector3Int WorldToGridIndex(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / nodeSpacing);
        int y = Mathf.FloorToInt(worldPosition.y / nodeSpacing);
        int z = Mathf.FloorToInt(worldPosition.z / nodeSpacing);
        return new Vector3Int(x, y, z);
    }



    // Precompute visibility for the entire grid
    public void PrecomputeVisibility()
    {
        foreach (var kvp in grid)
        {
            GridCell current = kvp.Value;
            if (!current.isWalkable) continue;

            foreach (var offset in neighborOffsets)
            {
                Vector3 neighborPos = current.worldPosition + offset * nodeSpacing;

                // This creates neighbor if it doesn't exist
                GridCell neighbor = GetOrCreateCell(neighborPos);
                if (!neighbor.isWalkable) continue;

                Vector3 dir = (neighbor.worldPosition - current.worldPosition).normalized;
                float dist = Vector3.Distance(current.worldPosition, neighbor.worldPosition);

                if (!Physics.Raycast(current.worldPosition, dir, dist, obstacleLayers))
                {
                    current.visibleNeighbors.Add(neighbor);
                }
            }
        }

        Debug.Log("Grid visibility precomputed.");
    }

    public void UpdateOrigin(Vector3 newCenter)
    {
        gridOrigin = new Vector3(
            Mathf.Floor(newCenter.x / nodeSpacing) * nodeSpacing,
            Mathf.Floor(newCenter.y / nodeSpacing) * nodeSpacing,
            Mathf.Floor(newCenter.z / nodeSpacing) * nodeSpacing
        );
    }

    public Vector3 GridIndexToWorld(Vector3Int localIndex)
    {
        return gridOrigin + new Vector3(localIndex.x, localIndex.y, localIndex.z) * nodeSpacing;
    }


    // Generate neighbor offsets (8 directions in 3D space)
    private List<Vector3> GenerateOffsets()
    {
        List<Vector3> offsets = new List<Vector3>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if (x != 0 || y != 0 || z != 0) // Avoid the current cell itself
                    {
                        offsets.Add(new Vector3(x, y, z));
                    }
                }
            }
        }
        return offsets;
    }
}
