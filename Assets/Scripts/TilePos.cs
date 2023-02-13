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

}
