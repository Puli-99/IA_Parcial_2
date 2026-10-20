using UnityEngine;

[RequireComponent(typeof(FOVSensor))]
public class _Gizmo : MonoBehaviour
{
    private FOVSensor sensor;

    private Vector3 DirFromAngle(float angleDegrees)
    {
        float angleRad = (transform.eulerAngles.y + angleDegrees) * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(angleRad), 0f, Mathf.Cos(angleRad));
    }

    private void OnDrawGizmosSelected()
    {
        if (sensor == null) sensor = GetComponent<FOVSensor>();
        if (sensor == null) return;

        Gizmos.color = sensor.CanSeeTarget ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sensor.ViewRadius);

        Vector3 left = DirFromAngle(-sensor.ViewAngle * 0.5f);
        Vector3 right = DirFromAngle(sensor.ViewAngle * 0.5f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + left * sensor.ViewRadius);
        Gizmos.DrawLine(transform.position, transform.position + right * sensor.ViewRadius);

        if (sensor.Target != null)
        {
            Gizmos.color = sensor.CanSeeTarget ? Color.green : Color.gray;
            Gizmos.DrawLine(transform.position + Vector3.up * sensor.EyeHeight, sensor.Target.position + Vector3.up * sensor.EyeHeight);
        }
    }
}
