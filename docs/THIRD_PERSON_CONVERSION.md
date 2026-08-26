# 3D Conversion Path

The original 2D game remains unchanged. The 3D work starts in a separate Unity scene so the
existing levels, browser build, data, and progression systems remain available as a reference.

## Create a 3D conversion scene

1. Open the project in **Unity 6.0.5** (the version recorded in `ProjectSettings/ProjectVersion.txt`).
2. Let Unity finish importing the project.
3. If this is a fresh clone, run **From Cell → Setup → Run Full Prototype Setup** once so the
   player tag and game data assets are available.
4. Select **From Cell → 3D Conversion → Create 3D Conversion Test Scene**.
5. Unity creates and opens:
   `Assets/_Project/Scenes/ThirdPerson/3D_Conversion_Test.unity`
6. Press **Play**.

## Play the converted Level 1

Level 1 is the first complete 3D vertical slice. It is generated separately from the original
2D scene and reads the same `Level01` blueprint for its route, wind zones, collectibles,
checkpoint, Echo Fox encounter ID, and finish location.

1. Select **From Cell → 3D Conversion → Create 3D Level 1 - First Steps**.
2. Unity creates and opens:
   `Assets/_Project/Scenes/ThirdPerson/3D_Level_01_FirstSteps.unity`
3. Press **Play**, then tap/click along the route.
4. Cross the three wind currents, collect vocabulary gems if desired, clear the Echo Fox ESL
   challenge, and reach the glowing exit.

The original Level 1 blueprint requires zero collectibles, so the five gems remain collectible
learning content rather than becoming a new completion requirement.

## Test controls

- **Tap / click ground:** navigate with NavMesh pathfinding. Generated scenes accept only the
  `Ground` layer, so taps on scenery do not issue accidental movement commands.
- **Tap NPC or vocabulary sign:** move into range, then tap again to interact
- **Drag or long press:** does not move the player. The tap/drag boundary scales with the
  device's shortest screen edge and taps have a short maximum duration, so actions remain
  consistent across Android resolutions.
- **Mouse wheel / two-finger pinch:** camera zoom. A pinch that begins on UI is ignored so quiz
  controls do not also move the camera.

The scene generator bakes a `NavMeshData` asset from the 3D environment colliders in the editor
and assigns it to the generated scene. This prevents launch-time collider scanning in converted
levels. Graybox scenes without a baked asset retain an asynchronous runtime update fallback.

### Character art

The 3D actors use the existing SVG character source files through the baked SpriteBank rather
than introducing a second art pipeline. Level 1 displays Milo Mouse, Timmy Turtle, and Echo Fox.
`ArtKeys.HeroForStage` maps all ten evolution stages to the existing hero artwork, while
`ArtKeys.VillainForEncounter` maps all six ESL villains.

The 3D scene menu automatically bakes the character art before generating its scene. **From Cell
→ Setup → Bake Character Art** remains available when updating the art on its own. The sprites
remain billboarded and crisp in the 3D scene while the actors retain 3D colliders and NavMesh
movement.

## What is included now

- Elevated/isometric camera with smooth follow and zoom
- Tap-to-move input using Unity NavMesh agents
- Evolution-data adapter for player movement speed
- Placeholder stylized 3D player and NPC actors
- NPC patrol movement with walking/idle animation hooks
- Tap-to-interact targeting for actors and objects
- Mobile-safe HUD instructions
- SVG-backed 3D character visuals for the Level 1 hero, guide, and villain gate
- A complete 3D adaptation of Level 1's authored route, wind, gems, checkpoint, Echo Fox ESL
  gate, finish, and local progress save

## Preserved systems

No existing 2D scene, web build, level blueprint, evolution data, ESL content, checkpoint,
collectible, hazard, save, or progression source was deleted or replaced. Level 1 is built in a
separate 3D scene; the next conversion step is to verify it on Android, then adapt further
levels and evolution abilities without changing the original path.


## Android development build and device constraints

The project configures Android for IL2CPP, ARM64, and auto-rotating landscape through
**From Cell → Setup → Apply Android Defaults (ARM64)**. Both left and right landscape are
supported; portrait is intentionally disabled.

To make an installable test build:

1. Create the conversion test scene from **From Cell → 3D Conversion → Create 3D Conversion
   Test Scene**.
2. Switch the active platform to Android in **File → Build Profiles**.
3. Select **From Cell → 3D Conversion → Build Android Development APK**.
4. Install `Builds/Android/FromCell3DConversion-dev.apk` on the device. The menu command builds
   only the isolated conversion test scene with Development, script debugging, and profiler
   connection enabled.
5. For the larger authored route, create **3D Level 1 - First Steps** and select
   **From Cell → 3D Conversion → Build 3D Level 1 Development APK**. It writes
   `Builds/Android/FromCell3DLevel01-dev.apk` with the same profiler-enabled settings.

The generated HUD uses `CanvasScaler` at a 1920×1080 landscape reference and applies
`Screen.safeArea`, so its text remains clear of display cutouts and gesture insets. Test both
landscape directions on cutout devices after launch, after rotation, and after background/resume.


### Current performance and save constraints

- Generated 3D scenes bake a `NavMeshData` asset during editor generation and attach that asset
  at launch, avoiding collider collection and synchronous NavMesh builds on the device. The
  runtime component retains an asynchronous update fallback for unbaked graybox scenes and
  keeps existing navigation active while it updates.
- The development build does not impose a frame-rate claim. Use the Android profiler to confirm
  stable frame pacing during a 10-minute continuous movement and pinch run on a representative
  mid-tier device. Record device model, Android version, refresh rate, thermal state, and
  median/low frame rate in `docs/PLAYTEST.md`.
- The 3D Level 1 finish writes the existing completion save (`last completed level` and
  `stage`) using `PlayerPrefs`. It does not persist an in-progress 3D position or checkpoint.
  Validate completion survives a cold restart; force-closing during a run is expected to restart
  the converted scene at its spawn.
