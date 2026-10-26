using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Graph : MonoBehaviour
{
    public static Graph current;

    [Header("Grid setup")]
    [SerializeField] int gridWidth = 10;
    [SerializeField] int gridHeight = 10;
    [SerializeField] float nodeOffset = 1f;

    [Header("References")]
    [SerializeField] Node nodePrefab;

    Dictionary<Coordinates, Node> grid;
    public List<Node> GetAllGridNodes => grid.Values.ToList();
    public List<Node> GetAllWalkableNodes => GetAllGridNodes.Where(n => !n.Blocked).ToList();

    void Awake()
    {
        current = this;
        GenerateGrid();
    }

    void GenerateGrid()
    {
        grid = new();

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Node node = Instantiate(nodePrefab, transform);
                node.transform.position = new(x * nodeOffset, 0f, y * nodeOffset);

                Coordinates coord = new(x, y);
                node.Setup(new Coordinates(x,y), 1);

                grid.Add(coord, node);
            }
        }
    }

    public List<Node> GetNeighbors(Node current)
    {
        List<Node> neighbors = new();

        foreach (Coordinates coord in current.Coordinates.Directions)
        {
            if (!IsInsideGrid(coord)) continue;
            neighbors.Add(grid[coord]);
        }

        return neighbors;
    }

    public Node GetNodeFromPosition(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x / nodeOffset);
        int y = Mathf.RoundToInt(position.z / nodeOffset);

        Coordinates coord = new(x, y);

        if (grid.TryGetValue(coord, out Node node))
            return node;

        return null;
    }

    bool IsInsideGrid(Coordinates coord) =>
        coord.X >= 0 && coord.X < gridWidth &&
        coord.Y >= 0 && coord.Y < gridHeight;
}