# Source Characters — imported from ESL Super Squad

Reused (not modified) from `J:\ESL Website Project\esl-super-squad-v2\src\` — a separate,
existing ESL web project. That project's source was only **read**, never edited; everything
below is a copy.

## What's here

- **`Squad/*.svg`** (9) — hero characters, extracted at their default idle pose (evolution
  stage 2, emotion idle, not talking/walking): MiloMouse, TimmyTurtle, DashCheetah, SkyEagle,
  MaxElephant, FinnWhale, BigTick, KingLeo, DrImperfecto. Valid, standalone SVG files (each
  verified to parse as well-formed XML) — colors are on a scoped `<style>` block using the
  same class names as the source, geometry values are resolved from the source's default props.
- **`Villains/*.svg`** (6) — antagonist characters, same idle-pose extraction: BuilderBear,
  ConnectorSnake, DebateHawk, EchoFox, QuestionOwl, TheMimic.
- **`_ReferenceSource/`** — the original TSX/CSS source files, copied verbatim (unmodified),
  kept as reference because the flattened SVGs above lose information the source has:
  - `movable/*V2.tsx` — the real parametric squad character components. Each one scales its
    geometry by `stage` (1/2/3) and swaps its mouth/eyebrows by `emotion`
    (happy/sad/angry/scared/idle/excited) — the numbers for every variant are in these files.
  - `movable/characterStyles.css` — every character's color palette plus its CSS `@keyframes`
    animations (idle float, walk cycle, talk mouth-flap, eye blink, hit knockback, attack
    lunge, per-character extras like tail-wag/wing-flap/trunk-sway).
  - `progression/*Char.tsx` — the real villain SVG source (simpler, colors inline).
  - `squad/*.tsx`, `villains/*.tsx` — the thin wrapper components showing each character's
    actual prop defaults (used to resolve the idle-pose SVGs above).

## Status

Assets only — nothing in `Assets/_Project/Scripts/` references these yet. Unity 2022.3
(this project's version, confirmed via `ProjectSettings/ProjectVersion.txt`) cannot import
raw `.svg` natively without the official `com.unity.vectorgraphics` package, which is not
currently in `Packages/manifest.json`. Turning these into real in-game sprites/animations is
future work (per the approved plan's Phase 2/3 — either rasterize to PNG through the same
baking pipeline planned there, or add the vector-graphics package) and hasn't been done yet.

## Provenance

Read-only source: `J:\ESL Website Project\esl-super-squad-v2\src\characters\` and
`src\components\characters\`. Confirmed via `git status` in that repo that these files were
not modified by this copy operation.
