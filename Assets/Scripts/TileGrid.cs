// Created by Dr. Adam Streck, 2023, adam.streck@gmail.com

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class TileGrid : MonoBehaviour
{
    [SerializeField] private Tile grassTile;
    [SerializeField] private Tile waterTile;
    [SerializeField] private Tile awardTile;
    [SerializeField] private Agent agentPrefab;

    [SerializeField] private bool automatic;
    [SerializeField] private float waterReward = -1;
    [SerializeField] private float grassReward = 0;
    [SerializeField] private float awardReward = 1;
    [SerializeField] private float alpha = 0.1f;
    [SerializeField] private float gamma = 0.9f;

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

    private readonly List<ActionEnum> _actions = new()
        { ActionEnum.Left, ActionEnum.Right, ActionEnum.Up, ActionEnum.Down };

    private Tile[,] _tiles;
    private Agent _agent;
    private int _counter;

    private const float TILE_SIZE = 128f;
    private const int BOARD_WIDTH = 5;
    private const int BOARD_HEIGHT = 9;
    private const int START_X = 2;
    private const int START_Y = 6;

    private Tile TilePrefabByType(TileEnum tileEnum)
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

    private Tile TileByPos(TilePos pos)
        => _tiles[pos.Y, pos.X];

    public static Vector3 LogicalToLocalPos(int x, int y)
        => new(TILE_SIZE * x, -TILE_SIZE * y, 0);

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

    private void Start()
    {
        GenerateTiles();
        InstantiateAgent();
        ResetAgentPos();
    }

    private void Update()
    {
        if (automatic || Input.GetKeyDown(KeyCode.Space))
        {

            var statePos = _agent.CurrentPos;
            var stateTile = TileByPos(statePos);
            var randomAction = _actions[Random.Range(0, _actions.Count)];
            var targetPos = GetTargetPos(statePos, randomAction);
            var targetState = TileByPos(targetPos);
            var currentQ = stateTile.GetQValue(randomAction);
            var reward = targetState.Reward;
            var maxQNext = _actions.Select(a => targetState.GetQValue(a)).Max();
            var temporalDifference = reward + gamma * maxQNext - currentQ;
            var newQ = currentQ + alpha * temporalDifference;
            stateTile.SetQValue(randomAction, newQ);
            if (stateTile.TileType == TileEnum.Grass)
            {
                _agent.CurrentPos = targetPos;
            }
            else
            {
                ResetAgentPos();
            }
        }
    }

    private void ResetAgentPos()
    {
        _agent.CurrentPos = new TilePos(START_X, START_Y);
    }

    private void InstantiateAgent()
    {
        _agent = Instantiate(agentPrefab, transform);
    }

    private void GenerateTiles()
    {
        _tiles = new Tile[BOARD_HEIGHT, BOARD_WIDTH];
        for (var y = 0; y < BOARD_HEIGHT; y++)
        {
            for (var x = 0; x < BOARD_WIDTH; x++)
            {
                var tileType = _map[y, x];
                var tilePrefab = TilePrefabByType(tileType);
                var newTile = Instantiate(tilePrefab, transform);
                newTile.Reward = RewardByType(tileType);
                _actions.ForEach(a => newTile.SetQValue(a, 0));
                newTile.CurrentPos = new TilePos(x, y);
                _tiles[y, x] = newTile;
            }
        }
    }
}