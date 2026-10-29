using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum SheepState { None, Idle, Flock, Eat, Escape, Dead }

public class SheepMovement : Agent
{

    [SerializeField] Agent hunter;
    [SerializeField] Transform bait;
    [SerializeField] Renderer render;
    [SerializeField] Collider col;

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

        Escape();

        transform.position += velocity * Time.deltaTime;

        if (velocity != Vector3.zero)
            transform.forward = velocity;

        KeepHeight(1);
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

    public void Respawn()
    {
        if (!killed) return;
        col.enabled = false;
        StartCoroutine(Revive());
    }

    public void ClearHunter() => hunter = null;

    public override void OnArrive()
    {
        velocity = Vector3.zero;
    }

    void KeepHeight(float height) // pequeño fix porque sino estan en y = 0 se van volando :C
    {
        if (transform.position.y != height)
        {
            Vector3 x = transform.position;
            x.y = height;
            transform.position = x;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (stateMachine.CurrentState != deadState)
        {
            if (other.gameObject.CompareTag("Hunter") && stateMachine.CurrentState != eatState)
            {
                hunter = other.GetComponent<Agent>();
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


    IEnumerator Revive()
    {
        bool visible = true;
        for (float t = 0f; t < 1.5f; t += 0.1f)
        {
            visible = !visible;
            SetVisible(visible);
            yield return new WaitForSeconds(0.1f);
        }

        SetVisible(true);

        List<Node> n = Graph.current.GetAllGridNodes.Where(node => node.Zone == Node.ZoneType.Entry).ToList();

        Node entryNode = n[Random.Range(0, n.Count())];
        transform.position = entryNode.transform.position;
        killed = false;
        col.enabled = true;
        stateMachine.ChangeState(SheepState.Flock);
    }

    void SetVisible(bool value)
    {
        render.enabled = value;
    }
}