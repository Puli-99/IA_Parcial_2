using UnityEngine;

public enum HunterState { None, ToHuntZone, Patrol, SetBait, Attack, CollectCorpse, Escape, Dead }

public class Hunter : PathfindingAgent
{
    StateMachine stateMachine;

    ToHuntZoneState toHuntZoneState;
    [SerializeField] ToHuntZoneData toHuntZoneData;

    SetBaitState SetBaitState;
    BaitData baitData;

    DistanceAttackState distanceAttackState;
    [SerializeField] DistanceData distanceData;

    CollectCorpseState collectCorpseState;
    [SerializeField] CollectCorpseData collectCorpseData;

    DeadState deadState;



    public bool a;
    public bool b;

    void Awake()
    {
        InitializeStates();
    }
    void InitializeStates()
    {
        stateMachine = new ();

        toHuntZoneState = new(this, toHuntZoneData, stateMachine);
        distanceAttackState = new(this, distanceData, stateMachine);
        collectCorpseState = new(this, collectCorpseData, stateMachine);
        //EscapeState
        deadState = new(this);

        stateMachine.RegisterState(HunterState.ToHuntZone, toHuntZoneState);
        stateMachine.RegisterState(HunterState.Attack, distanceAttackState);
        stateMachine.RegisterState(HunterState.CollectCorpse, collectCorpseState);
        //EscapeState
        stateMachine.RegisterState(HunterState.Dead, deadState);

        stateMachine.RegisterState(HunterState.None, new NoneState());
    }

    void Start()
    {

    }

    void Update()
    {
        stateMachine.Update();

        if (b)
        {
            b = false;
            if (walkCoroutine != null) StopCoroutine(walkCoroutine);
            stateMachine.ChangeState(HunterState.Dead);
        }

        if (a)
        {
            a = false;
            stateMachine.ChangeState(HunterState.ToHuntZone);
        }

        transform.position += velocity * Time.deltaTime;

        if (velocity != Vector3.zero)
            transform.forward = velocity;
    }
}