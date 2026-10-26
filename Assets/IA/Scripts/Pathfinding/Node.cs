using TMPro;
using UnityEngine;

public class Node : MonoBehaviour
{
    public enum ZoneType { None, Entry, Exit, Safe, Hunt }

    [SerializeField] Renderer _renderer;
    [SerializeField] GameObject wallGameObject;
    [SerializeField] TextMeshPro costText;

    public Coordinates Coordinates { get; private set; }
    [SerializeField] ZoneType zone = ZoneType.None;
    public ZoneType Zone => zone;
    bool blocked = false; // cambiar por ZoneType.Blocked ?
    public bool Blocked 
    { 
        get => blocked; 

        private set
        {
            blocked = value;
            wallGameObject.SetActive(value);
            ResetColor();
        } 
    }

    int cost = 1;
    public int Cost
    {
        get => cost;
        private set
        {
            cost = Mathf.Clamp(value, 1, 5);
            costText.text = cost.ToString();
        }
    }


    public void Setup(Coordinates coordinates, int newCost)
    {
        Coordinates = coordinates;
        name = $"Node {coordinates}";
        Cost = newCost;
    }

    public void ChangeColor(Color color) => _renderer.material.color = color;

    public void ResetColor() => ChangeColor(Blocked ? Color.grey : Color.white);

    void OnValidate()
    {
        if (_renderer != null) return;
        if (!TryGetComponent(out _renderer)) Debug.LogError("This gameobject doesn't have Renderer component");
    }

    void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.layer)
        {
            case 8:
                Blocked = true;
                break;

            case 9:
                zone = ZoneType.Entry;
                ChangeColor(Color.lightGreen);
                break;

            case 10:
                zone = ZoneType.Exit;
                ChangeColor(Color.lightBlue);
                break;
            case 11:
                zone = ZoneType.Safe;
                ChangeColor(Color.lightYellow);
                break;
            case 12:
                zone = ZoneType.Hunt;
                ChangeColor(Color.lightPink);
                break;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 8)
            Blocked = false;
    }
}