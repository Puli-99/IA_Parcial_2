using System;
using UnityEngine;

public class CollectCorpseState : State
{
    readonly Agent agent;
    readonly CollectCorpseData data;
    float collectionTimer;
    readonly StateMachine sm;

    public CollectCorpseState(Agent agent, CollectCorpseData data, StateMachine stateMachine)
    {
        this.agent = agent;
        this.data = data;
        this.sm = stateMachine;
    }

    public override void Enter()
    {
        collectionTimer = 0;
        //data.collected = false;

        agent.GetComponentInChildren<Renderer>().material.color = data.goingToCollectColor;
    }

    public override void Update()
    {
        agent.AddVelocity(agent.Arrive(data.target.transform.position));

        float distance = (agent.transform.position - data.target.transform.position).magnitude;
        if (distance < data.collectRange)
        {
            agent.StopVelocity();
            agent.GetComponentInChildren<Renderer>().material.color = data.collectingColor;

            collectionTimer += Time.deltaTime;

            if (collectionTimer > data.collectionTime)
            {
                //data.collected = true;
                UnityEngine.Object.Destroy(data.target.gameObject);
                data.target = null;
                sm.ChangeState(PatrolHunterState.Patrol);
                data.collectedCorpses ++;
            }
        }
    }

    public override void Exit()
    {

    }
}

[System.Serializable]
public class CollectCorpseData
{
    public Agent target;
    public float collectionTime;
    public float collectRange;
    //[NonSerialized] public bool collected;
    public Color collectingColor;
    public Color goingToCollectColor;
    public int collectedCorpses = 0;
}