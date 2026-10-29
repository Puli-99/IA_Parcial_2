using UnityEngine;

public class SafeZoneState : State
{
    readonly PathfindingAgent agent;
    readonly SafeZoneData data;
    readonly StateMachine sm;

    float timer;

    public SafeZoneState(PathfindingAgent agent, SafeZoneData data, StateMachine sm)
    {
        this.agent = agent;
        this.data = data;
        this.sm = sm;
    }

    public override void Enter()
    {
        timer = 0;
        agent.GetComponentInChildren<Renderer>().material.color = data.toSafeZoneColor;
        agent.CalculatePath(Graph.current.GetNodeFromPosition(agent.transform.position), node => node.Zone == Node.ZoneType.Safe);
    }
    public override void Update()
    {
        if ((agent.transform.position - agent.EndNode.transform.position).magnitude <= 1.01f)
        {
            agent.StopVelocity();
            agent.GetComponentInChildren<Renderer>().material.color = data.inSafeZoneColor;

            timer += Time.deltaTime;
            if (timer > data.safeZoneWaitTime)
            {
                sm.ChangeState(HunterState.Search);
            }
        }
    }
    public override void Exit()
    {
        agent.StopAllCoroutines();
    }
}

[System.Serializable]
public class SafeZoneData
{
    public Color toSafeZoneColor;
    public Color inSafeZoneColor;
    public float safeZoneWaitTime = 3;
}