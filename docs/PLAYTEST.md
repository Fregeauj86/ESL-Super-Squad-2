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

## 3D conversion — Android development build

### Install and launch
- [ ] Create `3D_Conversion_Test.unity`, switch to Android, and run
      **From Cell → 3D Conversion → Build Android Development APK**.
- [ ] Install `Builds/Android/FromCell3DConversion-dev.apk` on a physical device.
- [ ] Cold launch reaches the conversion scene without an error or a prolonged navigation-build
      hitch.
- [ ] Create `3D_Level_01_FirstSteps.unity`, then run **From Cell → 3D Conversion → Build 3D
      Level 1 Development APK**. Use `Builds/Android/FromCell3DLevel01-dev.apk` for the
      representative larger-scene performance capture.
- [ ] Background and resume while idle, while moving, and during a two-finger gesture. No delayed
      movement command or stuck camera input occurs.

### Controls and UI
- [ ] **Tap:** one tap on each open ground area moves the player to the intended NavMesh point.
- [ ] **Drag:** a deliberate one-finger drag never becomes a move command.
- [ ] **Interaction:** tap the NPC and vocabulary sign from out of range, wait for the approach,
      then tap again in range. Confirm the prompt and action are predictable.
- [ ] **Pinch:** pinch in and out reaches both zoom limits without jumps or jitter.
- [ ] **Gesture transition:** start a touch, add a second finger, then release. No destination is
      set after the pinch.
- [ ] **UI ownership:** operate an ESL overlay control with two fingers. The camera does not zoom
      from a pinch that began on the overlay.
- [ ] **Safe area and scale:** title, instruction, prompt, and Level 1 status text remain visible
      and readable at 16:9, 19.5:9, and 20:9 landscape aspect ratios, including left and right
      cutout orientations.

### Orientation, performance, and save
- [ ] Portrait remains unavailable; rotation between both landscape directions preserves control
      behavior and safe-area placement.
- [ ] Record the test device, Android version, refresh rate, and whether it has a display cutout.
- [ ] Profile a 10-minute movement, interaction, and pinch session on a representative mid-tier
      device. Record median FPS, lowest sustained FPS, frame-time spikes, peak memory, and
      thermal condition below.
- [ ] In 3D Level 1, clear Echo Fox and reach the exit. Cold restart confirms the
      completion-only PlayerPrefs save remains. Force-closing before the exit is expected to
      restart the scene at spawn because in-progress checkpoints are not persisted yet.

| Device / Android | Aspect / refresh | Launch & rotation | Median / low FPS | Peak memory | Thermal state | Notes / fixes |
|---|---|---|---|---|---|---|
| _Record physical-device result_ | _e.g. 20:9 / 60 Hz_ | _pass / issue_ | _record_ | _record_ | _record_ | _constraint or fix_ |

### Performance measurement protocol

Record this section from a **Development** Android build with **Autoconnect Profiler** enabled.
The 3D runtime labels the main NavMesh phases as `FromCell.NavMesh.CollectSources`,
`FromCell.NavMesh.StartAsyncUpdate`, and (for comparison-only builds)
`FromCell.NavMesh.BuildSync`.

1. **Cold launch:** force-stop the app, clear it from recents, launch it three times, and record
   the time from tapping the icon to the first frame where the player accepts a tap. Report the
   median and worst launch time. Confirm the frame remains responsive while the asynchronous
   NavMesh finishes.
2. **Frame pacing:** profile a ten-minute route containing movement, wind, collectibles, the ESL
   overlay, and pinch zoom. Record median FPS, lowest sustained FPS, 95th-percentile frame time,
   and the count of frame-time spikes above 33.3 ms.
3. **Memory:** record the peak total, reserved, and graphics memory shown by the Unity Profiler
   during the same route. Repeat after returning to the menu to catch scene-lifetime leaks.
4. **Thermal behavior:** record the device temperature/thermal status at the start and end of the
   ten-minute route, noting whether the OS reduced refresh rate or performance.
5. **Comparison:** repeat the launch and route capture on the compact conversion scene and one
   larger converted scene. The larger scene should not use `BuildSync` during startup.

No physical Android profiler result is fabricated here; Unity and an Android device are not
available in the Replit workspace. Fill the table above with the actual representative
mid-tier device, OS, refresh rate, memory, and thermal readings before treating this milestone
as device-verified.

| Device / Android / refresh | Cold launch median / worst | Median FPS / p95 frame time | Frames >33.3 ms | Peak total / reserved / graphics memory | Thermal start → end | Result / follow-up |
|---|---|---|---|---|---|---|
| _Record physical-device result_ | _seconds / seconds_ | _FPS / ms_ | _count_ | _MB / MB / MB_ | _e.g. nominal → warm_ | _pass, issue, or planned fix_ |
