using UnityEngine;

[RequireComponent(typeof(FOVSensor))]
public class _Visual : MonoBehaviour
{
    [SerializeField] private Renderer bodyRenderer;
    [SerializeField] private Color alertColor = Color.red;

    private FOVSensor sensor;
    private Material matInstance;
    private Color defaultColor;

    private void Awake()
    {
        sensor = GetComponent<FOVSensor>();

        if (bodyRenderer == null) return;

        matInstance = bodyRenderer.material;
        defaultColor = matInstance.color;
    }

    private void Update()
    {
        if (matInstance == null) return;

        matInstance.color = sensor.CanSeeTarget ? alertColor : defaultColor;
    }
}