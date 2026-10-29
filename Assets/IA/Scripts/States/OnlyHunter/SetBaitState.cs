using System.Collections.Generic;
using UnityEngine;

public class SetBaitState : State
{
    readonly Agent agent;
    readonly BaitData data;
    readonly StateMachine sm;
    float baitTimer;
    Vector3 baitPosition;

    public SetBaitState(Agent agent, BaitData data, StateMachine stateMachine)
    {
        this.agent = agent;
        this.data = data;
        this.sm = stateMachine;
    }

    public override void Enter()
    {
        int randomIndex = UnityEngine.Random.Range(0, data.baitSpots.Count);
        baitPosition = data.baitSpots[randomIndex].position;
        baitPosition.y = 1;
        baitTimer = 0;
        agent.GetComponentInChildren<Renderer>().material.color = Color.forestGreen;

        if (agent is PathfindingAgent)
        {
            PathfindingAgent pAgent = agent as PathfindingAgent;
            pAgent.SetNodes(Graph.current.GetNodeFromPosition(agent.transform.position), Graph.current.GetNodeFromPosition(baitPosition));
            pAgent.CalculatePath(pAgent.StartNode, pAgent.EndNode);
        }
    }

    public override void Update()
    {
        PathfindingAgentSetsBait();
        PatrolAgentSetsBait();
    }

    public override void Exit()
    {
        if (agent.Velocity != Vector3.zero)
            agent.AddVelocity(-agent.Velocity);
    }

    void PathfindingAgentSetsBait()
    {
        if (agent is PathfindingAgent)
        {
            float agentDistancetFromBaitPosition = (agent.transform.position - baitPosition).magnitude;

            if (agentDistancetFromBaitPosition <= 1.1f)
            {
                agent.StopVelocity();
                agent.GetComponentInChildren<Renderer>().material.color = Color.darkGreen;

                baitTimer += Time.deltaTime;
                if (data.baitPreparationTime < baitTimer)
                {
                    UnityEngine.Object.Instantiate(data.bait, Graph.current.GetNodeFromPosition(baitPosition).transform.position, Quaternion.identity);
                    sm.ChangeState(HunterState.Search);
                }
            }
        }

    }

    void PatrolAgentSetsBait()
    {
        if (agent is PatrolAgent)
        {
            agent.AddVelocity(agent.Arrive(baitPosition));
            float agentDistancetFromBaitPosition = (agent.transform.position - baitPosition).magnitude;

            if (agentDistancetFromBaitPosition <= 0.1f)
            {
                agent.AddVelocity(-agent.Velocity);
                baitTimer += Time.deltaTime;

                agent.GetComponentInChildren<Renderer>().material.color = Color.darkGreen;

                if (data.baitPreparationTime < baitTimer)
                {
                    UnityEngine.Object.Instantiate(data.bait, baitPosition, Quaternion.identity);
                    sm.ChangeState(PatrolHunterState.Patrol);
                }
            }
        }
    }
}

[System.Serializable]
public class BaitData
{
    public GameObject bait;
    public float baitPreparationTime;
    public List<Transform> baitSpots = new();
}