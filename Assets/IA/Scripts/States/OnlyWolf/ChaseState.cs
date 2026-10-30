using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
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
        agent.GetComponentInChildren<Renderer>().material.color = data.chasingColor;
    }

    public override void Update()
    {
        agent.AddVelocity(agent.Seek(data.hunter.transform.position));

        float distanceFromHunter = (agent.transform.position - data.hunter.position).magnitude;

        if (distanceFromHunter < data.distanceToKill)
        {
            if (data.hunter.TryGetComponent<Hunter>(out Hunter hunter))
            {
                hunter.Killed();
                data.hunter = null;
                sm.ChangeState(Wolf.WolfState.Patrol);
            }
        }
    }

    public override void Exit()
    {
        FinishedOnBlockedNode();
        data.hunter = null;
    }

    void FinishedOnBlockedNode()
    {
        if (Graph.current.GetNodeFromPosition(agent.transform.position).Blocked)
        {
            Debug.Log("Reposicionando agente que termino en nodo bloqueado, para que no termine naufragando");
            List<Node> n = Graph.current.GetAllWalkableNodes.Where(node => node.Zone == Node.ZoneType.None).ToList();

            agent.transform.position = n[Random.Range(0, n.Count)].transform.position; // cambiar esto por A* hacia el nodo no bloqueado mas cercano
        }
    }
}

[System.Serializable]
public class ChaseData
{
    public Transform hunter;
    public float distanceToKill = 0.5f;
    public Color chasingColor;
}