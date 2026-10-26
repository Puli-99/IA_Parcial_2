using UnityEngine;

public class ChaseState : State
{
    readonly PathfindingAgent agent;
    readonly ChaseData data;
    readonly StateMachine sm;

    public ChaseState(PathfindingAgent agent, ChaseData data, StateMachine sm)
    {
        this.agent = agent;
        this.data = data;
        this.sm = sm;
    }

    public override void Enter()
    {

    }

    public override void Update()
    {
        agent.AddVelocity(agent.Seek(data.hunter.transform.position));

        float distanceFromHunter = (agent.transform.position - data.hunter.transform.position).magnitude;

        if (distanceFromHunter < 0.1f)
        {
            Debug.Log("Kill hunter");
        }        
    }

    public override void Exit()
    {
        data.hunter = null;
    }
}

[System.Serializable]
public class ChaseData
{
    public Transform hunter;
}