using System;
using UnityEngine;

public class DistanceAttackState : State
{
    float shootCooldownTimer;
    float shootTimer;
    float outOfAimTimer;
    bool bShooting;

    readonly Agent agent;
    readonly DistanceData data;

    StateMachine sm;
    public DistanceAttackState(Agent hunter, DistanceData data, StateMachine stateMachine)
    {
        this.agent = hunter;
        this.data = data;
        this.sm = stateMachine;
    }

    public override void Enter()
    {
        Debug.Log("DistanceAttackState Enter");
        data.hasShot = false;
        data.targetLost = false;
        bShooting = false;
        shootTimer = 0;
        outOfAimTimer = 0;
        agent.GetComponentInChildren<Renderer>().material.color = data.initialColor;

    }

    public override void Update()
    {
        if (data.target == null) { Finish(); return; }

        float distance = (agent.transform.position - data.target.transform.position).magnitude;

        if (distance < data.aimRange)
        {
            shootCooldownTimer -= Time.deltaTime;
            if (shootCooldownTimer >= 0)
            {
                agent.GetComponentInChildren<Renderer>().material.color = Color.grey;
                return;
            }

            if (PrepareShot()) { Finish(); return; }

            bShooting = true;
            outOfAimTimer = 0;
            agent.GetComponentInChildren<Renderer>().material.color = data.aimingColor;
            agent.StopVelocity();
        }
        else if (distance > data.sightRange)
        {
            bShooting = false;
            shootTimer = 0;
            agent.AddVelocity(agent.Pursuit(data.target));
            agent.GetComponentInChildren<Renderer>().material.color = data.pursuitColor;

            outOfAimTimer += Time.deltaTime;
            if (outOfAimTimer > data.pursuitTime) { Finish(); return; }
        }
        else if (!bShooting)
        {
            shootTimer = 0;
            agent.AddVelocity(agent.Pursuit(data.target));
        }
        else
        {
            shootCooldownTimer -= Time.deltaTime;
            if (shootCooldownTimer >= 0)
            {
                agent.GetComponentInChildren<Renderer>().material.color = Color.grey;
                return;
            }
            if (PrepareShot()) { Finish(); return; }
        }
    }

    bool PrepareShot()
    {
        shootTimer += Time.deltaTime;
        if (shootTimer <= data.aimingTime) return false;

        agent.GetComponentInChildren<Renderer>().material.color = data.shotColor;
        shootCooldownTimer = data.shootCooldown;
        data.target.Killed();
        return true;
    }

    void Finish()
    {
        data.target = null;
        sm.ChangeState(PatrolHunterState.Patrol);
    }

    public override void Exit()
    {
        Debug.Log("DistanceState Exit");
    }
}

[System.Serializable]
public class DistanceData
{
    [NonSerialized] public bool targetLost;
    [NonSerialized] public bool hasShot;
    public float shootCooldown = 2;
    public float aimingTime = 2;
    public float aimRange = 10;
    public float sightRange = 15;
    public float pursuitTime = 3;
    public SheepMovement target;
    public Color shotColor = Color.silver;
    public Color pursuitColor = Color.cornflowerBlue;
    public Color aimingColor = Color.gold;
    public Color initialColor = Color.white;
}