using System;
using System.Collections.Generic;
using UnityEngine;

public static class Pathfinding
{
    public static float GetHeuristic(Coordinates a, Coordinates b)
    {
        int dx = Mathf.Abs(a.X - b.X);
        int dy = Mathf.Abs(a.Y - b.Y);

        return Mathf.Sqrt(dx * dx + dy * dy);
    }

    public static List<Node> AStar(Node startingNode, Node endNode) // podria actualizar usando Search
    {
        /*
        if (startingNode == null || endNode == null)
        {
            Debug.LogError("Invalid node parameters");
            return new();
        }

        return Search(startingNode, n => n == endNode, n => GetHeuristic(n.Coordinates, endNode.Coordinates));
        */

        if (startingNode == null || endNode == null)
        {
            Debug.LogError("Invalid node parameters");
            return new();
        }

        PriorityQueue<Node> frontier = new();
        frontier.Enqueue(startingNode, 0);

        Dictionary<Node, Node> cameFrom = new();
        cameFrom.Add(startingNode, null);

        Dictionary<Node, int> costSoFar = new();
        costSoFar.Add(startingNode, 0);

        HashSet<Node> processed = new();

        Node current;
        while (frontier.Count != 0)
        {
            current = frontier.Dequeue();
            if (!processed.Add(current)) continue;

            if (current == endNode) return ReconstructPath(cameFrom, current);

            foreach (Node neighbor in Graph.current.GetNeighbors(current))
            {
                if (neighbor.Blocked) continue;

                int newCost = costSoFar[current] + neighbor.Cost;
                if (cameFrom.ContainsKey(neighbor) && newCost >= costSoFar[neighbor]) continue;

                frontier.Enqueue(neighbor, newCost + GetHeuristic(neighbor.Coordinates, endNode.Coordinates));
                cameFrom[neighbor] = current;
                costSoFar[neighbor] = newCost;
                processed.Remove(neighbor);
            }
        }
        return new();
    }

    public static List<Node> AStar(Node startingNode, Func<Node, bool> isGoal)
    {
        // Sin un destino único no hay una heurística admisible: h = 0 (Dijkstra)
        return Search(startingNode, isGoal, _ => 0f);
    }

    static List<Node> Search(Node startingNode, Func<Node, bool> isGoal, Func<Node, float> heuristic)
    {
        if (startingNode == null)
        {
            Debug.LogError("Invalid node parameters");
            return new();
        }

        PriorityQueue<Node> frontier = new();
        frontier.Enqueue(startingNode, 0);

        Dictionary<Node, Node> cameFrom = new();
        cameFrom.Add(startingNode, null);

        Dictionary<Node, int> costSoFar = new();
        costSoFar.Add(startingNode, 0);

        HashSet<Node> processed = new();

        Node current;
        while (frontier.Count != 0)
        {
            current = frontier.Dequeue();
            if (!processed.Add(current)) continue;

            if (isGoal(current)) return ReconstructPath(cameFrom, current);

            foreach (Node neighbor in Graph.current.GetNeighbors(current))
            {
                if (neighbor.Blocked) continue;

                int newCost = costSoFar[current] + neighbor.Cost;
                if (cameFrom.ContainsKey(neighbor) && newCost >= costSoFar[neighbor]) continue;

                frontier.Enqueue(neighbor, newCost + heuristic(neighbor));
                cameFrom[neighbor] = current;
                costSoFar[neighbor] = newCost;
                processed.Remove(neighbor);
            }
        }
        return new();
    }

    static List<Node> ReconstructPath(Dictionary<Node, Node> cameFrom, Node current)
    {
        List<Node> path = new();
        while (current != null)
        {
            path.Add(current);
            current = cameFrom[current];
        }
        path.Reverse();
        return path;
    }
}