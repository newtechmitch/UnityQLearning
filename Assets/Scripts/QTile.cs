using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QTile : BaseTile
{
    [SerializeField] private List<Text> qValueTexts;

    private readonly double[] _qValues = new double[4];

    private void Start()
    {
        Agent.Actions.ForEach(action => SetQValue(action, 0));
    }

    public void SetQValue(ActionEnum action, double value)
    {
        var index = (int)action;
        _qValues[index] = value;
        qValueTexts[index].text = value.ToString("F3");
        qValueTexts[index].color = TextColor(value);
    }

    public double GetQValue(ActionEnum action)
        => _qValues[(int)action];
}