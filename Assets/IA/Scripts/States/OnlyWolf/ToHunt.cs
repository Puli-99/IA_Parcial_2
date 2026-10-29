using UnityEngine;

public class ToHunt : State
{
    readonly PathfindingAgent agent;
    readonly ToHuntData data;
    readonly StateMachine sm;

    Vector3 hunterInitialPos;

    public ToHunt(PathfindingAgent agent, ToHuntData data, StateMachine sm)
    {
        this.agent = agent;
        this.data = data;
        this.sm = sm;
    }

    public override void Enter()
    {
        agent.GetComponentInChildren<Renderer>().material.color = data.joiningColor;
        hunterInitialPos = data.hunter.position;
        agent.SetNodes(Graph.current.GetNodeFromPosition(agent.transform.position), Graph.current.GetNodeFromPosition(data.hunter.position));
        agent.CalculatePath(agent.StartNode, agent.EndNode);
    }
    public override void Update()
    {
        float distanceFromInitialPos = (data.hunter.position - hunterInitialPos).magnitude;

        if (distanceFromInitialPos > data.recalculatePathDistance)
        {
            RecalculateTargetPathfinding();
        }
    }

    void RecalculateTargetPathfinding()
    {
        hunterInitialPos = data.hunter.position;

        Node start = Graph.current.GetNodeFromPosition(agent.transform.position);
        Node end = Graph.current.GetNodeFromPosition(data.hunter.position);

        agent.SetNodes(start, end);
        agent.CalculatePath(agent.StartNode, agent.EndNode);
    }

    public override void Exit()
    {
        agent.StopAllCoroutines();
        data.hunter = null;
    }
}

[System.Serializable]
public class ToHuntData
{
    public Transform hunter;
    public float recalculatePathDistance = 5;
    public Color joiningColor;
}