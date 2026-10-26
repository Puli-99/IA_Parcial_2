using System.Collections.Generic;
using UnityEngine;

public class SetBaitState : State
{
    readonly Agent agent;
    readonly BaitData data;
    readonly StateMachine sm;
    float baitTimer;
    Vector3 baitPosition;
    //bool baitPlaced;
    //public bool BaitPlaced => baitPlaced;

    public SetBaitState(Agent agent, BaitData data, StateMachine stateMachine)
    {
        this.agent = agent;
        this.data = data;
        this.sm = stateMachine;
    }

    public override void Enter()
    {
        Debug.Log("BaitState Enter");
        int randomIndex = UnityEngine.Random.Range(0, data.baitSpots.Count);
        baitPosition = data.baitSpots[randomIndex].position;
        //baitPlaced = false;
        baitTimer = 0;
        agent.GetComponentInChildren<Renderer>().material.color = Color.forestGreen;
    }

    public override void Update()
    {
        agent.AddVelocity(agent.Arrive(baitPosition));
        float agentDistancetFromBaitPosition = (agent.transform.position - baitPosition).magnitude;

        if (agentDistancetFromBaitPosition <= 0.1f /*&& baitPlaced == false*/)
        {
            agent.AddVelocity(-agent.Velocity);
            baitTimer += Time.deltaTime;

            agent.GetComponentInChildren<Renderer>().material.color = Color.darkGreen;

            if (data.baitPreparationTime < baitTimer)
            {
                //baitPlaced = true;
                UnityEngine.Object.Instantiate(data.bait, baitPosition, Quaternion.identity);
                sm.ChangeState(PatrolHunterState.Patrol);
                //baitTimer = 0;
            }
        }
    }

    public override void Exit()
    {
        Debug.Log("BaitState Exit");

        if (agent.Velocity != Vector3.zero)
            agent.AddVelocity(-agent.Velocity);
    }
}

[System.Serializable]
public class BaitData
{
    public GameObject bait;
    public float baitPreparationTime;
    public List<Transform> baitSpots = new();
}