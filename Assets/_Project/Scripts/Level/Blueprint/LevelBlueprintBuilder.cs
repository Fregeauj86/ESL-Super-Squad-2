using System.Collections.Generic;
using FromCell.ESL;
using UnityEngine;

namespace FromCell.Level
{
    /// <summary>
    /// Fluent authoring helper for LevelBlueprint - makes the per-level content files
    /// (Phase 6) read as a sequence of beats instead of raw array initializers. Every method
    /// returns `this` and Build() assembles the final immutable-by-convention LevelBlueprint.
    /// </summary>
    public class LevelBlueprintBuilder
    {
        readonly LevelBlueprint bp;
        readonly List<PlatformDef> platforms = new List<PlatformDef>();
        readonly List<HazardDef> hazards = new List<HazardDef>();
        readonly List<CollectibleDef> collectibles = new List<CollectibleDef>();
        readonly List<GrowthPickupDef> growthPickups = new List<GrowthPickupDef>();
        readonly List<CheckpointDef> checkpoints = new List<CheckpointDef>();
        readonly List<RolePadDef> rolePads = new List<RolePadDef>();
        readonly List<RoleGateDef> roleGates = new List<RoleGateDef>();
        readonly List<WindZoneDef> windZones = new List<WindZoneDef>();
        readonly List<VillainGateDef> villainGates = new List<VillainGateDef>();
        readonly List<MovingPlatformDef> movingPlatforms = new List<MovingPlatformDef>();
        readonly List<CrumblingPlatformDef> crumblingPlatforms = new List<CrumblingPlatformDef>();
        readonly List<OneWayPlatformDef> oneWayPlatforms = new List<OneWayPlatformDef>();
        readonly List<EnemyDef> enemies = new List<EnemyDef>();
        readonly List<SecretDef> secrets = new List<SecretDef>();

        LevelBlueprintBuilder(string name, int levelIndex, int stageIndex)
        {
            bp = new LevelBlueprint { name = name, levelIndex = levelIndex, stageIndex = stageIndex };
        }

        public static LevelBlueprintBuilder Create(string name, int levelIndex, int stageIndex) =>
            new LevelBlueprintBuilder(name, levelIndex, stageIndex);

        public LevelBlueprintBuilder WorldWidth(float width) { bp.worldWidth = width; return this; }
        public LevelBlueprintBuilder Spawn(Vector2 position) { bp.spawn = position; return this; }

        public LevelBlueprintBuilder Finish(Vector2 position, Vector2? size = null)
        {
            bp.finish = position;
            if (size.HasValue) bp.finishSize = size.Value;
            return this;
        }

        public LevelBlueprintBuilder Tutorial(string text) { bp.tutorialLine = text; return this; }
        public LevelBlueprintBuilder RequireCollectibles(int count) { bp.requiredCollectibles = count; return this; }

        public LevelBlueprintBuilder Ground(string name, Vector2 position, Vector2 size, Color? color = null)
        {
            var def = new PlatformDef(name, position, size);
            if (color.HasValue) def.color = color.Value;
            platforms.Add(def);
            return this;
        }

        public LevelBlueprintBuilder RoleGate(string name, Vector2 position, PlayerRoleState.SquadRole requiredRole)
        {
            roleGates.Add(new RoleGateDef(name, position, requiredRole));
            return this;
        }

        public LevelBlueprintBuilder Hazard(string name, Vector2 position, Vector2 size)
        {
            hazards.Add(new HazardDef(name, position, size));
            return this;
        }

        public LevelBlueprintBuilder Collectible(string name, Vector2 position)
        {
            collectibles.Add(new CollectibleDef(name, position));
            return this;
        }

        public LevelBlueprintBuilder Growth(string name, Vector2 position)
        {
            growthPickups.Add(new GrowthPickupDef(name, position));
            return this;
        }

        public LevelBlueprintBuilder Checkpoint(string name, Vector2 position)
        {
            checkpoints.Add(new CheckpointDef(name, position));
            return this;
        }

        public LevelBlueprintBuilder RolePad(string name, Vector2 position, PlayerRoleState.SquadRole role)
        {
            rolePads.Add(new RolePadDef(name, position, role));
            return this;
        }

        public LevelBlueprintBuilder Wind(string name, Vector2 position, Vector2 size, Vector2 force)
        {
            windZones.Add(new WindZoneDef(name, position, size, force));
            return this;
        }

        public LevelBlueprintBuilder VillainGate(string encounterId, Vector2 position, Vector2? size = null)
        {
            var def = new VillainGateDef { encounterId = encounterId, position = position };
            if (size.HasValue) def.gateSize = size.Value;
            villainGates.Add(def);
            return this;
        }

        public LevelBlueprintBuilder MovingPlatform(
            string name, Vector2[] waypoints, Vector2? size = null, float speed = 2f,
            Platforms.CarryMode carryMode = Platforms.CarryMode.PositionDelta, bool loop = false)
        {
            var def = new MovingPlatformDef(name, waypoints, size ?? new Vector2(2f, 0.5f), speed)
            {
                carryMode = carryMode,
                loop = loop,
            };
            movingPlatforms.Add(def);
            return this;
        }

        public LevelBlueprintBuilder CrumblingPlatform(string name, Vector2 position, Vector2? size = null)
        {
            crumblingPlatforms.Add(new CrumblingPlatformDef(name, position, size ?? new Vector2(2f, 0.5f)));
            return this;
        }

        public LevelBlueprintBuilder OneWayPlatform(string name, Vector2 position, Vector2? size = null)
        {
            oneWayPlatforms.Add(new OneWayPlatformDef(name, position, size ?? new Vector2(2f, 0.5f)));
            return this;
        }

        public LevelBlueprintBuilder Enemy(string name, Vector2 position, EnemyKind kind, float speed, float range, Vector2? secondary = null)
        {
            var def = new EnemyDef(name, position, kind, speed, range);
            if (secondary.HasValue) def.secondary = secondary.Value;
            enemies.Add(def);
            return this;
        }

        public LevelBlueprintBuilder Secret(string name, Vector2 position)
        {
            secrets.Add(new SecretDef(name, position));
            return this;
        }

        public LevelBlueprint Build()
        {
            bp.platforms = platforms.ToArray();
            bp.hazards = hazards.ToArray();
            bp.collectibles = collectibles.ToArray();
            bp.growthPickups = growthPickups.ToArray();
            bp.checkpoints = checkpoints.ToArray();
            bp.rolePads = rolePads.ToArray();
            bp.roleGates = roleGates.ToArray();
            bp.windZones = windZones.ToArray();
            bp.villainGates = villainGates.ToArray();
            bp.movingPlatforms = movingPlatforms.ToArray();
            bp.crumblingPlatforms = crumblingPlatforms.ToArray();
            bp.oneWayPlatforms = oneWayPlatforms.ToArray();
            bp.enemies = enemies.ToArray();
            bp.secrets = secrets.ToArray();
            return bp;
        }
    }
}
