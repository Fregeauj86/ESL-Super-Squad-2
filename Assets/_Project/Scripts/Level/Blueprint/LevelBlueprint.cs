using System;
using FromCell.ESL;
using UnityEngine;

namespace FromCell.Level
{
    /// <summary>
    /// One level's full content as plain data - the single source of truth LevelAssembler
    /// reads at play time and (later) Tools.Validate/Tools.EmitLevels read outside Unity.
    /// spawn/finish live here too so there's one place to edit a level, even though
    /// BuildGrayboxLevelPublic still takes them as separate parameters when building the scene.
    /// </summary>
    [Serializable]
    public class LevelBlueprint
    {
        public string name;
        public int levelIndex;
        public int stageIndex;
        // Unity-unit scale, matching the existing graybox levels (original Level 1 spans
        // ~22 units, Level 10 ~34) - NOT pixels (the separate web/ build uses a pixel scale
        // for its own canvas and is unrelated). Always overridden via .WorldWidth(...) for
        // real content; this default just keeps an unauthored blueprint sane-looking.
        public float worldWidth = 24f;

        public Vector2 spawn;
        public Vector2 finish;
        public Vector2 finishSize = new Vector2(2f, 3f);

        public int requiredCollectibles;
        public string tutorialLine;

        public PlatformDef[] platforms = Array.Empty<PlatformDef>();
        public HazardDef[] hazards = Array.Empty<HazardDef>();
        public CollectibleDef[] collectibles = Array.Empty<CollectibleDef>();
        public GrowthPickupDef[] growthPickups = Array.Empty<GrowthPickupDef>();
        public CheckpointDef[] checkpoints = Array.Empty<CheckpointDef>();
        public RolePadDef[] rolePads = Array.Empty<RolePadDef>();
        public RoleGateDef[] roleGates = Array.Empty<RoleGateDef>();
        public WindZoneDef[] windZones = Array.Empty<WindZoneDef>();
        public VillainGateDef[] villainGates = Array.Empty<VillainGateDef>();

        public MovingPlatformDef[] movingPlatforms = Array.Empty<MovingPlatformDef>();
        public CrumblingPlatformDef[] crumblingPlatforms = Array.Empty<CrumblingPlatformDef>();
        public OneWayPlatformDef[] oneWayPlatforms = Array.Empty<OneWayPlatformDef>();
        public EnemyDef[] enemies = Array.Empty<EnemyDef>();
        public SecretDef[] secrets = Array.Empty<SecretDef>();
    }
}
