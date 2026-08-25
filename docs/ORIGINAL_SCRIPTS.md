# Original Untitled Scripts (Source Reference)

All 9 unsaved editor buffers are saved on disk:

- **Archives (exact copies):** `docs/original-scripts/Untitled-1` … `Untitled-9`
- **Live Unity scripts:** `Assets/_Project/Scripts/` (see mapping table in `docs/original-scripts/README.md`)

These scripts were recovered from Cursor editor backups and integrated into the project.

## Untitled-9 → `Joystick.cs`

Virtual joystick implementing `IDragHandler`, `IPointerUpHandler`, `IPointerDownHandler`.

## Untitled-8 → `PlayerController.cs` (joystick variant)

Mobile `PlayerController` reading `joystick.Horizontal` and exposing `Jump()` for UI.

## Untitled-1 → `PlayerController.cs` (keyboard variant)

Editor testing variant using `Input.GetAxisRaw` and `Input.GetButtonDown("Jump")`.

**Merged into:** `PlayerController` + `PlayerMovement` + `TouchInputManager` (keyboard fallback in Editor).

## Untitled-7 → `JumpButton.cs`

UI hook calling `player.Jump()` → now calls `TouchInputManager.RegisterJump()`.

## Untitled-6 → `AbilityManager.cs`

Double jump (`Input.GetButtonDown("Jump")`) and dash (`KeyCode.LeftShift`).

**Updated:** Uses `TouchInputManager` for mobile; keyboard fallback retained for Editor.

## Untitled-5 → `EvolutionSystem.cs` (primary)

`EvolutionManager` with `Stage` enum, `AbilityManager` toggles, switch-based stats.

## Untitled-2 → `EvolutionSystem.cs` (fallback)

Earlier `EvolutionManager` without abilities — values preserved in `ApplyFallback()`.

## Untitled-3 → `GameFlowSystem.cs`

`GameManager.CompleteLevel()` → evolve → `LoadScene(buildIndex + 1)` after 1.5s.

## Untitled-4 → `FinishZone.cs`

`LevelComplete` trigger calling `gameManager.CompleteLevel()`.
