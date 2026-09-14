# Rotatris Core Mechanics Prototype (Unity / C#)

A minimal, code-only prototype of Rotatris' core "Avalanche" mechanic: a
fixed core cell on the floor, standard tetrominoes falling one at a time
from above and locking in place like classic Tetris, and a highlighted
5x5 target zone above the core that must never have a gap sealed inside
it - doing so ends the game immediately.

No art assets, prefabs, or scene wiring are required — `GameManager.cs`
builds the camera, board, and HUD entirely at runtime from plain colored
squares and `TextMesh` labels.

## Requirements

- Unity **2021.3 LTS** or newer (2022 LTS also fine). Any render pipeline
  (Built-in, URP) works, since only `SpriteRenderer` and `TextMesh` are used.

## How to run it

1. Open Unity Hub → **New Project** → any template (2D or 3D Core is fine).
2. Copy the `Assets/Scripts` folder from this submission into your new
   project's `Assets` folder.
3. In the new project, create an empty Scene (or use the default
   `SampleScene`).
4. Create an empty GameObject in the scene (`GameObject → Create Empty`),
   name it `BOOTSTRAPER`.
5. Drag the `Bootstraper.cs` script onto that GameObject (or use
   **Add Component → Rotatris → GameManager**).
6. Press **Play**.

That's it — everything else (camera, grid, HUD, next-piece preview) is
generated in code on `Awake()`.

> A skeleton `ProjectSettings/` and `Packages/manifest.json` are also
> included in this ZIP as a convenience starting point. If Unity has any
> trouble opening that skeleton directly (project files are version/editor
> specific), just follow the 6 steps above in a fresh project instead —
> the scripts have no other dependencies.

## Controls

| Action              | Key(s)                       |
|---------------------|-------------------------------|
| Move left / right   | `A`/`D` or Arrow keys         |
| Soft drop (hold)    | `S` or Down Arrow             |
| Hard drop           | `Space`                       |
| Rotate clockwise    | `W`, `X`, or Up Arrow         |
| Rotate counter-CW   | `Z`                           |
| Hold / swap piece   | `C`                           |
| Pause / resume      | `P` or `Escape`               |
| Restart (game over) | `R`                           |

These mirror the reference site's own control scheme (confirmed via its
in-game Settings / How to Play pages).

## Troubleshooting

**Movement/rotation keys don't seem to do anything (but the piece still
falls on its own):**
1. Click into the **Game view** first — Unity only receives keyboard input
   when that window has focus, not the Scene view or Console.
2. Check the Console for a red error starting with `Rotatris: keyboard
   input is disabled...`. If you see it, go to **Edit → Project Settings →
   Player → Other Settings → Active Input Handling** and set it to
   **"Both"** (or **"Input Manager (Old)"**), then re-enter Play mode.
   Newer Unity project templates sometimes default to the new Input
   System exclusively, which silently breaks the legacy `Input` class the
   scripts use here.

## What's implemented

- Fixed core cell on the floor of a bounded play area; pieces only ever
  fall straight down and lock exactly where they land (no side-attach or
  auto-rotation - confirmed to match the reference).
- All 7 standard tetrominoes, falling one at a time at a steady pace.
- 7-bag randomized queue with a "Next 3" preview panel.
- Ghost preview showing exactly where the active piece will land.
- A highlighted 5x5 target zone directly above the core (the only cells
  on the board with a background tint - see `BoardRenderer.
  DrawTargetZoneHighlight`); everything outside it is left plain.
- Game over the instant a locked piece seals an unreachable gap inside
  that target zone (`StructureGrid.HasSealedHoleInTargetZone`), or when a
  new piece has nowhere valid to spawn.
- Score (+10 per locked piece) and a persisted Best score (`PlayerPrefs`).
- Hold/swap piece, pause, hard/soft drop, manual move & rotate.

## What's intentionally left out (per assignment scope)

Reference-matching visual theme, menus, settings panels, sound, and
animation polish — the assignment explicitly scopes these out in favor of
correct, legible core mechanics.
