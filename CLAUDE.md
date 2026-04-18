# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

Educational reinforcement-learning demo accompanying the lecture *NAIL133 – Agent-based Learning* (Charles University, Dr. Adam Streck). Two interactive Unity scenes visualize the Bellman equation and tabular Q-Learning on an 8×5 grid world.

- Unity version: **6000.4.0b11** (Unity 6 beta; see `ProjectSettings/ProjectVersion.txt`)
- No build/test/lint tooling — open in Unity Editor and enter Play mode. The project has no automated test suite and no CI.
- Scenes live in `Assets/Scenes/`: `Bellman.unity` and `QLearn.unity`. Each scene is driven by a single top-level MonoBehaviour (`Bellman.cs` / `QLearn.cs`).
- In Play mode: **Space** = advance one step, **A** = toggle automatic stepping, **S** = screenshot (`ScreenShot.cs`).

## Architecture

The codebase is small (all scripts in `Assets/Scripts/`), but there are a few non-obvious design choices worth knowing before editing:

### Grid model
- `TileGrid` owns a hard-coded `_map[,]` int array where `-1 = Water`, `0 = Grass`, `1 = Award`. Changing board layout/size requires editing both `_map` and the `BOARD_WIDTH`/`BOARD_HEIGHT` constants.
- `GetTargetPos` in `TileGrid.cs` enforces the rule that **only grass tiles can transition**. Water and award act as absorbing/sink states — the agent is expected to be externally reset after landing on one.
- `TileGrid.GenerateTiles()` must be called from the scene driver's `Start()` before anything else uses the grid.

### Tile polymorphism
- `BaseTile` holds reward + position. Two subclasses exist because the two demos need different per-tile state:
  - `VTile` — single `V(s)` value, used by `Bellman`.
  - `QTile` — array of four Q-values (one per `ActionEnum`), used by `QLearn`.
- **Scenes use different tile prefabs** — don't assume a scene has both. The `TileGrid` component referenced in each scene is wired to prefabs of the appropriate subclass.
- `Agent.State` is typed as `QTile` — the `Agent` component is Q-Learning-specific.

### Bellman update ordering
`Bellman.CalculateValues()` writes into `VTile.NextValue`, then `Bellman.Step()` copies `NextValue → Value` for every tile. This two-phase update is **load-bearing**: it prevents in-place contamination within a single sweep (a tile must be updated from its neighbours' *previous* values, not partially-updated ones). Preserve this pattern if modifying the Bellman loop.

### Q-Learning update ordering
`QLearn.Step()` updates Q-values for **all four actions from the current state** before the agent moves (see `QLearn.cs:86-94`). This differs from textbook Q-learning, which updates only the taken action. It's intentional for pedagogical visualization — don't "fix" it to single-action updates without checking with the author.

Action selection happens *after* the Q-update, via ε-greedy in `PickAction`. `ε` is decayed once per call to `PickAction` (i.e., per step taken, not per Q-update). Tie-breaking uses `Extensions.Shuffle()` before `OrderBy` so equal Q-values don't bias direction.

### Reset behaviour
When the agent lands on water/award, the next `Step()` call detects `TileType != Grass` and calls `ResetAgentPos()` back to `(START_X, START_Y) = (2, 2)`. The Q-values from the terminal transition are already baked in by the previous step; the reset itself performs no learning.

## Conventions

- Action indexing is fixed by `ActionEnum` (`Left=0, Right=1, Up=2, Down=3`) and is used as an array index into `QTile._qValues`. Do not reorder the enum.
- Rewards are authored as `float` on `TileGrid` serialized fields but stored/used as `double` everywhere downstream.
- Text colouring via `BaseTile.TextColor` interpolates white→green for positive and white→red for negative values, clamped at ±1. If you add new displayed numbers, reuse this helper for consistency.
