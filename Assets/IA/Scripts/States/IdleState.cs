using UnityEngine;

public class IdleState : State
{
    readonly Agent agent;

    public IdleState(Agent agent)
    {
        this.agent = agent;
    }
    public override void Enter()
    {
        //Debug.Log("IdleState Enter");

        agent.AddVelocity(-agent.Velocity);
    }

    public override void Update()
    {

    }

    public override void Exit()
    {
        //Debug.Log("IdleState Exit");
    }
}