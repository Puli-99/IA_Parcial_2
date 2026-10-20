using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FOVSensor))]
public class PathfindingAgent : Agent
{
    StateMachine stateMachine;

    ToHuntZoneState tohuntzoneState;
    [SerializeField] ToHuntZoneData tohuntzoneData;

    List<Node> path;
    Coroutine walkRoutine;
    [SerializeField] FOVSensor sense;

    [SerializeField] Node starting, end;

    public bool chasing;

    void Awake()
    {
        stateMachine = new();

        tohuntzoneState = new(this, tohuntzoneData);

        //if (sense == null) sense = GetComponent<FOVSensor>();
    }

    void Start()
    {
        Wander();
    }

    void Update()
    {

        if (chasing)
        {
            stateMachine.ChangeState(tohuntzoneState);
        }

        transform.position += velocity * Time.deltaTime;

        if (velocity != Vector3.zero)
            transform.forward = velocity;
    }
    void Wander()
    {
        starting = Graph.current.GetNodeFromPosition(transform.position);
        end = Graph.current.GetAllGridNodes[Random.Range(0, Graph.current.GetAllGridNodes.Count)]; // esto deberia ser el patrullaje
        CalculatePath(starting, end);
    }

    public void StartPathfinding(Node endNode) // Este approach sirve unicamente con un unico graph y si el agente se encuentra en algun nodo.
    {
        starting = Graph.current.GetNodeFromPosition(transform.position); 
        CalculatePath(starting, endNode);
    }

    public void Reposition(Node node)
    {
        if (walkRoutine != null) StopCoroutine(walkRoutine);
        transform.position = node.transform.position + Vector3.up;
    }

    public void CalculatePath(Node startNode, Node endNode)
    {
        path = Pathfinding.AStar(startNode, endNode);
        if (path != null && path.Count > 0)
        {
            if (walkRoutine != null) StopCoroutine(walkRoutine);
            walkRoutine = StartCoroutine(WalkPath(path));
        }
    }

    IEnumerator WalkPath(List<Node> path)
    {
        foreach (Node node in path)
        {
            Vector3 target = node.transform.position + Vector3.up;
            while ((transform.position - target).sqrMagnitude > 0.001f)
            {

                //transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
                AddVelocity(Seek(target)); // El arrive si se pasa, vuelve hacia atras hasta lograr bien la posicion del nodo antes de pasar al siguiente

                yield return null;
            }
        }
        StopVelocity();
        //Debug.Log("Reached");
        //yield return new WaitForSeconds(1); // Esto debería quedar por fuera, en un state
        //Wander();

    }
}