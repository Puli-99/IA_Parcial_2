using System;
using System.Collections.Generic;
using UnityEngine;

public class FlockState : State
{
    readonly FlockData data;
    readonly SheepMovement agent;


    public FlockState(SheepMovement agent, FlockData data)
    {
        this.agent = agent;
        this.data = data;
    }
    public override void Enter()
    {
        //Debug.Log("FlockState Enter");
        agent.GetComponentInChildren<Renderer>().material.color = Color.white;
        Vector3 random = new Vector3(UnityEngine.Random.Range(10, -10), 0, UnityEngine.Random.Range(10, -10)).normalized;
        agent.AddVelocity(random * agent.MaxSpeed);
    }

    public override void Update()
    {
        agent.AddVelocity(Flocking());
    }

    public override void Exit()
    {
        //Debug.Log("FlockState Exit");
    }

    Vector3 Flocking()
    {
        return Separation() * data.separationWeight + Alignment() * data.alignmentWeight + Cohesion() * data.cohesionWeight;
    }

    Vector3 Separation() => GetAverageNeighbourVector(agent.AllAgents, data.separationRange, sheep => (sheep.transform.position - data.transform.position), desired => agent.CalculateSteering(-desired.normalized * agent.MaxSpeed));

    Vector3 Alignment() => GetAverageNeighbourVector(agent.AllAgents, data.alignmentRange, sheep => sheep.Velocity, desired => agent.CalculateSteering(desired.normalized * agent.MaxSpeed));

    Vector3 Cohesion() => GetAverageNeighbourVector(agent.AllAgents, data.cohesionRange, sheep => sheep.Velocity, desired => agent.Seek(desired));

    /// <summary>
    /// Returns the average vector (defined by <paramref name="sheepVector"/>) among neighbours
    /// within range, processed by <paramref name="onResult"/>.
    /// </summary>
    /// <param name="list">List of agents to evaluate as possible neighbours.</param>
    /// <param name="range">Distance to consider an agent a neighbour.</param>
    /// <param name="sheepVector">Function that extracts the relevant vector from each neighbour (e.g. its velocity, or the direction toward it).</param>
    /// <param name="onResult">Function applied to the final averaged vector (e.g. normalization, sign, steering calculation).</param>
    /// <returns>The resulting steering vector, or Vector3.zero if no neighbours are found in range.</returns>
    Vector3 GetAverageNeighbourVector(IEnumerable<SheepMovement> list, float range, Func<SheepMovement, Vector3> sheepVector, Func<Vector3, Vector3> onResult)
    {
        Vector3 desired = default;
        int count = 0;

        foreach (var item in list)
        {
            if (item == this.agent) { continue; }

            if (Vector3.Distance(item.transform.position, data.transform.position) <= range)
            {
                desired += sheepVector(item);
                count++;
            }
        }

        if (count == 0) { return Vector3.zero; }

        desired /= count;
        return onResult(desired);
    }
}

[System.Serializable]
public class FlockData
{
    [Range(0, 3)] public float separationWeight = 1f;
    [Range(0, 3)] public float alignmentWeight = 2f;
    [Range(0, 3)] public float cohesionWeight = 3f;

    public float separationRange = 1.0f;
    public float alignmentRange = 2.5f;
    public float cohesionRange = 5.0f;

    public Transform transform;
}