import * as THREE from "three";
import { ActionEnum, ALL_ACTIONS, TILE_SIZE } from "./types";
import { QTile } from "./tiles/QTile";

// Agent — a robot sprite that sits on top of the grid and moves to whichever
// QTile is set as its state. ALL_ACTIONS / randomAction mirror the static
// helpers on Unity's Agent.cs.

const ROBOT_URL = "sprites/robot.png";

export function randomAction(): ActionEnum {
  return ALL_ACTIONS[Math.floor(Math.random() * ALL_ACTIONS.length)];
}

export class Agent {
  readonly mesh: THREE.Sprite;
  private _state!: QTile;

  constructor() {
    const tex = new THREE.TextureLoader().load(ROBOT_URL);
    tex.magFilter = THREE.NearestFilter;
    const mat = new THREE.SpriteMaterial({ map: tex, transparent: true });
    this.mesh = new THREE.Sprite(mat);
    this.mesh.scale.set(TILE_SIZE * 0.75, TILE_SIZE * 0.75, 1);
    this.mesh.position.z = 1;
  }

  get state(): QTile {
    return this._state;
  }

  set state(s: QTile) {
    this._state = s;
    this.mesh.position.x = s.currentPos.x * TILE_SIZE;
    this.mesh.position.y = -s.currentPos.y * TILE_SIZE;
  }
}

export { ALL_ACTIONS };
