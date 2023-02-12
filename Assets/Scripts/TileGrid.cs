// Created by Dr. Adam Streck, 2023, adam.streck@gmail.com

using System;
using System.Linq;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    [SerializeField] private QTile grassTile;
    [SerializeField] private QTile waterTile;
    [SerializeField] private QTile awardTile;

    [SerializeField] private float waterReward = -1;
    [SerializeField] private float grassReward = 0;
    [SerializeField] private float awardReward = 1;
    
    private readonly TileEnum[,] _map =
    {
        { TileEnum.Water, TileEnum.Water, TileEnum.Water, TileEnum.Water, TileEnum.Water },
        { TileEnum.Water, TileEnum.Grass, TileEnum.Grass, TileEnum.Grass, TileEnum.Water },
        { TileEnum.Water, TileEnum.Grass, TileEnum.Award, TileEnum.Grass, TileEnum.Water },
        { TileEnum.Water, TileEnum.Grass, TileEnum.Grass, TileEnum.Grass, TileEnum.Water },
        { TileEnum.Water, TileEnum.Water, TileEnum.Grass, TileEnum.Water, TileEnum.Water },
        { TileEnum.Water, TileEnum.Grass, TileEnum.Grass, TileEnum.Grass, TileEnum.Water },
        { TileEnum.Water, TileEnum.Grass, TileEnum.Grass, TileEnum.Grass, TileEnum.Water },
        { TileEnum.Water, TileEnum.Grass, TileEnum.Grass, TileEnum.Grass, TileEnum.Water },
        { TileEnum.Water, TileEnum.Water, TileEnum.Water, TileEnum.Water, TileEnum.Water }
    };

    private QTile[,] _tiles;

    private const int BOARD_WIDTH = 5;
    private const int BOARD_HEIGHT = 9;

    private QTile TilePrefabByType(TileEnum tileEnum)
        => tileEnum switch
        {
            TileEnum.Water => waterTile,
            TileEnum.Grass => grassTile,
            TileEnum.Award => awardTile,
            _ => null
        };

    private double RewardByType(TileEnum tileEnum)
        => tileEnum switch
        {
            TileEnum.Water => waterReward,
            TileEnum.Grass => grassReward,
            TileEnum.Award => awardReward,
            _ => 0
        };
    
    public QTile GetTargetTile(QTile source, ActionEnum action)
        => TileByPos(GetTargetPos(source.CurrentPos, action));
    
    public QTile GetStartTile(int x, int y)
        => _tiles[y, x];

    private QTile TileByPos(TilePos pos)
        => _tiles[pos.Y, pos.X];

    // Bounded move on a grass, otherwise stay
    private TilePos GetTargetPos(TilePos source, ActionEnum action)
    {
        var tile = _tiles[source.Y, source.X];
        if (tile.TileType == TileEnum.Grass)
        {
            return action switch
            {
                ActionEnum.Up when source.Y > 0 => source with { Y = source.Y - 1 },
                ActionEnum.Down when source.Y < BOARD_HEIGHT - 1 => source with { Y = source.Y + 1 },
                ActionEnum.Left when source.X > 0 => source with { X = source.X - 1 },
                ActionEnum.Right when source.X < BOARD_WIDTH - 1 => source with { X = source.X + 1 },
                _ => source
            };
        }
        return source;
    }

    public void GenerateTiles()
    {
        _tiles = new QTile[BOARD_HEIGHT, BOARD_WIDTH];
        for (var y = 0; y < BOARD_HEIGHT; y++)
        {
            for (var x = 0; x < BOARD_WIDTH; x++)
            {
                var tileType = _map[y, x];
                var tilePrefab = TilePrefabByType(tileType);
                var newTile = Instantiate(tilePrefab, transform);
                newTile.Reward = RewardByType(tileType);
                Agent.Actions.ForEach(a => newTile.SetQValue(a, 0));
                newTile.CurrentPos = new TilePos(x, y);
                _tiles[y, x] = newTile;
            }
        }
    }
}