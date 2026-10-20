using UnityEngine;

public class EscapeState : State
{
    public bool IsSafe { get; private set; }

    readonly EscapeData data;
    readonly SheepMovement agent;
    public EscapeState(SheepMovement agent, EscapeData data)
    {
        this.agent = agent;
        this.data = data;
    }

    public override void Enter()
    {
        Debug.Log("EscapeState Enter");
        data.escapingTime = 0f;
        IsSafe = false;
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

        IsSafe = data.escapingTime > data.safeTime;
    }

    public override void Exit()
    {
        Debug.Log("EscapeState Exit");
    }
}

[System.Serializable]
public class EscapeData
{
    public PatrolAgent hunter;
    public float safeRange = 15.0f;
    public float safeTime = 3.0f;
    public float escapingTime;
}