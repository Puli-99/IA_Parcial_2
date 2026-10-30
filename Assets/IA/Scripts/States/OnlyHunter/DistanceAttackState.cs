using System;
using UnityEngine;

public class DistanceAttackState : State
{
    float shootCooldownTimer;
    float shootTimer;
    float outOfAimTimer;
    bool bShooting;
    Renderer agentRenderer;


    readonly Agent agent;
    readonly DistanceData data;
    readonly StateMachine sm;
    public DistanceAttackState(Agent hunter, DistanceData data, StateMachine stateMachine)
    {
        this.agent = hunter;
        this.data = data;
        this.sm = stateMachine;
    }

    public override void Enter()
    {
        agent.StopAllCoroutines();
        data.hasShot = false;
        data.targetLost = false;
        bShooting = false;
        shootTimer = 0;
        outOfAimTimer = 0;
        agentRenderer = agent.GetComponentInChildren<Renderer>();
        ChangeColor(data.initialColor);
    }

    public override void Update()
    {
        if (data.target == null) { Finish(); return; }

        PatrolAgentAttack();
    }

    void PatrolAgentAttack()
    {
        float distanceFromTarget = (agent.transform.position - data.target.transform.position).magnitude;

        if (distanceFromTarget < data.aimRange) 
        {
            shootCooldownTimer -= Time.deltaTime;
            if (shootCooldownTimer >= 0)
            {
                ChangeColor(data.onCooldownColor);

                return;
            }

            if (PrepareShot()) { Finish(); return; } // una vez terminada la recarga, preparamos el tiro, si terminamos de disparar (PrepareShot() = true) entonces terminamos

            bShooting = true;
            outOfAimTimer = 0;

            agent.StopVelocity();
        }
        else if (distanceFromTarget > data.sightRange)
        {
            ChangeColor(data.pursuitColor);

            shootCooldownTimer -= Time.deltaTime;
            shootTimer = 0;

            agent.AddVelocity(agent.Pursuit(data.target));

            outOfAimTimer += Time.deltaTime;
            if (outOfAimTimer > data.pursuitTime) { Finish(); return; }
        }
        else if (!bShooting) // Si la distancia es menor a sightRange, pero mayor a aimRange, y no estabamos preparando el tiro, seguimos al objetivo
        {
            ChangeColor(data.pursuitColor);
            shootCooldownTimer -= Time.deltaTime;
            shootTimer = 0;
            agent.AddVelocity(agent.Pursuit(data.target));
        }
        else // Sigue entre la distancia sightRange y aimRange, pero estabamos preparando el tiro
        {
            shootCooldownTimer -= Time.deltaTime;
            if (shootCooldownTimer >= 0)
            {
                ChangeColor(data.onCooldownColor);
                agent.StopVelocity();
                return;
            }
            if (PrepareShot()) { Finish(); return; }
        }
    }

    bool PrepareShot()
    {
        ChangeColor(data.aimingColor);
        shootTimer += Time.deltaTime;
        if (shootTimer <= data.aimingTime) return false;

        ChangeColor(data.shotColor);
        shootCooldownTimer = data.shootCooldown;
        data.target.Killed();
        return true;
    }

    void Finish()
    {
        data.target = null;

        if (agent is PathfindingAgent) sm.ChangeState(HunterState.Search);
        else if (agent is PatrolAgent) sm.ChangeState(PatrolHunterState.Patrol);
    }

    void ChangeColor(Color newColor)
    {
        agentRenderer.material.color = newColor;
    }

    public override void Exit()
    {
        agent.StopAllCoroutines();
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
    public Color onCooldownColor;
}