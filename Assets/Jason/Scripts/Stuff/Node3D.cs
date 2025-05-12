using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node3D : MonoBehaviour
{
    public Vector3 worldPosition;
    public bool walkable;

    public int gCost; // Cost from start node
    public int hCost; // Heuristic (estimated distance to end)
    public Node3D parent;

    public int fCost { get { return gCost + hCost; } }

    public Node3D(Vector3 worldPos, bool walkable)
    {
        this.worldPosition = worldPos;
        this.walkable = walkable;
    }
}
