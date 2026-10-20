using UnityEngine;

public class FOVSensor : MonoBehaviour
{
    [Header("Field Of View")]
    [SerializeField] protected float viewRadius = 8f;
    [SerializeField] protected float eyeHeight = 1f;
    [SerializeField, Range(0f, 360f)] protected float viewAngle = 90f;

    [Header("Filtros por Layer")]
    [SerializeField] protected LayerMask targetMask = ~0;
    [SerializeField] protected LayerMask obstacleMask = ~0;

    protected Transform target;

    public bool CanSeeTarget { get; protected set; }

    public Transform Target => target;
    public float ViewRadius => viewRadius;
    public float ViewAngle => viewAngle;
    public float EyeHeight => eyeHeight;

    protected static bool IsAValidTarget(int layer, LayerMask mask) => (mask.value & (1 << layer)) != 0;

    protected bool InViewAngle()
    {
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        return Vector3.Angle(transform.forward, dirToTarget) <= viewAngle * 0.5f;
    }

    protected bool HasLineOfSight()
    {
        Vector3 eyePos = transform.position + Vector3.up * eyeHeight;
        Vector3 targetPos = target.position + Vector3.up * eyeHeight;
        Vector3 direction = targetPos - eyePos;

        if (Physics.Raycast(eyePos, direction.normalized, out RaycastHit hit, direction.magnitude, obstacleMask))
            return hit.transform == target || hit.transform.IsChildOf(target);

        return true;
    }
}