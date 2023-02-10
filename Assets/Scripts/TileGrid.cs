// Created by Dr. Adam Streck, 2-123, adam.streck@gmail.com

using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    [SerializeField] private Tile grassTile;
    [SerializeField] private Tile waterTile;
    [SerializeField] private Tile awardTile;
    [SerializeField] private Agent agentPrefab;
    
    [SerializeField] private float gamma = 0.9f;
    [SerializeField] private int iterations = 1000;
    
    private int[,] _rewards;
    private float[,] _qValues;
    private Tile[,] _tiles;
    private Agent _agent;
    private int _counter;

    private const float TILE_SIZE = 128f;
    private const int BOARD_WIDTH = 5;
    private const int BOARD_HEIGHT = 9;
    private const int START_X = 2;
    private const int START_Y = 6;
    

    private void Start()
    {
        GenerateRewards();
        InitializeQValues();
        GenerateTiles();
        InstantiateAgent();
        ResetAgentPos();
    }

    private void Update()
    {
        if (_counter++ < iterations)
        {
            int rndX = Random.Range(0, BOARD_WIDTH);
            int rndY = Random.Range(0, BOARD_HEIGHT);
            float newQ = CalcQ(rndX, rndY);
            _agent.transform.localPosition = LogicalToLocalPos(rndX, rndY);
            _qValues[rndY, rndX] = newQ;
            _tiles[rndY, rndX].SetQValue(newQ);
        }
        else
        {
            int rndX = START_X;
            int rndY = START_Y;
            float newQ = CalcQ(rndX, rndY);
            _agent.transform.localPosition = LogicalToLocalPos(rndX, rndY);
            _qValues[rndY, rndX] = newQ;
            _tiles[rndY, rndX].SetQValue(newQ);
        }
    }
    
    private void InitializeQValues()
    {
        _qValues = new float[BOARD_HEIGHT, BOARD_WIDTH];
    }

    private Vector3 LogicalToLocalPos(int x, int y) 
        => new(TILE_SIZE * x, -TILE_SIZE * y, 0);
    
    private void ResetAgentPos()
    {
        _agent.transform.localPosition = LogicalToLocalPos(START_X, START_Y);
    }
    
    private void InstantiateAgent()
    {
        _agent = Instantiate(agentPrefab, transform);
    }

    private void GenerateRewards()
    {
        _rewards = new[,]
        {
            {-1, -1, -1, -1, -1},
            {-1, 0, 0, 0, -1},
            {-1, 0, 1, 0, -1},
            {-1, 0, 0, 0, -1},
            {-1, -1, 0, -1, -1},
            {-1, 0, 0, 0, -1}, 
            {-1, 0, 0, 0, -1},
            {-1, 0, 0, 0, -1},
            {-1, -1, -1, -1, -1}
        };
    }

    private float CalcQ(int x, int y)
    {
        float q = float.NegativeInfinity;
        for (int dy = -1; dy <= 1; dy += 1)
        {
            if (y + dy > 0 && y + dy < BOARD_HEIGHT)
            {
                for (int dx = -1; dx <= 1; dx += 1)
                {
                    if (x + dx > 0 && x + dx < BOARD_WIDTH)
                    {
                        float newQ = _rewards[y + dy, x + dx] + gamma * _qValues[y + dy, x + dx];
                        q = Mathf.Max(q, newQ);
                    }
                }
            }
        }
        return q;
    }

    private Tile GetTileByReward(int reward)
        => reward switch
        {
            -1 => waterTile,
            0 => grassTile,
            1 => awardTile,
            _ => null
        };

    private void GenerateTiles()
    {
        _tiles = new Tile[BOARD_HEIGHT, BOARD_WIDTH];
        for (int y = 0; y < BOARD_HEIGHT; y++)
        {
            for (int x = 0; x < BOARD_WIDTH; x++)
            {
                var tilePrefab = GetTileByReward(_rewards[y, x]);
                var newTile = Instantiate(tilePrefab, transform);
                newTile.SetReward(_rewards[y, x]);
                newTile.SetQValue(0);
                newTile.transform.localPosition = LogicalToLocalPos(x, y);
                _tiles[y, x] = newTile;
            }
        }
    }
}