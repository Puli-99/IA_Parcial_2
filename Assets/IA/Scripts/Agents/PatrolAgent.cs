using UnityEngine;

public enum PatrolHunterState { None, Patrol, Baiting, Attack, Collect }

public class PatrolAgent : Agent
{
    StateMachine stateMachine;

    PatrolState patrolState; // cambiar nombre de clase para que no sea confuso con el enum
    [SerializeField] PatrolData patrolData;

    SetBaitState setBaitState;
    [SerializeField] BaitData baitData;

    DistanceAttackState distanceAttackState;
    [SerializeField] DistanceData distanceData;

    CollectCorpseState collectCorpseState;
    [SerializeField] CollectCorpseData collectCorpseData;

    float baitTimer;
    [SerializeField] float baitCooldown = 5;

    [SerializeField] string currentState;


    LineRenderer aimRangeLine;
    LineRenderer sightRangeLine;
    LineRenderer targetLine;
    TextMesh debugText;

    #region Visual Debug

    void SetupDebugVisuals()
    {
        aimRangeLine = CreateCircleRenderer("AimRangeCircle", Color.red);
        sightRangeLine = CreateCircleRenderer("SightRangeCircle", Color.yellow);

        var targetLineObj = new GameObject("TargetLine");
        targetLineObj.transform.SetParent(transform);
        targetLine = targetLineObj.AddComponent<LineRenderer>();
        ConfigureLine(targetLine, Color.white, 0.05f);
        targetLine.positionCount = 2;

        var textObj = new GameObject("DebugText");
        textObj.transform.SetParent(transform);
        textObj.transform.localPosition = Vector3.up * 2.5f;
        debugText = textObj.AddComponent<TextMesh>();
        debugText.characterSize = 0.3f;
        debugText.anchor = TextAnchor.LowerCenter;
        debugText.alignment = TextAlignment.Center;
        debugText.color = Color.white;
    }

    LineRenderer CreateCircleRenderer(string name, Color color)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(transform);
        var lr = obj.AddComponent<LineRenderer>();
        ConfigureLine(lr, color, 0.03f);
        lr.loop = true;
        return lr;
    }

    void ConfigureLine(LineRenderer lr, Color color, float width)
    {
        lr.useWorldSpace = true;
        lr.startWidth = width;
        lr.endWidth = width;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = color;
        lr.endColor = color;
    }


    void UpdateDebugVisuals()
    {
        DrawCircle(aimRangeLine, distanceData.aimRange);
        DrawCircle(sightRangeLine, distanceData.sightRange);

        if (distanceData.target != null)
        {
            targetLine.gameObject.SetActive(true);
            targetLine.SetPosition(0, transform.position);
            targetLine.SetPosition(1, distanceData.target.transform.position);

            float distance = (transform.position - distanceData.target.transform.position).magnitude;
            Color lineColor = distance < distanceData.aimRange ? Color.green : Color.cyan;
            targetLine.startColor = targetLine.endColor = lineColor;

            debugText.text = $"dist: {distance:F1}\naim: {distanceData.aimRange:F1}\nsight: {distanceData.sightRange:F1}";
        }
        else
        {
            targetLine.gameObject.SetActive(false);
            debugText.text = $"aim: {distanceData.aimRange:F1}\nsight: {distanceData.sightRange:F1}";
        }

        if (Camera.main != null)
            debugText.transform.rotation = Camera.main.transform.rotation; // billboard hacia cámara
    }

    void DrawCircle(LineRenderer lr, float radius, int segments = 32)
    {
        lr.positionCount = segments;
        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            Vector3 point = transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            lr.SetPosition(i, point);
        }
    }

    #endregion

    void Awake()
    {
        SFMInitializer();   

        stateMachine.ChangeState(PatrolHunterState.Patrol);

        SetupDebugVisuals();
    }

    void SFMInitializer()
    {
        stateMachine = new();
        //IdleState idleState = new();
        patrolState = new(this, patrolData);
        setBaitState = new SetBaitState(this, baitData, stateMachine);
        distanceAttackState = new DistanceAttackState(this, distanceData, stateMachine);
        collectCorpseState = new CollectCorpseState(this, collectCorpseData, stateMachine);

        stateMachine.RegisterState(PatrolHunterState.Patrol, patrolState);
        stateMachine.RegisterState(PatrolHunterState.Baiting, setBaitState);
        stateMachine.RegisterState(PatrolHunterState.Attack, distanceAttackState);
        stateMachine.RegisterState(PatrolHunterState.Collect, collectCorpseState);
    }

    void Update()
    {
        currentState = stateMachine.CurrentState.ToString();
        stateMachine.Update();

        DistanceAttack();
        Bait();
        CollectCorpse();

        UpdateDebugVisuals();

        transform.position += velocity * Time.deltaTime;

        if (velocity != Vector3.zero)
            transform.forward = velocity;
    }

    void Bait()
    {
        if (stateMachine.CurrentState == distanceAttackState) return;
        if (stateMachine.CurrentState == setBaitState) return;

        if (distanceData.target != null)
        {
            baitTimer = 0;
            return;
        }

        baitTimer += Time.deltaTime;
        if (baitTimer > baitCooldown)
        {
            baitTimer = 0;
            stateMachine.ChangeState(PatrolHunterState.Baiting);
        }
    }

    void DistanceAttack()
    {
        if (stateMachine.CurrentState == distanceAttackState) return;

        if (distanceData.target != null && distanceData.target.killed == false)
        {
            stateMachine.ChangeState(PatrolHunterState.Attack);
        }
    }

    void CollectCorpse()
    {
        if (stateMachine.CurrentState == distanceAttackState) return;
        if (stateMachine.CurrentState == collectCorpseState) return;


        if (collectCorpseData.target != null)
        {
            stateMachine.ChangeState(PatrolHunterState.Collect);
        }
    }


    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Sheep"))
        {
            float distanceFromSheep = (other.transform.position - transform.position).magnitude;

            if (distanceFromSheep < distanceData.sightRange && distanceData.target == null)
            {
                if (!distanceData.target.killed) distanceData.target = other.GetComponent<SheepMovement>();
                collectCorpseData.target = other.GetComponent<SheepMovement>();
                Debug.Log("target located");
            }
        }
    }
}