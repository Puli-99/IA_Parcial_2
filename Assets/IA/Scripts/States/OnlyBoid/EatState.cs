using UnityEngine;

public class EatState : State
{
    public bool Finished { get; private set; }

    readonly EatData data;
    readonly SheepMovement agent;
    readonly StateMachine sm;
    public EatState(SheepMovement agent, EatData data, StateMachine sm)
    {
        this.agent = agent;
        this.data = data;
        this.sm = sm;
    }
    public override void Enter()
    {
        Debug.Log("EatState Enter");
        data.eatTimer = 0f;
        agent.GetComponentInChildren<Renderer>().material.color = Color.brown;
    }

    public override void Update()
    {
        if (data.baitTransform == null) return;

        agent.AddVelocity(agent.Arrive(data.baitTransform.position));

        if (agent.Velocity == Vector3.zero)
        {
            data.eatTimer += Time.deltaTime;
            if (data.eatTimer > data.eatTime)
            {
                Object.Destroy(data.baitTransform.gameObject);
                data.baitTransform = null;
                agent.AllAgents.Add(agent);
                sm.ChangeState(SheepState.Flock);
            }
        }
    }

    public override void Exit()
    {
        Debug.Log("EatState Exit");
    }
}

[System.Serializable]
public class EatData
{
    public Transform baitTransform;
    public float eatTimer;
    public float eatTime = 3.0f;
}