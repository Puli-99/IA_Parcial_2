using UnityEngine;

public class ToHuntZoneState : State
{
    ToHuntZoneData data;
    PathfindingAgent agent;
    public ToHuntZoneState(PathfindingAgent agent, ToHuntZoneData data)
    {
        this.agent = agent;
        this.data = data;
    }
    public override void Enter()
    {
        agent.StartPathfinding(data.entryNode);
    }

    public override void Update()
    {

    }

    public override void Exit()
    {

    }
}

[System.Serializable]
public class ToHuntZoneData
{
    public Node entryNode;
}