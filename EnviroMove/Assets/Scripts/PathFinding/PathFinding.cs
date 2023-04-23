using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PathFinding
{
    private SceneEditor sceneEditor;

    public PathFinding(SceneEditor sceneEditor)
    {
        this.sceneEditor = sceneEditor;
    }
    
    public static PathFinding instance;
    private void Awake()
    {
        instance = this;
    }

    public List<Node> FindPath(Vector3 start, Vector3 target)
    {
        Node startNode = GetNodeFromPosition(start);
        Node targetNode = GetNodeFromPosition(target);

        List<Node> openList = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openList.Add(startNode);
        int count = 0;
        while (openList.Count > 0 && count < 100)
        {
            Node currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].fCost < currentNode.fCost ||
                    Math.Abs(openList[i].fCost - currentNode.fCost) < 0.01f && openList[i].hCost < currentNode.hCost)
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbor in GetNeighbours(currentNode))
            {
                if (neighbor.isBlocked || closedSet.Contains(neighbor))
                {
                    continue;
                }

                float newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newMovementCostToNeighbor < neighbor.gCost || !openList.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.fCost = neighbor.gCost + neighbor.hCost;
                    neighbor.parent = currentNode;

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }

            count++;
        }

        return null;
    }


    public Node GetNodeFromPosition(Vector3 position)
    {
        var startblockPos = sceneEditor.startBlock.transform.position;
        int x = (int)(position.x - startblockPos.x);
        int y = (int)(position.y - startblockPos.y);
        int z = (int)(position.z - startblockPos.z);
        return sceneEditor.pathGrid[x, y, z];
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbors = new List<Node>();

        var startBlockPos = sceneEditor.startBlock.transform.position;
        int x = (int)(node.position.x - startBlockPos.x);
        int y = (int)(node.position.y - startBlockPos.y);
        int z = (int)(node.position.z - startBlockPos.z);

        if (x - 1 >= 0)
        {
            neighbors.Add(sceneEditor.pathGrid[x - 1, y, z]);
        }

        if (x + 1 < sceneEditor.pathGrid.GetLength(0))
        {
            neighbors.Add(sceneEditor.pathGrid[x + 1, y, z]);
        }
        
        if (y - 1 >= 0)
        {
            neighbors.Add(sceneEditor.pathGrid[x, y - 1, z]);
        }
        
        if (y + 1 < sceneEditor.pathGrid.GetLength(1))
        {
            neighbors.Add(sceneEditor.pathGrid[x, y + 1, z]);
        }
        
        if (z - 1 >= 0)
        {
            neighbors.Add(sceneEditor.pathGrid[x, y, z - 1]);
        }
        
        if (z + 1 < sceneEditor.pathGrid.GetLength(2))
        {
            neighbors.Add(sceneEditor.pathGrid[x, y, z + 1]);
        }


        foreach (Vector3 wallOrFloor in sceneEditor.wallsAndFloors.Select(wallsAndFloor => wallsAndFloor.transform.position))
        {
            Node node1 = GetNodeFromPosition(wallOrFloor);
            node1.isBlocked = true;
        }

        return neighbors;
    }

    private float GetDistance(Node nodeA, Node nodeB)
    {
        float distanceX = Mathf.Abs(nodeA.position.x - nodeB.position.x);
        float distanceY = Mathf.Abs(nodeA.position.y - nodeB.position.y);

        if (distanceX > distanceY)
        {
            return 14 * distanceY + 10 * (distanceX - distanceY);
        }

        return 14 * distanceX + 10 * (distanceY - distanceX);
    }

    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        int count = 0;

        while (currentNode != startNode && count < 100)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
            count++;
        }

        path.Reverse();
        return path;
    }
}