using UnityEngine;
using System.Collections;

public class FOVTrigger : FOVSensor
{
    [SerializeField] float followTime = 2;
    [SerializeField] bool targetInRange;
    Coroutine keepLookingCoroutine;

    void Awake()
    {
        SphereCollider sensor = gameObject.AddComponent<SphereCollider>();
        sensor.isTrigger = true;
        sensor.radius = viewRadius;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsAValidTarget(other.gameObject.layer, targetMask)) return;

        if (keepLookingCoroutine != null)
            StopCoroutine(keepLookingCoroutine);

        target = other.transform.root;
        targetInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (!IsAValidTarget(other.gameObject.layer, targetMask)) return;

        keepLookingCoroutine = StartCoroutine(KeepLooking());
    }

    void Update()
    {
        CanSeeTarget = targetInRange && target != null && InViewAngle() && HasLineOfSight(); // ESTO ES LO QUE IMPORTA
    }

    IEnumerator KeepLooking()
    {
        yield return new WaitForSeconds(followTime);
        targetInRange = false;
    }
}