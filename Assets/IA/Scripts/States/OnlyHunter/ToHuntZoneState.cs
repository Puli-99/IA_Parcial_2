using UnityEngine;

public class ToHuntZoneState : State
{
    readonly ToHuntZoneData data;
    readonly PathfindingAgent agent;
    readonly StateMachine sm;
    public ToHuntZoneState(PathfindingAgent agent, ToHuntZoneData data, StateMachine sm)
    {
        this.agent = agent;
        this.data = data;
        this.sm = sm;
    }
    public override void Enter()
    {
        agent.GetComponentInChildren<Renderer>().material.color = data.toHuntZoneColor;
        agent.CalculatePath(Graph.current.GetNodeFromPosition(agent.transform.position), node => node.Zone == Node.ZoneType.Hunt);
    }

    public override void Update()
    {
        if ((agent.transform.position - agent.EndNode.transform.position).magnitude <= 1.01f)
        {
            agent.StopVelocity();
            sm.ChangeState(HunterState.None);
        }
    }

    public override void Exit()
    {
    }
}

[System.Serializable]
public class ToHuntZoneData
{
    public Node entryNode;
    public Color toHuntZoneColor;
}