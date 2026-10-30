using UnityEngine;
public class ExitState : State
{
    readonly PathfindingAgent agent;
    readonly ExitData data;
    readonly StateMachine sm;

    public ExitState(PathfindingAgent agent, ExitData data, StateMachine sm)
    {
        this.agent = agent;
        this.data = data;
        this.sm = sm;
    }

    public override void Enter()
    {
        agent.GetComponentInChildren<Renderer>().material.color = data.toExitColor;
        agent.CalculatePath(Graph.current.GetNodeFromPosition(agent.transform.position), node => node.Zone == Node.ZoneType.Exit);
    }
    public override void Update()
    {
        if ((agent.transform.position - agent.EndNode.transform.position).magnitude <= 1.01f)
        {
            agent.StopVelocity();
            agent.GetComponentInChildren<Renderer>().material.color = data.onExitColor;
            // Hola Mati :D aca deberia llamar al respawn (hunter.Killed()), pero no lo hice para que maten al hunter mas facil
            // y puedas ver mas rapido el comportamiento de cuando muere el hunter
        }
    }
    public override void Exit()
    {
        
    }
}

[System.Serializable]
public class ExitData
{
    public Color toExitColor;
    public Color onExitColor;
}