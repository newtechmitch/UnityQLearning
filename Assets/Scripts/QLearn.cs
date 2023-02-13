// Created by Dr. Adam Streck, 2023, adam.streck@gmail.com

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QLearn : MonoBehaviour
{
    private const int START_X = 2;
    private const int START_Y = 2;

    [Header("References")]
    [SerializeField] private TileGrid tileGrid;
    [SerializeField] private Agent agentPrefab;

    [Header("Learning parameters")]
    [SerializeField] private float alpha = 0.1f;
    [SerializeField] private float epsilonStart = 1f;
    [SerializeField] private float epsilonEnd = 0.05f;
    [SerializeField] private float epsilonDecay = 0.001f;
    [SerializeField] private float gamma = 0.9f;

    [Header("Controls")]
    [SerializeField] private bool automatic;

    private Agent _agent;
    private int _counter;
    private float _epsilon;

    private void Start()
    {
        tileGrid.GenerateTiles();
        _agent = Instantiate(agentPrefab, transform);
        _epsilon = epsilonStart;
        ResetAgentPos();
    }
    
    private ActionEnum GetAction(QTile state)
        => Random.Range(0f, 1f) > _epsilon 
            ? Agent.Actions.Shuffle().OrderBy(state.GetQValue).Last() 
            : Agent.RndAction();

    private void Update()
    {
        if (automatic || Input.GetKeyDown(KeyCode.Space))
        {
            if (_agent.State.TileType != TileEnum.Grass)
            {
                ResetAgentPos();
            }
            else
            {
                var s = _agent.State;
                var a = GetAction(s);
                var sPrime = tileGrid.GetTargetTile(s, a);
                var q = s.GetQValue(a);
                var r = sPrime.Reward;
                var qMax = Agent.Actions.Select(aPrime => sPrime.GetQValue(aPrime)).Max();
                var td = r + gamma * qMax - q;
                var newQ = q + alpha * td;
                s.SetQValue(a, newQ);
                _agent.State = sPrime;
                _epsilon = Mathf.Max(epsilonEnd, _epsilon - epsilonDecay);
            }
            Debug.Log($"Step {_counter++}, epsilon {_epsilon}");
        }
    }

    private void ResetAgentPos()
    {
        _agent.State = tileGrid.GetTileByCoords<QTile>(START_X, START_Y);
    }
}