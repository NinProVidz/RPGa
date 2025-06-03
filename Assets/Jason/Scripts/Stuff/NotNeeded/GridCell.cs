using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell
{
    public Vector3 worldPosition;
    public bool isWalkable;
    public List<GridCell> visibleNeighbors; // To store neighbors that are visible

    public GridCell(Vector3 position, bool walkable)
    {
        worldPosition = position;
        isWalkable = walkable;
        visibleNeighbors = new List<GridCell>();
    }
}

