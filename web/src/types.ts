// Direct port of ActionEnum / TileEnum / TilePos from the Unity project.
// Action values are used as array indices (Left=0, Right=1, Up=2, Down=3).

export enum ActionEnum {
  Left = 0,
  Right = 1,
  Up = 2,
  Down = 3,
}

export const ALL_ACTIONS: readonly ActionEnum[] = [
  ActionEnum.Left,
  ActionEnum.Right,
  ActionEnum.Up,
  ActionEnum.Down,
];

export enum TileEnum {
  Water = -1,
  Grass = 0,
  Award = 1,
}

export interface TilePos {
  readonly x: number;
  readonly y: number;
}

export const BOARD_WIDTH = 8;
export const BOARD_HEIGHT = 5;
export const TILE_SIZE = 1; // world units; we work in a normalised grid space.

export const START_POS: TilePos = { x: 2, y: 2 };

export const REWARDS: Record<TileEnum, number> = {
  [TileEnum.Water]: -1,
  [TileEnum.Grass]: 0,
  [TileEnum.Award]: 1,
};

// Matches Unity's _map — rows are Y (top→bottom), cols are X (left→right).
export const MAP: readonly (readonly TileEnum[])[] = [
  [-1, -1, -1, -1, -1, -1, -1, -1],
  [-1,  0,  0,  0, -1,  0,  1, -1],
  [-1,  0,  0,  0, -1,  0,  0, -1],
  [-1,  0,  0,  0,  0,  0,  0, -1],
  [-1, -1, -1, -1, -1, -1, -1, -1],
] as const;
