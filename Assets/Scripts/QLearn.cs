// Created by Dr. Adam Streck, 2023, adam.streck@gmail.com

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
    [SerializeField] private float stepsPerSecond = 10f;

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

    private System.Collections.IEnumerator AutoStep()
    {
        while (automatic)
        {
            yield return new WaitForSeconds(stepsPerSecond > 0f ? 1f / stepsPerSecond : 0f);
            Step();
        }
        yield return null;
    }
    
    private ActionEnum GetAction(QTile state)
        => Random.Range(0f, 1f) > _epsilon 
            ? Agent.Actions.Shuffle().OrderBy(state.GetQValue).Last() 
            : Agent.RndAction();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            automatic = !automatic;
            if (automatic)
            {
                StartCoroutine(nameof(AutoStep));
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Step();
        }
    }

    private void Step()
    {
        if (_agent.State.TileType != TileEnum.Grass)
        {
            ResetAgentPos();
        }
        else
        {
            QTile s = _agent.State;

            // Update Q-values for ALL actions from current state
            foreach (var a in Agent.Actions)
            {
                double q = s.GetQValue(a);
                QTile sPrime = tileGrid.GetTargetTile(s, a);
                double r = sPrime.Reward;
                double qMax = Agent.Actions.Select(sPrime.GetQValue).Max();
                double td = r + gamma * qMax - q;
                s.SetQValue(a, q + alpha * td);
            }

            // Now pick action based on updated Q-values and move
            ActionEnum chosen = GetAction(s);
            _agent.State = tileGrid.GetTargetTile(s, chosen);
            _epsilon = Mathf.Max(epsilonEnd, _epsilon - epsilonDecay);
        }
        Debug.Log($"Step {_counter++}, epsilon {_epsilon}");
    }

    private void ResetAgentPos()
    {
        _agent.State = tileGrid.GetTileByCoords<QTile>(START_X, START_Y);
    }
}