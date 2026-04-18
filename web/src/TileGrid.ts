import * as THREE from "three";
import {
  ActionEnum,
  BOARD_HEIGHT,
  BOARD_WIDTH,
  MAP,
  TileEnum,
  TilePos,
  TILE_SIZE,
} from "./types";
import { BaseTile } from "./tiles/BaseTile";
import { TextSprite } from "./textSprite";

// TileGrid — port of Unity's TileGrid.cs. Generic over the concrete tile class
// so the Bellman demo can populate VTiles and the QLearn demo QTiles.
// Movement rule (GetTargetPos) is identical: only grass tiles can transition;
// water/award act as absorbing states.

export class TileGrid<T extends BaseTile> {
  readonly root: THREE.Group;
  private readonly tiles: T[][] = [];

  constructor(private readonly make: (type: TileEnum, pos: TilePos) => T) {
    this.root = new THREE.Group();
    this.generate();
    this.generateAxisLabels();
    // Centre the grid around the world origin.
    this.root.position.set(
      -((BOARD_WIDTH - 1) * TILE_SIZE) / 2,
      ((BOARD_HEIGHT - 1) * TILE_SIZE) / 2,
      0,
    );
  }

  // Row/column labels outside the grid — matches the "0..7 / 0..4" axis
  // labels visible in the article's screenshots (Docs/QLearnBase.png etc).
  private generateAxisLabels(): void {
    for (let x = 0; x < BOARD_WIDTH; x++) {
      const label = new TextSprite(String(x), {
        worldWidth: 0.4,
        worldHeight: 0.4,
        color: "#e7eaf0",
        font: "700 72px Inter, system-ui, sans-serif",
      });
      label.sprite.position.set(x * TILE_SIZE, TILE_SIZE * 0.75, 0.1);
      this.root.add(label.sprite);
    }
    for (let y = 0; y < BOARD_HEIGHT; y++) {
      const label = new TextSprite(String(y), {
        worldWidth: 0.4,
        worldHeight: 0.4,
        color: "#e7eaf0",
        font: "700 72px Inter, system-ui, sans-serif",
      });
      label.sprite.position.set(-TILE_SIZE * 0.75, -y * TILE_SIZE, 0.1);
      this.root.add(label.sprite);
    }
  }

  private generate(): void {
    for (let y = 0; y < BOARD_HEIGHT; y++) {
      this.tiles[y] = [];
      for (let x = 0; x < BOARD_WIDTH; x++) {
        const type = MAP[y][x] as TileEnum;
        const tile = this.make(type, { x, y });
        this.tiles[y][x] = tile;
        this.root.add(tile.group);
      }
    }
  }

  tileAt(x: number, y: number): T {
    return this.tiles[y][x];
  }

  *enumerate(): IterableIterator<T> {
    for (let y = 0; y < BOARD_HEIGHT; y++) {
      for (let x = 0; x < BOARD_WIDTH; x++) {
        yield this.tiles[y][x];
      }
    }
  }

  // Mirror of GetTargetPos: bounded move only from grass, otherwise stay.
  getTargetPos(source: TilePos, action: ActionEnum): TilePos {
    const tile = this.tiles[source.y][source.x];
    if (tile.tileType !== TileEnum.Grass) return source;

    switch (action) {
      case ActionEnum.Up:
        return source.y > 0 ? { x: source.x, y: source.y - 1 } : source;
      case ActionEnum.Down:
        return source.y < BOARD_HEIGHT - 1 ? { x: source.x, y: source.y + 1 } : source;
      case ActionEnum.Left:
        return source.x > 0 ? { x: source.x - 1, y: source.y } : source;
      case ActionEnum.Right:
        return source.x < BOARD_WIDTH - 1 ? { x: source.x + 1, y: source.y } : source;
    }
  }

  getTargetTile(source: T, action: ActionEnum): T {
    const pos = this.getTargetPos(source.currentPos, action);
    return this.tiles[pos.y][pos.x];
  }

  dispose(): void {
    this.root.traverse(obj => {
      const mesh = obj as THREE.Mesh;
      if (mesh.geometry) mesh.geometry.dispose();
      const mat = mesh.material as THREE.Material | THREE.Material[] | undefined;
      if (Array.isArray(mat)) mat.forEach(m => m.dispose());
      else if (mat) mat.dispose();
    });
  }
}
