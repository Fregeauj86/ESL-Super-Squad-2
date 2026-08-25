# Playtest Checklist

## Setup
- [ ] **From Cell → Setup → Run Full Prototype Setup**
- [ ] Open `_Boot.unity` or `_MainMenu.unity`
- [ ] Press Play

## Main Menu
- [ ] **NEW GAME** loads Level 1
- [ ] **CONTINUE** hidden when no save exists
- [ ] **CONTINUE** appears after completing at least one level

## Level 1 — Cell
- [ ] Float movement works (joystick / A-D)
- [ ] Jump button does nothing (by design)
- [ ] Wind current pushes player gently
- [ ] Tutorial banner appears
- [ ] HUD shows stage + level name
- [ ] Finish zone triggers evolution overlay → Level 2

## Level 2 — Cluster
- [ ] Collect 3 yellow division points
- [ ] Finish zone blocked until 3 collected
- [ ] Pit hazard respawns at checkpoint
- [ ] Pause menu works (II button / Escape)

## Pause Menu
- [ ] Resume returns to gameplay
- [ ] Restart reloads current level
- [ ] Main Menu returns to menu

## Levels 3–10 (smoke test)
- [ ] L3: Nerve pad opens purple gate
- [ ] L4: Jump reaches platforms
- [ ] L5: Pink growth pickups boost stats
- [ ] L8: Double jump reaches high ledge
- [ ] L9: Dash button visible + cooldown bar
- [ ] L10: Complete → Credits scene

## Mobile (when built)
- [ ] Joystick responsive
- [ ] Jump / Dash buttons work
- [ ] Landscape orientation
- [ ] 60 FPS on mid-tier device
