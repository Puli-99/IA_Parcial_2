using UnityEngine;

[System.Serializable]
public class AgentData
{
    public float maxSteering = 2.0f;
    public float maxSpeed = 5.0f;

    public float detectionRange = 10.0f;
    public float stopRange = 2.0f;
    public float slowRange = 7.5f;
}

public class Agent : MonoBehaviour
{
    [SerializeField] AgentData agentData;

    protected Vector3 velocity;
    public Vector3 Velocity { get { return velocity; } }

    protected float MaxSteering => agentData.maxSteering;
    public float MaxSpeed => agentData.maxSpeed;

    public float DetectionRange => agentData.detectionRange;

    protected float StopRange => agentData.stopRange;
    protected float SlowRange => agentData.slowRange;
    public void AddVelocity(Vector3 quantity) // hacer return Vector3
    {
        velocity += quantity;
        velocity = Vector3.ClampMagnitude(velocity, MaxSpeed);
    }

    public void StopVelocity()
    {
        if (velocity != Vector3.zero)
            AddVelocity(-velocity);
    }

    public Vector3 CalculateSteering(Vector3 desired)
    {
        Vector3 steering = desired - velocity;
        steering = Vector3.ClampMagnitude(steering, MaxSteering * Time.deltaTime);

        return steering;
    }

    public Vector3 GetDesired(Vector3 targetPosition)
    {
        Vector3 desired = (targetPosition - transform.position).normalized;
        return desired *= MaxSpeed;
    }

    public Vector3 Seek(Vector3 targetPosition)
    {
        Vector3 desired = GetDesired(targetPosition);

        return CalculateSteering(desired);
    }

    public virtual Vector3 Flee(Vector3 targetPosition)
    {
        Vector3 desired = GetDesired(targetPosition);

        return CalculateSteering(-desired);
    }

    public Vector3 Arrive(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;

        float distance = direction.magnitude;

        if (distance < StopRange)
        {
            OnArrive();
            return Vector3.zero;
        }

        float targetSpeed = MaxSpeed * (distance / SlowRange);
        float desiredSpeed = Mathf.Min(targetSpeed, MaxSpeed);

        Vector3 desired = direction.normalized * desiredSpeed;

        return CalculateSteering(desired);
    }

    /// <summary>
    /// Called when distance is lesser than stopRange in Arrive()
    /// </summary>
    public virtual void OnArrive() {}


    protected Vector3 GetFutureAgentPosition(Agent target)
    {
        Vector3 direction = target.transform.position - transform.position;

        float distance = direction.magnitude;

        float prediction = distance / (MaxSpeed + target.Velocity.magnitude);

        return target.transform.position + target.Velocity * prediction;
    }

    public Vector3 Pursuit(Agent target)
    {
        return Seek(GetFutureAgentPosition(target));
    }

    public Vector3 Evade(Agent target)
    {
        return Flee(GetFutureAgentPosition(target));
    }
}