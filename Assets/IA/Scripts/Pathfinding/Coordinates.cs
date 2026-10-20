using System.Collections.Generic;

public struct Coordinates
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public readonly Coordinates Up => new(X, Y + 1);
    public readonly Coordinates Down => new(X, Y - 1);
    public readonly Coordinates Right => new(X + 1, Y);
    public readonly Coordinates Left => new(X - 1, Y);
    public readonly Coordinates UpRight => new(X + 1, Y + 1);
    public readonly Coordinates UpLeft => new(X - 1, Y + 1);
    public readonly Coordinates DownRight => new(X + 1, Y - 1);
    public readonly Coordinates DownLeft => new(X - 1, Y - 1);

    public readonly List<Coordinates> Directions => new() { Up, Left, Down, Right, UpRight, UpLeft, DownRight, DownLeft };

    public Coordinates(int x, int y)
    {
        X = x;
        Y = y;
    }

    public readonly override string ToString() => $"({X},{Y})";
}