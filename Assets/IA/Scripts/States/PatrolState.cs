using UnityEngine;
using System.Collections.Generic;
public class PatrolState : State
{
    readonly PatrolAgent agent;
    readonly PatrolData data;
    int currentNode;
    int direction = 1;

    public PatrolState(PatrolAgent agent, PatrolData patrolData)
    {
        this.agent = agent;
        this.data = patrolData;
    }

    public override void Enter()
    {
        Debug.Log("PatrolState Enter");
        agent.GetComponentInChildren<Renderer>().material.color = Color.red;

        if (agent.Velocity != Vector3.zero)
            agent.AddVelocity(-agent.Velocity);
    }

    public override void Update()
    {
        PatrolLoop();
    }

    public override void Exit()
    {
        Debug.Log("PatrolState Exit");
    }

    void PatrolLoop()
    {
        var nextWaypoint = data.waypoints[currentNode];

        if (Vector3.Distance(nextWaypoint.position, data.transform.position) < data.waypointCheckDistance)
        {
            currentNode = currentNode + 1 < data.waypoints.Count ? currentNode + 1 : 0;
        }

        var dir = nextWaypoint.position - data.transform.position;
        data.transform.position += (dir.normalized * agent.MaxSpeed) * Time.deltaTime;
    }

    void PatrolPingPong()
    {
        var nextWaypoint = data.waypoints[currentNode];

        if (Vector3.Distance(nextWaypoint.position, data.transform.position) < data.waypointCheckDistance)
        {
            currentNode += direction;

            if (currentNode >= data.waypoints.Count)
            {
                currentNode = data.waypoints.Count - 1;
                direction = -1;
            }

            else if (currentNode < 0)
            {
                currentNode = 1;
                direction = 1;
            }
        }

        var dir = nextWaypoint.position - data.transform.position;
        data.transform.position += (dir.normalized * agent.MaxSpeed) * Time.deltaTime;
    }

}


[System.Serializable]
public class PatrolData
{
    public List<Transform> waypoints = new();
    public float waypointCheckDistance = 0.1f;
    public Transform transform;
}