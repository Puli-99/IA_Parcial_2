using System.Collections.Generic;
using UnityEngine;

public enum SheepState { None, Idle, Flock, Eat, Escape, Dead }

public class SheepMovement : Agent
{

    [SerializeField] PatrolAgent hunter;
    [SerializeField] Transform bait;

    readonly static List<SheepMovement> allAgents = new();
    public List<SheepMovement> AllAgents { get { return allAgents; } }

    public bool killed = false;

    StateMachine stateMachine;

    IdleState idleState;
    FlockState flockState;
    EatState eatState;
    EscapeState escapeState;
    DeadState deadState;

    [SerializeField] FlockData flockData;
    [SerializeField] EatData eatData;
    [SerializeField] EscapeData escapeData;

    void Awake()
    {
        allAgents.Add(this);

        SFMInitializer();

        flockData.transform = this.transform;
        stateMachine.ChangeState(SheepState.Flock);
    }


    void SFMInitializer()
    {
        stateMachine = new StateMachine();

        idleState = new IdleState(this);
        flockState = new FlockState(this, flockData);
        eatState = new EatState(this, eatData, stateMachine);
        escapeState = new EscapeState(this, escapeData, stateMachine);
        deadState = new DeadState(this);


        stateMachine.RegisterState(SheepState.Idle, idleState);
        stateMachine.RegisterState(SheepState.Flock, flockState);
        stateMachine.RegisterState(SheepState.Eat, eatState);
        stateMachine.RegisterState(SheepState.Escape, escapeState);
        stateMachine.RegisterState(SheepState.Dead, deadState);
    }


    void Update()
    {
        transform.position = Bounds.Instance.OutOfBounds(transform.position);

        stateMachine.Update();

        //if (stateMachine.CurrentState == escapeState && escapeState.IsSafe)
        //    ToFlockFromEscape();

        //if (stateMachine.CurrentState == eatState && eatState.Finished)
        //    ToFlockFromEat();

        Escape();

        transform.position += velocity * Time.deltaTime;

        if (velocity != Vector3.zero)
            transform.forward = velocity;
    }

    void Escape()
    {
        if (hunter == null || stateMachine.CurrentState == deadState) return;
        
        float distanceFromHunter = (hunter.transform.position - transform.position).magnitude;

        if (distanceFromHunter < DetectionRange)
        {
            allAgents.Remove(this);
            stateMachine.ChangeState(SheepState.Escape);
        }
    }

    public void Killed()
    {
        killed = true;
        allAgents.Remove(this);
        stateMachine.ChangeState(SheepState.Dead);
    }

    public void ClearHunter() => hunter = null;

    public override void OnArrive()
    {
        velocity = Vector3.zero;
    }

    private void OnTriggerStay(Collider other)
    {
        if (stateMachine.CurrentState != deadState)
        {
            if (other.gameObject.CompareTag("Hunter") && stateMachine.CurrentState != eatState)
            {
                hunter = other.GetComponent<PatrolAgent>();
                escapeData.hunter = hunter;
            }

            if (other.gameObject.CompareTag("Bait") && stateMachine.CurrentState != escapeState)
            {
                bait = other.gameObject.transform;
                eatData.baitTransform = bait;
                allAgents.Remove(this);
                stateMachine.ChangeState(SheepState.Eat);
            }
        }
    }
}