using UnityEngine;

public class Node
{
    public Vector3 position;
    public bool isBlocked;
    public float gCost;
    public float hCost;
    public float fCost;
    public Node parent;
    
    public Node(Vector3 position, bool isBlocked)
    {
        this.position = position;
        this.isBlocked = isBlocked;
    }
}