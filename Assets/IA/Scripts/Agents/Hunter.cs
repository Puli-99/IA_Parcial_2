using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum HunterState { None, Search, SetBait, Attack, CollectCorpse, Escape, Exit, Dead }

public class Hunter : PathfindingAgent
{
    #region State Vars
    StateMachine stateMachine;
    SearchState searchState;
    [SerializeField] SearchData searchData;

    SetBaitState setBaitState;
    [SerializeField] BaitData baitData;

    DistanceAttackState distanceAttackState;
    [SerializeField] DistanceData distanceData;

    CollectCorpseState collectCorpseState;
    [SerializeField] CollectCorpseData collectCorpseData;

    SafeZoneState safeZoneState;
    [SerializeField] SafeZoneData safeZoneData;

    ExitState exitState;
    [SerializeField] ExitData exitData;
    DeadState deadState;
    #endregion


    [SerializeField] string state;
    [SerializeField] Renderer render;
    bool dead = false;
    public bool Dead => dead;
    float respawnTime;


    void Awake()
    {
        InitializeStates();
    }
    void InitializeStates()
    {
        stateMachine = new ();

        searchState = new(this, searchData, stateMachine);
        setBaitState = new(this, baitData, stateMachine);
        distanceAttackState = new(this, distanceData, stateMachine);
        collectCorpseState = new(this, collectCorpseData, stateMachine);
        safeZoneState = new(this, safeZoneData, stateMachine);
        exitState = new(this, exitData, stateMachine);
        deadState = new(this);

        stateMachine.RegisterState(HunterState.Search, searchState);
        stateMachine.RegisterState(HunterState.SetBait, setBaitState);
        stateMachine.RegisterState(HunterState.Attack, distanceAttackState);
        stateMachine.RegisterState(HunterState.CollectCorpse, collectCorpseState);
        stateMachine.RegisterState(HunterState.Escape, safeZoneState);
        stateMachine.RegisterState(HunterState.Exit, exitState);
        stateMachine.RegisterState(HunterState.Dead, deadState);

        stateMachine.RegisterState(HunterState.None, new NoneState());
    }

    void Start()
    {
        StartCoroutine(StartOnGraphSet()); // Pone State en Search cuando existen nodos con ZoneType.Hunt
    }

    void Update()
    {
        stateMachine.Update();
        if(stateMachine.CurrentState != null) state = stateMachine.CurrentState.ToString();

        if (dead) return;

        if (stateMachine.CurrentState != safeZoneState && stateMachine.CurrentState != exitState && collectCorpseData.collectedCorpses >= 3) // mejorar esta condicion?
        {
            stateMachine.ChangeState(HunterState.Exit);
        }

        transform.position += velocity * Time.deltaTime;

        if (velocity != Vector3.zero)
            transform.forward = velocity;
    }

    public void Killed()
    {
        StopAllCoroutines();
        dead = true;
        stateMachine.ChangeState(HunterState.Dead);
        StartCoroutine(Revive());
    }

    void OnTriggerEnter(Collider other)
    {
        if (dead) return;

        if (other.CompareTag("Enemy"))
        {
            if (stateMachine.CurrentState == safeZoneState) return;

            if (walkCoroutine != null) StopCoroutine(walkCoroutine);
            stateMachine.ChangeState(HunterState.Escape);
        }        
    }

    void OnTriggerStay(Collider other)
    {
        if (dead) return;
        if (stateMachine.CurrentState == distanceAttackState) return;

        if (other.CompareTag("Sheep"))
        {
            float distanceFromSheep = (other.transform.position - transform.position).magnitude;

            if (distanceFromSheep < distanceData.sightRange)
            {
                searchData.boid = other.GetComponent<SheepMovement>();
                if (stateMachine.CurrentState == collectCorpseState) return;
                collectCorpseData.target = other.GetComponent<Agent>();

                if (distanceData.target == null && !searchData.boid.killed)
                {
                    distanceData.target = other.GetComponent<SheepMovement>();
                }
            }
        }
    }

    IEnumerator StartOnGraphSet()
    {
        yield return new WaitUntil(() => Graph.current.GetAllGridNodes.Any(node => node.Zone == Node.ZoneType.Hunt));
        stateMachine.ChangeState(HunterState.Search);
    }

    IEnumerator Revive()
    {
        yield return new WaitForSeconds(2);

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
        Reposition(entryNode);
        render.material.color = Color.grey;

        yield return new WaitForSeconds(1);
        collectCorpseData.collectedCorpses = 0;
        dead = false;
        stateMachine.ChangeState(HunterState.Search);
    }

    void SetVisible(bool value)
    {
        render.enabled = value;
    }
}