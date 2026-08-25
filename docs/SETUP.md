# Unity Setup Guide

## 1. Open project

1. Unity Hub → **Add** → `j:\Random Apps\From Cell`
2. Open with Unity 6 (6000.x) or 2022.3 LTS+

If Unity regenerates `ProjectSettings`, that is expected on first open.

## 2. One-click prototype (recommended)

Menu: **From Cell → Setup → Run Full Prototype Setup**

Creates everything below in one pass:
- Tags, layers, Android ARM64 defaults
- `GameConfig` + all evolution/level data
- Player + Mobile UI + GameSystems prefabs
- All **10 level scenes** + **Boot**, **Main Menu**, **Credits**
- Full Build Settings (Boot → Menu → Levels 1–10 → Credits)

## 3. Play the prototype

1. Open `Assets/_Project/Scenes/Boot/_Boot.unity` (or `_MainMenu.unity`)
2. Press **Play** → click **NEW GAME**
3. Play through levels; each finish zone triggers evolution → next level
4. After Level 10 → Credits → Main Menu

To test one level only: open `Level_01_Cell.unity` directly and press Play.

Editor: **A/D** still works via keyboard fallback. On device, use on-screen joystick.

## 4. Individual setup menus (optional)

| Menu | Use when |
|------|----------|
| Create Default Game Data | Regenerate ScriptableObjects only |
| Create Player Prefab | After changing player scripts |
| Create Mobile UI Prefab | After changing UI scripts |
| Create Graybox Level 01 / 02 | Rebuild a single level scene |
| Configure Build Settings | Re-register scenes after manual edits |

## 5. Tags

Edit → Project Settings → Tags and Layers:
- Tags: `Player`, `Ground`

## 6. Build settings

Add scenes in order:
1. `_Boot` (create when ready)
2. `_MainMenu`
3. `Level_01_Cell` … `Level_10_Adult`
4. `_Credits`

For now, open `Level_01_Cell` directly and press Play.

## 7. Android build

1. File → Build Settings → Android → Switch Platform
2. Player Settings → IL2CPP, ARM64
3. Minimum API per Play Store requirements
4. Build AAB
