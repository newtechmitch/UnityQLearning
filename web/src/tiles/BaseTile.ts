import * as THREE from "three";
import { REWARDS, TileEnum, TilePos, TILE_SIZE } from "../types";
import { TextSprite, valueColor } from "../textSprite";

// Shared tile base — mirrors Unity's BaseTile. Holds reward, position, and the
// Three.js group containing the sprite and reward label. Subclasses add their
// own value/Q-value text sprites into this group.

const textureLoader = new THREE.TextureLoader();
const textureCache = new Map<TileEnum, THREE.Texture>();

function loadTileTexture(type: TileEnum): THREE.Texture {
  const cached = textureCache.get(type);
  if (cached) return cached;
  const url =
    type === TileEnum.Water ? "sprites/water.png" :
    type === TileEnum.Award ? "sprites/award.png" :
    "sprites/grass.png";
  const tex = textureLoader.load(url);
  tex.magFilter = THREE.NearestFilter;
  tex.minFilter = THREE.LinearMipmapLinearFilter;
  textureCache.set(type, tex);
  return tex;
}

export abstract class BaseTile {
  readonly group: THREE.Group;
  readonly tileType: TileEnum;
  readonly currentPos: TilePos;
  readonly reward: number;
  protected readonly rewardLabel: TextSprite;

  constructor(type: TileEnum, pos: TilePos) {
    this.tileType = type;
    this.currentPos = pos;
    this.reward = REWARDS[type];

    this.group = new THREE.Group();
    this.group.position.set(pos.x * TILE_SIZE, -pos.y * TILE_SIZE, 0);

    // Tile background sprite.
    const tileMat = new THREE.MeshBasicMaterial({
      map: loadTileTexture(type),
      transparent: true,
    });
    const tileMesh = new THREE.Mesh(new THREE.PlaneGeometry(TILE_SIZE, TILE_SIZE), tileMat);
    this.group.add(tileMesh);

    // Reward label (top-left corner).
    this.rewardLabel = new TextSprite(this.reward.toFixed(4), {
      worldWidth: 0.45,
      worldHeight: 0.18,
      color: valueColor(this.reward),
      font: "500 40px Inter, system-ui, sans-serif",
    });
    this.rewardLabel.sprite.position.set(-TILE_SIZE * 0.28, TILE_SIZE * 0.38, 0.1);
    this.group.add(this.rewardLabel.sprite);
  }
}
