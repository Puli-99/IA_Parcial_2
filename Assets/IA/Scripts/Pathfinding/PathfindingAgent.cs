using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingAgent : Agent
{
    List<Node> path;
    protected Coroutine walkCoroutine;
    [SerializeField] protected Node startNode, endNode;
    public Node StartNode => startNode;
    public Node EndNode => endNode;


    public void SetNodes(Node a, Node b)
    {
        startNode = a;
        endNode = b;
    }

    public void Reposition(Node node)
    {
        if (walkCoroutine != null) StopCoroutine(walkCoroutine);
        transform.position = node.transform.position + Vector3.up;
    }

    public void CalculatePath(Node startNode, Node endNode)
    {
        path = Pathfinding.AStar(startNode, endNode);
        if (path != null && path.Count > 0)
        {
            if (walkCoroutine != null) StopCoroutine(walkCoroutine);
            walkCoroutine = StartCoroutine(WalkPath(path));
        }
    }

    public void CalculatePath(Node startNode, Func<Node, bool> endNode)
    {
        path = Pathfinding.AStar(startNode, endNode);
        this.endNode = path[path.Count - 1];
        // path[path.Count - 1].name este es mi endNode
        if (path != null && path.Count > 0)
        {
            if (walkCoroutine != null) StopCoroutine(walkCoroutine);
            walkCoroutine = StartCoroutine(WalkPath(path));
        }
    }

    IEnumerator WalkPath(List<Node> path)
    {
        foreach (Node node in path)
        {
            Vector3 target = node.transform.position + Vector3.up;
            while ((transform.position - target).sqrMagnitude > 0.001f)
            {
                AddVelocity(Seek(target));
                yield return null;
            }
        }
    }
}