using System.Linq;
using UnityEngine;

public class Bellman : MonoBehaviour
{
    [SerializeField] private TileGrid tileGrid;

    [Header("Learning parameters")]
    [SerializeField] private float gamma = 0.9f;

    [Header("Controls")]
    [SerializeField] private bool automatic;
    
    private double GetNewValue(VTile tile)
    {
        return Agent.Actions
            .Select(a => tileGrid.GetTargetTile(tile, a))
            .Select(t => t.Reward + gamma * t.Value)
            .Max();
    }

    private void CalculateValues()
    {
        for (var y = 0; y < TileGrid.BOARD_HEIGHT; y++)
        {
            for (var x = 0; x < TileGrid.BOARD_WIDTH; x++)
            {
                var tile = tileGrid.GetTileByCoords<VTile>(x, y);
                if (tile.TileType == TileEnum.Grass)
                {
                    tile.NextValue = GetNewValue(tile);
                }
            }
        }
    }

    private void Step()
    {
        for (var y = 0; y < TileGrid.BOARD_HEIGHT; y++)
        {
            for (var x = 0; x < TileGrid.BOARD_WIDTH; x++)
            {
                tileGrid.GetTileByCoords<VTile>(x, y).Step();
            }
        }
    }
    
    private void Start()
    {
        tileGrid.GenerateTiles();
    }

    private void Update()
    {        
        if (Input.GetKeyDown(KeyCode.A)) {
            automatic = !automatic;
        }
        if (automatic || Input.GetKeyDown(KeyCode.Space))
        {
            CalculateValues();
            Step();
        }
    }
}