import * as THREE from "three";
import { TileGrid } from "../TileGrid";
import { QTile } from "../tiles/QTile";
import { Agent, randomAction } from "../Agent";
import { ActionEnum, ALL_ACTIONS, START_POS, TileEnum } from "../types";
import { DemoScene, ParamDescriptor } from "./Scene";

// Q-Learning demo. Mirrors QLearn.cs: on each step we update Q-values for all
// four actions from the current state (pedagogical — differs from textbook
// single-action Q-learning), then pick an action via ε-greedy and move.

export class QLearnScene implements DemoScene {
  readonly title = "Q-Learning";
  readonly root: THREE.Group;

  private readonly grid: TileGrid<QTile>;
  private readonly agent: Agent;

  private alpha = 0.1;
  private gamma = 0.9;
  private epsilonStart = 1.0;
  private epsilonEnd = 0.05;
  private epsilonDecay = 0.001;
  private stepsPerSecond = 10;

  private epsilon = 1.0;
  private counter = 0;
  private auto = false;
  private autoAcc = 0;

  constructor() {
    this.grid = new TileGrid<QTile>((type, pos) => new QTile(type, pos));
    this.agent = new Agent();
    this.root = new THREE.Group();
    this.root.add(this.grid.root);
    this.grid.root.add(this.agent.mesh);
    this.reset();
  }

  readonly params: ParamDescriptor[] = [
    { key: "alpha", label: "α (learning rate)", value: 0.1, min: 0, max: 1, step: 0.01 },
    { key: "gamma", label: "γ (discount)", value: 0.9, min: 0, max: 1, step: 0.01 },
    { key: "epsilonStart", label: "ε start", value: 1.0, min: 0, max: 1, step: 0.05 },
    { key: "epsilonEnd", label: "ε end", value: 0.05, min: 0, max: 1, step: 0.01 },
    { key: "epsilonDecay", label: "ε decay", value: 0.001, min: 0, max: 0.1, step: 0.0005 },
    { key: "stepsPerSecond", label: "steps/sec", value: 10, min: 1, max: 200, step: 1 },
  ];

  onParam(key: string, value: number): void {
    switch (key) {
      case "alpha": this.alpha = value; break;
      case "gamma": this.gamma = value; break;
      case "epsilonStart":
        this.epsilonStart = value;
        if (this.counter === 0) this.epsilon = value;
        break;
      case "epsilonEnd": this.epsilonEnd = value; break;
      case "epsilonDecay": this.epsilonDecay = value; break;
      case "stepsPerSecond": this.stepsPerSecond = value; break;
    }
  }

  // ε-greedy action selection with random tie-breaking on exploit.
  private pickAction(state: QTile): ActionEnum {
    let action: ActionEnum;
    if (Math.random() > this.epsilon) {
      // Exploit — shuffle first so ties break randomly (matches Unity's Shuffle().OrderBy).
      const shuffled = [...ALL_ACTIONS].sort(() => Math.random() - 0.5);
      action = shuffled.reduce((best, a) =>
        state.getQ(a) > state.getQ(best) ? a : best,
      shuffled[0]);
    } else {
      action = randomAction();
    }
    this.epsilon = Math.max(this.epsilonEnd, this.epsilon - this.epsilonDecay);
    return action;
  }

  step(): void {
    const state = this.agent.state;
    if (state.tileType !== TileEnum.Grass) {
      this.resetAgentPos();
    } else {
      // Update Q-values for ALL actions from current state (pedagogical variant).
      for (const a of ALL_ACTIONS) {
        const q = state.getQ(a);
        const sPrime = this.grid.getTargetTile(state, a);
        const r = sPrime.reward;
        let qMax = -Infinity;
        for (const ap of ALL_ACTIONS) {
          const v = sPrime.getQ(ap);
          if (v > qMax) qMax = v;
        }
        const td = r + this.gamma * qMax - q;
        state.setQ(a, q + this.alpha * td);
      }
      const chosen = this.pickAction(state);
      this.agent.state = this.grid.getTargetTile(state, chosen);
    }
    this.counter += 1;
  }

  setAuto(on: boolean): void {
    this.auto = on;
    this.autoAcc = 0;
  }

  isAuto(): boolean {
    return this.auto;
  }

  tickAuto(dt: number): void {
    if (!this.auto) return;
    const interval = this.stepsPerSecond > 0 ? 1 / this.stepsPerSecond : 0;
    this.autoAcc += dt;
    if (interval === 0) {
      this.step();
      this.autoAcc = 0;
      return;
    }
    // Cap per-frame work so slow browsers don't explode.
    let budget = 256;
    while (this.autoAcc >= interval && budget-- > 0) {
      this.step();
      this.autoAcc -= interval;
    }
  }

  private resetAgentPos(): void {
    this.agent.state = this.grid.tileAt(START_POS.x, START_POS.y);
  }

  reset(): void {
    for (const tile of this.grid.enumerate()) {
      for (const a of ALL_ACTIONS) tile.setQ(a, 0);
    }
    this.epsilon = this.epsilonStart;
    this.counter = 0;
    this.resetAgentPos();
  }

  stats(): Record<string, string> {
    return {
      step: String(this.counter),
      ε: this.epsilon.toFixed(3),
      pos: `(${this.agent.state.currentPos.x}, ${this.agent.state.currentPos.y})`,
    };
  }

  dispose(): void {
    this.grid.dispose();
  }
}
