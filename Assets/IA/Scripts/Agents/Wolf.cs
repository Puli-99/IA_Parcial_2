using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FOVSensor))]
public class Wolf : PathfindingAgent
{
    public enum WolfState { None, Patrol, Howl, Chase, ToHunt/*cambiar nombre por algo mejor*/ }
    static List<Wolf> pack = new();


    // States Machines
    StateMachine stateMachine;
    public StateMachine StateMachine => stateMachine;

    PatrolAState patrolState;
    [SerializeField] PatrolAData patrolData;

    HowlState howlState;
    [SerializeField] HowlData howlData;

    ChaseState chaseState;
    [SerializeField] ChaseData chaseData;

    ToHunt toHuntState;
    [SerializeField] ToHuntData toHuntData;

    [SerializeField] FOVSensor sense;
    [SerializeField] string state;



    void Awake()
    {
        if (sense == null) sense = GetComponent<FOVSensor>();
        InitializingStates();
    }

    void InitializingStates()
    {
        stateMachine = new();

        patrolState = new(this, patrolData);
        howlState = new(this, howlData, stateMachine);
        chaseState = new(this, chaseData, stateMachine);
        toHuntState = new(this, toHuntData, stateMachine);

        stateMachine.RegisterState(WolfState.Patrol, patrolState);
        stateMachine.RegisterState(WolfState.Howl, howlState);
        stateMachine.RegisterState(WolfState.Chase, chaseState);
        stateMachine.RegisterState(WolfState.ToHunt, toHuntState);
    }

    void Start()
    {
        pack.Add(this);
        stateMachine.ChangeState(WolfState.Patrol);
    }


    void Update()
    {
        stateMachine.Update();
        state = stateMachine.CurrentState.ToString();
        ChangeStateManagement();

        transform.position += velocity * Time.deltaTime;

        if (velocity != Vector3.zero)
            transform.forward = velocity;
    }

    void ChangeStateManagement()
    {
        if (IsHunterOnSafeZone())
        {
            stateMachine.ChangeState(WolfState.Patrol);
            return;
        }

        if (sense.CanSeeTarget && stateMachine.CurrentState == patrolState)
        {
            chaseData.hunter = sense.Target;
            stateMachine.ChangeState(WolfState.Howl);
        }

        if (sense.CanSeeTarget && stateMachine.CurrentState == toHuntState)
        {
            chaseData.hunter = sense.Target;
            stateMachine.ChangeState(WolfState.Chase);
        }

        if (stateMachine.CurrentState == howlState)
        {
            foreach (var wolf in pack)
            {
                if (wolf.stateMachine.CurrentState is HowlState) continue;
                wolf.toHuntData.hunter = sense.Target;
                wolf.stateMachine.ChangeState(WolfState.ToHunt);
            }
        }
    }

    bool IsHunterOnSafeZone()
    {
        if (sense.Target != null)
        {
            return Graph.current.GetNodeFromPosition(sense.Target.position).Zone == Node.ZoneType.Safe;
        }

        if (chaseData.hunter != null)
        {
            return Graph.current.GetNodeFromPosition(chaseData.hunter.position).Zone == Node.ZoneType.Safe;
        }

        if (toHuntData.hunter != null)
        {
            return Graph.current.GetNodeFromPosition(toHuntData.hunter.position).Zone == Node.ZoneType.Safe;
        }

        return false;
    }

    void OnDisable()
    {
        pack.Remove(this);
    }
}