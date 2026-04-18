import { BaseTile } from "./BaseTile";
import { ActionEnum, ALL_ACTIONS, TileEnum, TilePos, TILE_SIZE } from "../types";
import { TextSprite, valueColor } from "../textSprite";

// QTile — four Q-values displayed on each edge of the tile, one per action.
// Placement matches the Unity prefab layout: Left/Right on the sides, Up/Down
// top/bottom.

// Compass layout matching the article's screenshots: reward at top,
// Up below the reward, Left/Right on the sides, Down at the bottom.
const LABEL_POS: Record<ActionEnum, [number, number]> = {
  [ActionEnum.Left]:  [-TILE_SIZE * 0.30, -TILE_SIZE * 0.10],
  [ActionEnum.Right]: [ TILE_SIZE * 0.30, -TILE_SIZE * 0.10],
  [ActionEnum.Up]:    [ 0,                -TILE_SIZE * 0.05],
  [ActionEnum.Down]:  [ 0,                -TILE_SIZE * 0.35],
};

export class QTile extends BaseTile {
  private readonly qValues: number[] = [0, 0, 0, 0];
  private readonly qSprites: TextSprite[] = [];

  constructor(type: TileEnum, pos: TilePos) {
    super(type, pos);

    for (const action of ALL_ACTIONS) {
      const sprite = new TextSprite("0.000", {
        worldWidth: 0.42,
        worldHeight: 0.18,
        color: valueColor(0),
        font: "700 44px Inter, system-ui, sans-serif",
      });
      const [x, y] = LABEL_POS[action];
      sprite.sprite.position.set(x, y, 0.1);
      this.qSprites[action] = sprite;
      this.group.add(sprite.sprite);
    }
  }

  getQ(a: ActionEnum): number {
    return this.qValues[a];
  }

  setQ(a: ActionEnum, v: number): void {
    this.qValues[a] = v;
    this.qSprites[a].draw(v.toFixed(3), valueColor(v));
  }
}
