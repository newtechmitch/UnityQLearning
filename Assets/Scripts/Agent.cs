using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public static ActionEnum RndAction()
        => (ActionEnum) Random.Range(0, 4);
    
    public static readonly List<ActionEnum> Actions = new()
        { ActionEnum.Left, ActionEnum.Right, ActionEnum.Up, ActionEnum.Down };

    private QTile _state;
    public QTile State
    {
        get => _state;
        set
        {
            _state = value;
            transform.localPosition = TileGrid.LogicalToLocalPos(_state.CurrentPos);
        }
    }
}
