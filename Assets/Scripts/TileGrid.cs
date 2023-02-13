using UnityEngine;
using UnityEngine.UI;

public class TileGrid : MonoBehaviour
{
    [SerializeField] private Text labelPrefab;
    [SerializeField] private BaseTile awardTile;
    [SerializeField] private BaseTile grassTile;
    [SerializeField] private BaseTile waterTile;

    [SerializeField] private float awardReward = 1;
    [SerializeField] private float grassReward = 0;
    [SerializeField] private float waterReward = -1;

    private readonly int[,] _map =
    {
        { -1, -1, -1, -1, -1, -1, -1, -1 },
        { -1,  0,  0,  0, -1,  0,  1, -1 },
        { -1,  0,  0,  0, -1,  0,  0, -1 },
        { -1,  0,  0,  0,  0,  0,  0, -1 },
        { -1, -1, -1, -1, -1, -1, -1, -1 }
    };

    private BaseTile[,] _tiles;

    public const int BOARD_WIDTH = 8;
    public const int BOARD_HEIGHT = 5;
    private const float TILE_SIZE = 128f;
    private const float OFFSET = 64f;

    private BaseTile TilePrefabByType(TileEnum tileEnum)
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

    public T GetTargetTile<T>(T source, ActionEnum action) where T : BaseTile
        => (T)TileByPos(GetTargetPos(source.CurrentPos, action));

    public T GetTileByCoords<T>(int x, int y) where T : BaseTile
        => (T)_tiles[y, x];


    public static Vector3 LogicalToLocalPos(TilePos pos)
        => new(OFFSET + TILE_SIZE * pos.X, -OFFSET + -TILE_SIZE * pos.Y, 0);

    private static Vector3 XLabelPos(int x)
        => new(OFFSET * 1.5f + TILE_SIZE * x, 0, 0);

    private static Vector3 YLabelPos(int y)
        => new(0, -OFFSET * 1.5f - TILE_SIZE * y, 0);

    private BaseTile TileByPos(TilePos pos)
        => _tiles[pos.Y, pos.X];

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

    public void GenerateTiles()
    {
        // Tiles
        _tiles = new BaseTile[BOARD_HEIGHT, BOARD_WIDTH];
        for (var y = 0; y < BOARD_HEIGHT; y++)
        {
            for (var x = 0; x < BOARD_WIDTH; x++)
            {
                var tileType = (TileEnum)_map[y, x];
                var tilePrefab = TilePrefabByType(tileType);
                var newTile = Instantiate(tilePrefab, transform);
                newTile.Reward = RewardByType(tileType);
                newTile.CurrentPos = new TilePos(x, y);
                _tiles[y, x] = newTile;
            }
        }
        // X labels
        for (var x = 0; x < BOARD_WIDTH; x++)
        {
            var newLabel = Instantiate(labelPrefab, transform);
            newLabel.name = "x" + x;
            newLabel.text = x.ToString();
            newLabel.transform.localPosition = XLabelPos(x);
        }
        // Y labels
        for (var y = 0; y < BOARD_HEIGHT; y++)
        {
            var newLabel = Instantiate(labelPrefab, transform);
            newLabel.name = "y" + y;
            newLabel.text = y.ToString();
            newLabel.transform.localPosition = YLabelPos(y);
        }
    }
}