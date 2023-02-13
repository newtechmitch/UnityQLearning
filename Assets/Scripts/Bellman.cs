using System.Linq;
using UnityEngine;

public class Bellman : MonoBehaviour
{
    [SerializeField] private TileGrid tileGrid;

    [Header("Learning parameters")]
    [SerializeField] private float gamma = 0.9f;

    [Header("Controls")]
    [SerializeField] private bool automatic;

    private void Start()
    {
        tileGrid.GenerateTiles();
    }

    private void Update()
    {
        if (automatic || Input.GetKeyDown(KeyCode.Space))
        {
            for (var y = 0; y < TileGrid.BOARD_HEIGHT; y++)
            {
                for (var x = 0; x < TileGrid.BOARD_WIDTH; x++)
                {
                    var tile = tileGrid.GetTileByCoords<VTile>(x, y);
                    tile.Value = Agent.Actions
                        .Select(a => tileGrid.GetTargetTile(tile, a))
                        .Select(t => t == tile ? tile.Reward : tile.Reward + gamma * t.Value)
                        .Max();
                }
            }
        }
    }
}