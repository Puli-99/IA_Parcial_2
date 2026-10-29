using UnityEngine;

public class EscapeState : State
{
    public bool IsSafe { get; private set; }

    readonly EscapeData data;
    readonly Agent agent;
    readonly StateMachine sm;
    public EscapeState(Agent agent, EscapeData data, StateMachine sm)
    {
        this.agent = agent;
        this.data = data;
        this.sm = sm;
    }

    public override void Enter()
    {
        data.escapingTime = 0f;
        agent.GetComponentInChildren<Renderer>().material.color = Color.violet;
    }

    public override void Update()
    {
        agent.AddVelocity(agent.Evade(data.hunter));

        float distanceFromHunter = (data.hunter.transform.position - agent.transform.position).magnitude;

        if (distanceFromHunter > data.safeRange)
            data.escapingTime += Time.deltaTime;
        else
            data.escapingTime = 0f;

        if (data.escapingTime > data.safeTime)
        {
            if (agent is SheepMovement sheep)
            {
                data.hunter = null;
                sheep.ClearHunter();
                sheep.AllAgents.Add(sheep);
                sm.ChangeState(SheepState.Flock);
            }
        }
    }

    public override void Exit()
    {
    }
}

[System.Serializable]
public class EscapeData
{
    public Agent hunter;
    public float safeRange = 15.0f;
    public float safeTime = 3.0f;
    public float escapingTime;
}