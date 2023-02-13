// Created by Dr. Adam Streck, 2023, adam.streck@gmail.com

using System.Linq;
using UnityEngine;

public class QLearn : MonoBehaviour
{
    private const int START_X = 2;
    private const int START_Y = 6;
    
    [Header("References")]
    [SerializeField] private TileGrid tileGrid;
    [SerializeField] private Agent agentPrefab;
    
    [Header("Learning parameters")]
    [SerializeField] private float alpha = 0.1f;
    [SerializeField] private float gamma = 0.9f;
    
    [Header("Controls")]
    [SerializeField] private bool automatic;

    private Agent _agent;
    private int _counter;
    
    private void Start()
    {
        tileGrid.GenerateTiles();
        _agent = Instantiate(agentPrefab, transform);
        ResetAgentPos();
    }

    private void Update()
    {
        if (automatic || Input.GetKeyDown(KeyCode.Space))
        {
            var s = _agent.State;
            var a = Agent.RndAction();
            var sPrime = tileGrid.GetTargetTile<QTile>(s, a);
            var q = s.GetQValue(a);
            var r = sPrime.Reward;
            var qMax = Agent.Actions.Select(aPrime => sPrime.GetQValue(aPrime)).Max();
            var TD = r + gamma * qMax - q;
            var newQ = q + alpha * TD;
            s.SetQValue(a, newQ);
            if (s.TileType == TileEnum.Grass)
            {
                _agent.State = sPrime;
            }
            else
            {
                ResetAgentPos();
            }
        }
    }

    private void ResetAgentPos()
    {
        _agent.State = tileGrid.GetTileByCoords<QTile>(START_X, START_Y);
    }
}