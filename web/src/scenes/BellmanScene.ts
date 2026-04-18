import { TileGrid } from "../TileGrid";
import { VTile } from "../tiles/VTile";
import { ALL_ACTIONS, BOARD_HEIGHT, BOARD_WIDTH, TileEnum } from "../types";
import { DemoScene, ParamDescriptor } from "./Scene";

// Bellman value-iteration demo. Mirrors Bellman.cs: two-phase update —
// compute NextValue for every grass tile, then copy NextValue→Value in Step().

export class BellmanScene implements DemoScene {
  readonly title = "Bellman";
  private readonly grid: TileGrid<VTile>;
  private auto = false;
  private iteration = 0;
  private gamma = 0.9;
  private autoTimer = 0;
  private readonly autoInterval = 0.25; // seconds between auto steps

  constructor() {
    this.grid = new TileGrid<VTile>((type, pos) => new VTile(type, pos));
  }

  get root() {
    return this.grid.root;
  }

  readonly params: ParamDescriptor[] = [
    { key: "gamma", label: "γ (discount)", value: 0.9, min: 0, max: 1, step: 0.01 },
  ];

  onParam(key: string, value: number): void {
    if (key === "gamma") this.gamma = value;
  }

  private newValueForTile(tile: VTile): number {
    let best = -Infinity;
    for (const a of ALL_ACTIONS) {
      const t = this.grid.getTargetTile(tile, a);
      const candidate = t.reward + this.gamma * t.value;
      if (candidate > best) best = candidate;
    }
    return best;
  }

  step(): void {
    // Phase 1: write NextValue for every grass tile.
    for (let y = 0; y < BOARD_HEIGHT; y++) {
      for (let x = 0; x < BOARD_WIDTH; x++) {
        const tile = this.grid.tileAt(x, y);
        if (tile.tileType === TileEnum.Grass) {
          tile.nextValue = this.newValueForTile(tile);
        }
      }
    }
    // Phase 2: commit NextValue → Value.
    for (const tile of this.grid.enumerate()) tile.step();
    this.iteration += 1;
  }

  setAuto(on: boolean): void {
    this.auto = on;
    this.autoTimer = 0;
  }

  isAuto(): boolean {
    return this.auto;
  }

  reset(): void {
    for (const tile of this.grid.enumerate()) {
      tile.nextValue = 0;
      tile.value = 0;
    }
    this.iteration = 0;
  }

  tickAuto(dt: number): void {
    if (!this.auto) return;
    this.autoTimer += dt;
    while (this.autoTimer >= this.autoInterval) {
      this.autoTimer -= this.autoInterval;
      this.step();
    }
  }

  stats(): Record<string, string> {
    return {
      iteration: String(this.iteration),
      γ: this.gamma.toFixed(2),
    };
  }

  dispose(): void {
    this.grid.dispose();
  }
}
