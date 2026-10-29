using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FOVSensor))]
public class Wolf : PathfindingAgent
{
    public enum WolfState { None, Patrol, Howl, Chase, ToHunt/*cambiar nombre por un mejor nombre*/ }
    readonly static List<Wolf> pack = new();


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

    Hunter hunterRef;


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
        if (IsHunterOnSafeZone() || IsHunterDead())
        {
            chaseData.hunter = null;
            toHuntData.hunter = null;
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

    bool TryGetHunterRef()
    {
        if (hunterRef != null) return true;

        Transform transform = GetAnyHunterTransform();
        return transform != null && transform.TryGetComponent(out hunterRef);
    }

    bool IsHunterDead() => TryGetHunterRef() && hunterRef.Dead;

    bool IsHunterOnSafeZone()
    {
        Transform hunter = GetAnyHunterTransform();
        return hunter != null && Graph.current.GetNodeFromPosition(hunter.position).Zone == Node.ZoneType.Safe;
    }

    Transform GetAnyHunterTransform()
    {
        if (sense.Target != null) return sense.Target;
        if (chaseData.hunter != null) return chaseData.hunter;
        if (toHuntData.hunter != null) return toHuntData.hunter;

        return null;
    }

    void OnDisable()
    {
        pack.Remove(this);
    }
}