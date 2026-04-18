import * as THREE from "three";

// Shared contract so main.ts can host either demo behind the same lifecycle.

export interface DemoScene {
  readonly root: THREE.Object3D;
  readonly params: ParamDescriptor[];
  readonly title: string;
  step(): void;
  setAuto(on: boolean): void;
  isAuto(): boolean;
  reset(): void;
  dispose(): void;
  tickAuto(dt: number): void;
  stats(): Record<string, string>;
  onParam(key: string, value: number): void;
}

export interface ParamDescriptor {
  key: string;
  label: string;
  value: number;
  min?: number;
  max?: number;
  step?: number;
}
