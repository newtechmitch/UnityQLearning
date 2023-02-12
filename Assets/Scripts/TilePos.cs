// Created by Dr. Adam Streck, 2023, adam.streck@gmail.com


using UnityEngine;

public record TilePos
{
    public int X;
    public int Y;
    public TilePos(int x, int y)
    {
        X = x;
        Y = y;
    }
    private const float TILE_SIZE = 128f;
    public static Vector3 LogicalToLocalPos(TilePos pos)
        => new(TILE_SIZE * pos.X, -TILE_SIZE * pos.Y, 0);
}
