using UnityEngine;

public class DeadState : State
{
    readonly Agent agent;

    public DeadState(Agent agent)
    {
        this.agent = agent;
    }

    public override void Enter()
    {
        //Debug.Log("DeadState Enter");
        agent.GetComponentInChildren<Renderer>().material.color = Color.black;
    }

    public override void Update()
    {
        if (agent.Velocity != Vector3.zero)
            agent.AddVelocity(-agent.Velocity);
    }

    public override void Exit()
    {
        //Debug.Log("DeadState Exit");
    }
}