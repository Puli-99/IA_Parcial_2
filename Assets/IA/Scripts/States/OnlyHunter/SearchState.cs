using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SearchState : State
{
    List<Node> huntZoneNodes;
    float baitTimer = 0;

    readonly SearchData data;
    readonly PathfindingAgent agent;
    readonly StateMachine sm;
    public SearchState(PathfindingAgent agent, SearchData data, StateMachine sm)
    {
        this.agent = agent;
        this.data = data;
        this.sm = sm;
    }
    public override void Enter()
    {
        if (Graph.current.GetNodeFromPosition(agent.transform.position).Zone == Node.ZoneType.Hunt) return;

        huntZoneNodes = Graph.current.GetAllGridNodes.Where(node => node.Zone == Node.ZoneType.Hunt).ToList();
        agent.GetComponentInChildren<Renderer>().material.color = data.toHuntZoneColor;
        agent.CalculatePath(Graph.current.GetNodeFromPosition(agent.transform.position), node => node.Zone == Node.ZoneType.Hunt);

        baitTimer = 0;
        data.boid = null;
    }

    public override void Update()
    {
        if (Graph.current.GetNodeFromPosition(agent.transform.position).Zone != Node.ZoneType.Hunt) return;

        if (data.boid != null)
        {
            if (!data.boid.killed)
            {
                sm.ChangeState(HunterState.Attack);
            }
            else
            {
                sm.ChangeState(HunterState.CollectCorpse);
            }
        }

        WaitingToBait();


        if ((agent.transform.position - agent.EndNode.transform.position).magnitude <= 1.01f) // si alcanzamos el ultimo nodo del punto al que nos dirigiamos
        {
            NewRandomPoint();            
        }
    }

    void NewRandomPoint()
    {
        Vector3 randomPoint = huntZoneNodes[Random.Range(0, huntZoneNodes.Count)].transform.position;

        Node start = Graph.current.GetNodeFromPosition(agent.transform.position);
        Node end = Graph.current.GetNodeFromPosition(randomPoint);

        agent.SetNodes(start, end);
        agent.CalculatePath(start, end);
    }

    void WaitingToBait()
    {
        baitTimer += Time.deltaTime;
        if (baitTimer > data.patienceToBait)
        {
            sm.ChangeState(HunterState.SetBait);
            baitTimer = 0;
        }
    }

    public override void Exit()
    {
        agent.StopAllCoroutines();
    }
}

[System.Serializable]
public class SearchData
{
    public Node entryNode;
    public Color toHuntZoneColor;
    public Color patrolingColor;
    public float patienceToBait = 2;
    public SheepMovement boid;
}