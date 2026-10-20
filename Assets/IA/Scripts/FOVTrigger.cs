using UnityEngine;
using System.Collections;

public class FOVTrigger : FOVSensor
{
    [SerializeField] float followTime;
    bool targetInRange;
    Coroutine keepLookingCoroutine;

    private void Awake()
    {
        SphereCollider sensor = gameObject.AddComponent<SphereCollider>();
        sensor.isTrigger = true;
        sensor.radius = viewRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsAValidTarget(other.gameObject.layer, targetMask)) return;

        if (keepLookingCoroutine != null)
            StopCoroutine(keepLookingCoroutine);

        target = other.transform.root;
        targetInRange = true;
        Debug.Log("Player found: " + other.gameObject.layer);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsAValidTarget(other.gameObject.layer, targetMask)) return;

        keepLookingCoroutine = StartCoroutine(KeepLooking());
        Debug.Log("Player Exited");
    }

    private void Update()
    {
        CanSeeTarget = targetInRange && target != null && InViewAngle() && HasLineOfSight(); // ESTO ES LO QUE IMPORTA
    }

    IEnumerator KeepLooking()
    {
        yield return new WaitForSeconds(followTime);
        targetInRange = false;
        Debug.Log("Player lost");
    }
}