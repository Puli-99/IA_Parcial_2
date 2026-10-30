using UnityEngine;

public class CollectCorpseState : State
{
    readonly Agent agent;
    readonly CollectCorpseData data;
    float collectionTimer;
    readonly StateMachine sm;
    Vector3 targetPosition;

    public CollectCorpseState(Agent agent, CollectCorpseData data, StateMachine stateMachine)
    {
        this.agent = agent;
        this.data = data;
        this.sm = stateMachine;
    }

    public override void Enter()
    {
        collectionTimer = 0;

        if (data.target == null)
        {
            sm.ChangeState(HunterState.Search);
            return;
        }

        agent.GetComponentInChildren<Renderer>().material.color = data.goingToCollectColor;
        targetPosition = data.target.transform.position;
        targetPosition.y = 1;
    }

    public override void Update()
    {

        agent.AddVelocity(agent.Seek(targetPosition));

        float distance = (agent.transform.position - targetPosition).magnitude;
        if (distance < data.collectRange)
        {
            agent.StopVelocity();
            agent.GetComponentInChildren<Renderer>().material.color = data.collectingColor;

            collectionTimer += Time.deltaTime;

            if (collectionTimer > data.collectionTime)
            {
                BodyCollected();
            }
        }
    }

    void BodyCollected()
    {
        if (data.target.TryGetComponent<SheepMovement>(out SheepMovement s))
        {
            s.Respawn();
        }

        data.target = null;
        data.collectedCorpses++;

        if (agent is PathfindingAgent) sm.ChangeState(HunterState.Search);
        else if (agent is PatrolAgent) sm.ChangeState(PatrolHunterState.Patrol);
    }

    public override void Exit()
    {
        agent.StopAllCoroutines();
    }
}

[System.Serializable]
public class CollectCorpseData
{
    public Agent target;
    public float collectionTime;
    public float collectRange;
    public Color collectingColor;
    public Color goingToCollectColor;
    public int collectedCorpses = 0;
}