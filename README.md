# From Cell: Evolution Run

Android 2D evolution platformer. Begin as a single cell and evolve through 10 biological stages into an adult human.

## Quick Start

> **Play now (no Unity):** [http://localhost:8080](http://localhost:8080) or open [`web/index.html`](web/index.html) — see [web/README.md](web/README.md).  
> Unity install help: [docs/UNITY_INSTALL.md](docs/UNITY_INSTALL.md)

1. Install **Unity Hub** + **2022.3 LTS** (see install guide above).
2. Unity Hub → **Add** → select this folder (`From Cell`).
3. In Unity: **From Cell → Setup → Run Full Prototype Setup** (one-click).
4. Open `Assets/_Project/Scenes/Boot/_Boot.unity` or `_MainMenu.unity` → **Play**.
5. **New Game** from menu, or open `Level_01_Cell` directly to test a single level.

### 3D conversion scenes

The original 2D game remains intact while the 3D mobile conversion is developed in an isolated
scene. In Unity, choose **From Cell → 3D Conversion → Create 3D Conversion Test Scene**, then
press Play. See [docs/THIRD_PERSON_CONVERSION.md](docs/THIRD_PERSON_CONVERSION.md).

The first authored level is also available as an isolated 3D scene. Choose
**From Cell → 3D Conversion → Create 3D Level 1 - First Steps** to create
`Assets/_Project/Scenes/ThirdPerson/3D_Level_01_FirstSteps.unity`.

### Setup menu items

| Menu | What it does |
|------|----------------|
| **Run Full Prototype Setup** | Everything: all 10 levels, Boot, Menu, Credits, ARM64 defaults |
| **Create All Graybox Levels (1-10)** | Regenerate level scenes only |
| **Create Boot + Main Menu + Credits** | Flow scenes + full Build Settings |
| **Apply Android Defaults (ARM64)** | IL2CPP + ARM64 + landscape |
| Create Player Prefab | `Prefabs/Player/Player.prefab` |
| Create Mobile UI Prefab | Joystick, Jump, Dash, evolution overlay |
| Create Graybox Level 01 / 02 | Playable test scenes |
| Configure Build Settings | Registers Level 01 + 02 for scene transitions |

## Your Original Scripts (integrated)

These untitled editor buffers were merged into the project:

| Original | Project location |
|----------|------------------|
| `Joystick` | `Scripts/Input/Joystick.cs` |
| `PlayerController` (keyboard + joystick) | `Scripts/Player/PlayerController.cs` |
| `JumpButton` | `Scripts/Input/JumpButton.cs` |
| `AbilityManager` | `Scripts/Abilities/AbilityManager.cs` |
| `EvolutionManager` | `Scripts/Evolution/EvolutionSystem.cs` + `EvolutionStageData` |
| `GameManager` | `Scripts/Core/GameFlowSystem.cs` |
| `LevelComplete` | `Scripts/Level/FinishZone.cs` |

## Documentation

Full architecture: [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)

## Scene Setup (manual in Editor)

### Player prefab
- `Rigidbody2D` (Freeze Rotation Z, Interpolate)
- `CapsuleCollider2D`
- Tag: `Player`
- Components: `PlayerController`, `PlayerMovement`, `GroundChecker`, `AbilityManager`, `PlayerHealth`, `PlayerVisual`
- Child: Sprite (placeholder square)

### Mobile UI Canvas
- `TouchInputManager` on Canvas root
- Left: Joystick (background + handle RectTransforms)
- Right: Jump button → `JumpButton.OnJumpPressed()`
- Right: Dash button (hidden until Teen) → `DashButton.OnDashPressed()`

### Level template
- `LevelBootstrap` (references GameConfig)
- `PlayerSpawn` empty at start
- `FinishZone` trigger at end (tagged trigger collider)
- Tilemaps / platforms tagged `Ground`
- `KillZone` hazards optional

### Build order
`_Boot` → `_MainMenu` → `Level_01_Cell` … `Level_10_Adult` → `_Credits`

## Layers & Tags

**Tags:** `Player`, `Ground`

**Layers:** `Player`, `Ground`, `Hazard`, `Trigger`

## Development status

- [x] Core scripts (movement, input, evolution, abilities, flow)
- [x] ScriptableObject data layer + editor generator
- [x] Player prefab + Mobile UI prefab (editor-generated)
- [x] Graybox Levels 1–10 (unique mechanics per stage)
- [x] Boot, Main Menu, Credits scenes
- [x] Role-switch gates (L3), growth pickups (L5), camera follow
- [x] Full Build Settings + Android ARM64 defaults (editor menu)
- [x] Pause menu (resume / restart / quit)
- [x] Gameplay HUD + tutorial banners + dash cooldown UI
- [x] Level completion validation (collectibles, growth orbs, exit pulse)
- [x] Browser playable build (`web/`) — all 11 levels, touch + keyboard
- [x] Wind zones, player facing, audio manager scaffold
- [x] Isolated 3D conversion test scene — isometric camera, NavMesh tap-to-move, NPC patrols,
      object interaction, and mobile-safe HUD
- [x] 3D Level 1 conversion — authored wind route, vocabulary gems, checkpoint, Echo Fox ESL
       gate, completion state, and progress save without changing the original 2D level
- [ ] Art assets (placeholder sprites only)
- [ ] Audio clips (manager ready)
- [ ] Play Store build / signing

See [docs/PLAYTEST.md](docs/PLAYTEST.md) for a full test checklist.

### Source validation

Run `python3 tools/validate_unity_source.py` before committing Unity C# changes. GitHub Actions
also checks for deprecated Unity object lookup APIs automatically.
