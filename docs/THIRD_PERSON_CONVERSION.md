# 3D Conversion Path

The original 2D game remains unchanged. The 3D work starts in a separate Unity scene so the
existing levels, browser build, data, and progression systems remain available as a reference.

## Create the first 3D test scene

1. Open the project in **Unity 6.0.5** (the version recorded in `ProjectSettings/ProjectVersion.txt`).
2. Let Unity finish importing the project.
3. If this is a fresh clone, run **From Cell → Setup → Run Full Prototype Setup** once so the
   player tag and game data assets are available.
4. Select **From Cell → 3D Conversion → Create 3D Conversion Test Scene**.
5. Unity creates and opens:
   `Assets/_Project/Scenes/ThirdPerson/3D_Conversion_Test.unity`
6. Press **Play**.

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

## Preserved systems

No existing 2D scene, web build, level blueprint, evolution data, ESL content, checkpoint,
collectible, hazard, save, or progression source was deleted or replaced. The next conversion
step is to adapt one existing level blueprint into this 3D scene structure, then verify that
its mechanics and completion outcome match the original before converting additional levels.

## Android note

The project already configures Android for IL2CPP, ARM64, and landscape through
**From Cell → Setup → Apply Android Defaults (ARM64)**. Build and test the conversion scene as
an Android development build after confirming Editor movement and interaction.