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
    public DistanceAttackState(Agent hunter, DistanceData data)
    {
        this.agent = hunter;
        this.data = data;
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
        if (data.target == null)
        {
            data.targetLost = true;
            return;
        }

        float distance = (agent.transform.position - data.target.transform.position).magnitude;

        if (distance < data.aimRange) // Se encuentra en el rango dentro del circulo rojo
        {
            shootCooldownTimer -= Time.deltaTime;

            if (shootCooldownTimer >= 0)
            {
                agent.GetComponentInChildren<Renderer>().material.color = Color.grey;
                return;
            }

            PrepareShot();
            bShooting = true;
            outOfAimTimer = 0;
            agent.GetComponentInChildren<Renderer>().material.color = data.aimingColor;

            agent.StopVelocity();

        }
        else
        {
            if (distance > data.sightRange) // Se encuentra fuera del rango del circulo amarillo
            {
                bShooting = false;
                shootTimer = 0;
                agent.AddVelocity(agent.Pursuit(data.target));
                agent.GetComponentInChildren<Renderer>().material.color = data.pursuitColor;

                outOfAimTimer += Time.deltaTime;

                if (outOfAimTimer > data.pursuitTime)
                {
                    data.targetLost = true;
                }
            }

            else if (!bShooting) // Se encuentra entre los circulos rojo y amarillo pero todavia no esta apuntando
            {
                shootTimer = 0;
                agent.AddVelocity(agent.Pursuit(data.target));
            }

            else // Se encuentra entre los circulos rojo y amarillo y ya estaba apuntando
            {
                shootCooldownTimer -= Time.deltaTime;

                if (shootCooldownTimer >= 0)
                {
                    agent.GetComponentInChildren<Renderer>().material.color = Color.grey;
                    return;
                }
                PrepareShot();
            }
        }
    }

    void PrepareShot()
    {
        if (!data.hasShot)
        {
            shootTimer += Time.deltaTime;
            if (shootTimer > data.aimingTime)
            {
                agent.GetComponentInChildren<Renderer>().material.color = data.shotColor;
                data.hasShot = true;
                shootCooldownTimer = data.shootCooldown; // Hola Mati :) puse esto aca porque facilita que el cooldown se reinicie solo cuando el hunter se encuentra en el estado
                data.target.Killed();                   // De esta forma es mas sencillo que llegue a un estado de no poder disparar por cooldown. Salu2
            }
        }
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