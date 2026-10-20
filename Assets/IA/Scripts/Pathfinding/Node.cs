using System;
using TMPro;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField] Renderer _renderer; // borar si no hace falta visualizar para el parcial
    [SerializeField] GameObject wallGameObject;
    [SerializeField] TextMeshPro costText; // borar si no hace falta visualizar para el parcial

    public Coordinates Coordinates { get; private set; }

    bool blocked = false;
    public bool Blocked 
    { 
        get => blocked; 

        private set
        {
            blocked = value;
            wallGameObject.SetActive(value);
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

    public void ChangeColor(Color color) => _renderer.material.color = color; // borar si no hace falta visualizar para el parcial

    void OnValidate() // borar si no hace falta visualizar para el parcial
    {
        if (_renderer != null) return;
        if (!TryGetComponent(out _renderer)) Debug.LogError("This gameobject doesn't have Renderer component");
    }
}