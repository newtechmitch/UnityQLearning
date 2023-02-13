using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BaseTile : MonoBehaviour
{
    [SerializeField] private TileEnum tileType;
    [SerializeField] private Text rewardText;

    protected static Color TextColor(double val)
        => val < 0 
            ? Color.Lerp(Color.white, Color.red, Mathf.Clamp01((float) Math.Abs(val))) 
            : Color.Lerp(Color.white, Color.green, Mathf.Clamp01((float) val));

    private double _reward;
    public double Reward
    {
        get => _reward;
        set
        {
             _reward = value;
             rewardText.text = _reward.ToString("F4");
             rewardText.color = TextColor(value);
        }
    }

    public TileEnum TileType => tileType;
    
    private TilePos _currentPos;
    public TilePos CurrentPos
    {
        get => _currentPos;
        set
        {
            _currentPos = value;
            transform.localPosition = TilePos.LogicalToLocalPos(_currentPos);
        }
    }
}
