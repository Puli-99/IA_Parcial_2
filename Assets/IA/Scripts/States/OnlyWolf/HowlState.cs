using System.Collections.Generic;
using UnityEngine;

public class HowlState : State
{
    private class Ring
    {
        public LineRenderer line;
        public float delay;
        public float elapsed;
    }

    const int c_Segments = 48;
    static Material sharedMaterial;

    readonly List<Ring> rings = new ();
    Vector3 center;


    readonly PathfindingAgent agent;
    readonly HowlData data;
    readonly StateMachine sm;


    public HowlState(PathfindingAgent agent, HowlData data, StateMachine sm)
    {
        this.agent = agent;
        this.data = data;
        this.sm = sm;
    }

    public override void Enter()
    {
        ClearRings(); // por si se reentra antes de terminar

        if (sharedMaterial == null)
            sharedMaterial = new Material(Shader.Find("Sprites/Default"));

        CircleConfig();
    }

    public override void Update()
    {
        CircleCycle();
    }

    public override void Exit()
    {
        Debug.Log("Exit");
        ClearRings();
    }

    void CircleConfig()
    {
        Vector3 offset = Vector3.up * data.heightOffset;
        center = agent.transform.position + offset;

        for (int i = 0; i < data.ringCount; i++)
        {
            var ringGO = new GameObject("HowlRing");
            var lineRenderer = ringGO.AddComponent<LineRenderer>();
            lineRenderer.useWorldSpace = true;
            lineRenderer.loop = true;
            lineRenderer.positionCount = c_Segments;
            lineRenderer.widthMultiplier = data.lineWidth;
            lineRenderer.sharedMaterial = sharedMaterial;
            lineRenderer.enabled = false; // se activa al terminar su delay

            rings.Add(new Ring { line = lineRenderer, delay = i * data.ringDelay });
        }
    }

    void CircleCycle()
    {
        float deltaTime = Time.deltaTime;

        for (int i = rings.Count - 1; i >= 0; i--)
        {
            Ring ring = rings[i];

            if (ring.delay > 0f)
            {
                ring.delay -= deltaTime;
                continue;
            }

            ring.line.enabled = true;
            ring.elapsed += deltaTime;
            float n = ring.elapsed / data.duration;

            if (n >= 1f)
            {
                Object.Destroy(ring.line.gameObject);
                rings.RemoveAt(i);
                continue;
            }

            float eased = 1f - (1f - n) * (1f - n); // ease-out
            SetCircle(ring.line, data.maxRadius * eased);

            Color color = data.color;
            color.a = data.color.a * (1f - n); // fade out
            ring.line.startColor = ring.line.endColor = color;

        }

        if (rings.Count <= 0) sm.ChangeState(Wolf.WolfState.Chase);
    }

    void SetCircle(LineRenderer lineRenderer, float radius)
    {
        for (int i = 0; i < c_Segments; i++)
        {
            float angle = i / (float)c_Segments * Mathf.PI * 2f;
            Vector3 dir = new (Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            lineRenderer.SetPosition(i, center + dir * radius);
        }
    }

    void ClearRings()
    {
        foreach (var ring in rings)
        {
            if (ring.line != null) Object.Destroy(ring.line.gameObject);
        }
        rings.Clear();
    }
}

[System.Serializable]
public class HowlData
{
    public Color color = Color.white;
    public float maxRadius = 3f;
    public float duration = 0.8f;
    public float lineWidth = 0.05f;
    public int ringCount = 2;
    public float ringDelay = 0.15f;
    public float heightOffset = 0.05f; // evita z-fighting con el piso
}