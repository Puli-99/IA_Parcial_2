using System.Collections.Generic;
using UnityEngine;

public class PatrolAState : State
{
    readonly PathfindingAgent agent;
    readonly PatrolAData data;

    int currentNode;

    public PatrolAState(PathfindingAgent agent, PatrolAData data)
    {
        this.agent = agent;
        this.data = data;
    }

    public override void Enter()
    {
        agent.GetComponentInChildren<Renderer>().material.color = data.patrolColor;
        agent.SetNodes(Graph.current.GetNodeFromPosition(agent.transform.position), Graph.current.GetNodeFromPosition(data.waypoints[currentNode].position));
        agent.CalculatePath(agent.StartNode, agent.EndNode);
    }

    public override void Update()
    {
        PatrolLoop();
    }

    public override void Exit()
    {
        agent.Reposition(Graph.current.GetNodeFromPosition(agent.transform.position));
        agent.StopVelocity();
    }

    void PatrolLoop()
    {
        var nextWaypoint = data.waypoints[currentNode];

        if (Vector3.Distance(nextWaypoint.position, agent.transform.position) < data.waypointCheckDistance)
        {
            currentNode = currentNode + 1 < data.waypoints.Count ? currentNode + 1 : 0;

            Node start = Graph.current.GetNodeFromPosition(agent.transform.position);
            Node end = Graph.current.GetNodeFromPosition(data.waypoints[currentNode].position);

            agent.SetNodes(start, end);
            agent.CalculatePath(agent.StartNode, agent.EndNode);
        }        
    }
}


[System.Serializable]
public class PatrolAData
{
    public List<Transform> waypoints = new();
    public float waypointCheckDistance = 1.1f;
    public Color patrolColor;
}