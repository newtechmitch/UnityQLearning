import * as THREE from "three";

// Small helper: render text into a canvas and return a Three.js Sprite so it
// faces the camera and scales in world units. We redraw the canvas in-place
// rather than recreating the sprite, which keeps memory stable during the
// per-frame Q-value updates.

export interface TextSpriteOptions {
  widthPx?: number;
  heightPx?: number;
  font?: string;
  color?: string;
  align?: CanvasTextAlign;
  baseline?: CanvasTextBaseline;
  worldWidth: number; // size in grid units
  worldHeight: number;
}

export class TextSprite {
  readonly sprite: THREE.Sprite;
  private readonly ctx: CanvasRenderingContext2D;
  private readonly texture: THREE.CanvasTexture;
  private readonly opts: Required<TextSpriteOptions>;

  constructor(text: string, opts: TextSpriteOptions) {
    this.opts = {
      widthPx: 256,
      heightPx: 96,
      font: "600 48px Inter, system-ui, sans-serif",
      color: "#ffffff",
      align: "center",
      baseline: "middle",
      ...opts,
    };
    const canvas = document.createElement("canvas");
    canvas.width = this.opts.widthPx;
    canvas.height = this.opts.heightPx;
    const ctx = canvas.getContext("2d");
    if (!ctx) throw new Error("2d context unavailable");
    this.ctx = ctx;

    this.texture = new THREE.CanvasTexture(canvas);
    this.texture.minFilter = THREE.LinearFilter;
    this.texture.magFilter = THREE.LinearFilter;
    this.texture.generateMipmaps = false;

    const material = new THREE.SpriteMaterial({
      map: this.texture,
      transparent: true,
      depthWrite: false,
    });
    this.sprite = new THREE.Sprite(material);
    this.sprite.scale.set(this.opts.worldWidth, this.opts.worldHeight, 1);

    this.draw(text, this.opts.color);
  }

  draw(text: string, color: string = this.opts.color): void {
    const { ctx } = this;
    const { widthPx, heightPx, font, align, baseline } = this.opts;
    ctx.clearRect(0, 0, widthPx, heightPx);
    ctx.font = font;
    ctx.textAlign = align;
    ctx.textBaseline = baseline;
    ctx.fillStyle = color;
    const x = align === "left" ? 8 : align === "right" ? widthPx - 8 : widthPx / 2;
    const y = baseline === "top" ? 8 : baseline === "bottom" ? heightPx - 8 : heightPx / 2;
    ctx.fillText(text, x, y);
    this.texture.needsUpdate = true;
  }

  dispose(): void {
    this.texture.dispose();
    (this.sprite.material as THREE.SpriteMaterial).dispose();
  }
}

// Colour helper mirroring Unity's BaseTile.TextColor — interpolates white→green
// for positive values and white→red for negative, clamped at |val| = 1.
export function valueColor(v: number): string {
  const clamped = Math.min(1, Math.abs(v));
  if (v >= 0) {
    const r = Math.round(255 * (1 - clamped));
    const g = 255;
    const b = Math.round(255 * (1 - clamped));
    return `rgb(${r},${g},${b})`;
  }
  const r = 255;
  const g = Math.round(255 * (1 - clamped));
  const b = Math.round(255 * (1 - clamped));
  return `rgb(${r},${g},${b})`;
}
