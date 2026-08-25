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

- **Tap / click ground:** navigate with NavMesh pathfinding
- **Tap NPC or vocabulary sign:** move into range, then tap again to interact
- **Mouse wheel / two-finger pinch:** camera zoom

The scene creates its NavMesh from the 3D environment colliders at runtime. This keeps the
conversion scene self-contained while the level-by-level conversion adapter is developed.

## What is included now

- Elevated/isometric camera with smooth follow and zoom
- Tap-to-move input using Unity NavMesh agents
- Evolution-data adapter for player movement speed
- Placeholder stylized 3D player and NPC actors
- NPC patrol movement with walking/idle animation hooks
- Tap-to-interact targeting for actors and objects
- Mobile-safe HUD instructions
- A complete 3D adaptation of Level 1's authored route, wind, gems, checkpoint, Echo Fox ESL
  gate, finish, and local progress save

## Preserved systems

No existing 2D scene, web build, level blueprint, evolution data, ESL content, checkpoint,
collectible, hazard, save, or progression source was deleted or replaced. Level 1 is built in a
separate 3D scene; the next conversion step is to verify it on Android, then adapt further
levels and evolution abilities without changing the original path.

## Android note

The project already configures Android for IL2CPP, ARM64, and landscape through
**From Cell → Setup → Apply Android Defaults (ARM64)**. Build and test the conversion scene as
an Android development build after confirming Editor movement and interaction.