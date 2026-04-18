# Q-Learning & Bellman — Three.js port

TypeScript + Three.js port of the Unity demos in this repo. Same grid world,
same update rules; runs entirely in the browser — no Unity required.

## Run

```bash
cd web
npm install
npm run dev
```

Then open http://localhost:5173.

## Scenes

- **Bellman** — iterative value propagation. Each step computes `V(s) = max_a [R(s,a) + γ·V(s')]` for every grass tile.
- **Q-Learning** — ε-greedy agent. Each step updates Q-values for all four actions from the current state (matches `QLearn.cs` — pedagogical variant), then moves.

## Controls

| Key     | Action              |
|---------|---------------------|
| `Space` | Step once           |
| `A`     | Toggle auto-step    |
| `R`     | Reset current scene |

Parameters (γ, α, ε, etc.) are editable in the right-hand panel.

## Architecture

Mirrors the Unity C# classes one-for-one:

```
src/types.ts             // ActionEnum, TileEnum, TilePos, MAP
src/TileGrid.ts          // generic grid, movement rules
src/tiles/BaseTile.ts    // reward + sprite + label
src/tiles/VTile.ts       // V(s) tile (Bellman)
src/tiles/QTile.ts       // four Q-values per tile
src/Agent.ts             // robot sprite
src/scenes/BellmanScene.ts
src/scenes/QLearnScene.ts
src/main.ts              // renderer + scene switching + UI
```
