using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode
{
    public Vector3Int position;
    public bool isWalkable;
    public float gCost, hCost;
    public GridNode parent;

    public float fCost => gCost + hCost;

    public GridNode(Vector3Int pos, bool walkable)
    {
        position = pos;
        isWalkable = walkable;
    }

    public override bool Equals(object obj)
    {
        if (obj is not GridNode other) return false;
        return position == other.position;
    }

    public override int GetHashCode()
    {
        return position.GetHashCode();
    }
}

