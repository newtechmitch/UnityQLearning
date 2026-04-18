import { BaseTile } from "./BaseTile";
import { TileEnum, TilePos, TILE_SIZE } from "../types";
import { TextSprite, valueColor } from "../textSprite";

// VTile — carries a single V(s) value and a NextValue used for the two-phase
// Bellman update (see Bellman.cs: writes NextValue then copies in Step()).

export class VTile extends BaseTile {
  private _value = 0;
  nextValue = 0;
  private readonly valueSprite: TextSprite;

  constructor(type: TileEnum, pos: TilePos) {
    super(type, pos);

    this.valueSprite = new TextSprite(this.formatted(0), {
      worldWidth: 0.8,
      worldHeight: 0.28,
      color: valueColor(0),
      font: "700 64px Inter, system-ui, sans-serif",
    });
    this.valueSprite.sprite.position.set(0, -TILE_SIZE * 0.22, 0.1);
    this.group.add(this.valueSprite.sprite);
  }

  get value(): number {
    return this._value;
  }

  set value(v: number) {
    this._value = v;
    this.valueSprite.draw(this.formatted(v), valueColor(v));
  }

  step(): void {
    this.value = this.nextValue;
  }

  private formatted(v: number): string {
    return v.toFixed(4);
  }
}
