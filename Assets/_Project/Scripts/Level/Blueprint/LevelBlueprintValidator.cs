using System.Collections.Generic;
using System.Linq;
using FromCell.ESL;
using UnityEngine;

namespace FromCell.Level
{
    /// <summary>
    /// Pure data validation for LevelBlueprint - no MonoBehaviour/GameObject/scene access, so
    /// this runs under plain `dotnet` (Tools.Validate), same as EslContentValidator. Checks
    /// what can be proven from the numbers alone; does not simulate physics.
    /// </summary>
    public static class LevelBlueprintValidator
    {
        const float BoundsMargin = 50f;

        public static (List<string> errors, List<string> warnings) Validate(LevelBlueprint bp, VillainEncounter[] eslCatalog = null)
        {
            var errors = new List<string>();
            var warnings = new List<string>();

            if (bp == null)
            {
                errors.Add("Blueprint is null.");
                return (errors, warnings);
            }

            string tag = string.IsNullOrEmpty(bp.name) ? $"Level[{bp.levelIndex}]" : bp.name;

            if (bp.worldWidth <= 0f)
                errors.Add($"[{tag}] worldWidth must be > 0.");

            CheckInBounds(tag, "spawn", bp.spawn, bp.worldWidth, errors);
            CheckInBounds(tag, "finish", bp.finish, bp.worldWidth, errors);

            if (bp.requiredCollectibles > (bp.collectibles?.Length ?? 0))
                errors.Add($"[{tag}] requiredCollectibles ({bp.requiredCollectibles}) exceeds the number of collectibles placed ({bp.collectibles?.Length ?? 0}).");

            foreach (var p in bp.platforms ?? System.Array.Empty<PlatformDef>())
                CheckInBounds(tag, $"platform '{p.name}'", p.position, bp.worldWidth, errors, allowNegativeY: true);
            foreach (var h in bp.hazards ?? System.Array.Empty<HazardDef>())
                CheckInBounds(tag, $"hazard '{h.name}'", h.position, bp.worldWidth, errors, allowNegativeY: true);
            foreach (var c in bp.collectibles ?? System.Array.Empty<CollectibleDef>())
                CheckInBounds(tag, $"collectible '{c.name}'", c.position, bp.worldWidth, errors, allowNegativeY: true);
            foreach (var g in bp.growthPickups ?? System.Array.Empty<GrowthPickupDef>())
                CheckInBounds(tag, $"growth pickup '{g.name}'", g.position, bp.worldWidth, errors, allowNegativeY: true);
            foreach (var cp in bp.checkpoints ?? System.Array.Empty<CheckpointDef>())
                CheckInBounds(tag, $"checkpoint '{cp.name}'", cp.position, bp.worldWidth, errors, allowNegativeY: true);
            foreach (var vg in bp.villainGates ?? System.Array.Empty<VillainGateDef>())
                CheckInBounds(tag, $"villain gate '{vg.encounterId}'", vg.position, bp.worldWidth, errors, allowNegativeY: true);

            foreach (var mp in bp.movingPlatforms ?? System.Array.Empty<MovingPlatformDef>())
            {
                if (mp.waypoints == null || mp.waypoints.Length < 2)
                    errors.Add($"[{tag}] moving platform '{mp.name}' needs at least 2 waypoints to move (has {mp.waypoints?.Length ?? 0}).");
                else
                    foreach (var wp in mp.waypoints)
                        CheckInBounds(tag, $"moving platform '{mp.name}' waypoint", wp, bp.worldWidth, errors, allowNegativeY: true);

                if (mp.speed <= 0f)
                    errors.Add($"[{tag}] moving platform '{mp.name}' speed must be > 0.");
            }

            foreach (var cp in bp.crumblingPlatforms ?? System.Array.Empty<CrumblingPlatformDef>())
                CheckInBounds(tag, $"crumbling platform '{cp.name}'", cp.position, bp.worldWidth, errors, allowNegativeY: true);

            foreach (var op in bp.oneWayPlatforms ?? System.Array.Empty<OneWayPlatformDef>())
                CheckInBounds(tag, $"one-way platform '{op.name}'", op.position, bp.worldWidth, errors, allowNegativeY: true);

            foreach (var e in bp.enemies ?? System.Array.Empty<EnemyDef>())
            {
                CheckInBounds(tag, $"enemy '{e.name}'", e.position, bp.worldWidth, errors, allowNegativeY: true);
                if (e.speed <= 0f)
                    errors.Add($"[{tag}] enemy '{e.name}' speed must be > 0.");
                if (e.range <= 0f)
                    errors.Add($"[{tag}] enemy '{e.name}' range must be > 0.");
            }

            foreach (var s in bp.secrets ?? System.Array.Empty<SecretDef>())
                CheckInBounds(tag, $"secret '{s.name}'", s.position, bp.worldWidth, errors, allowNegativeY: true);

            CheckPlatformOverlaps(tag, bp.platforms, errors);

            var rolesWithPads = new HashSet<PlayerRoleState.SquadRole>(
                (bp.rolePads ?? System.Array.Empty<RolePadDef>()).Select(p => p.role));
            foreach (var rg in bp.roleGates ?? System.Array.Empty<RoleGateDef>())
            {
                CheckInBounds(tag, $"role gate '{rg.name}'", rg.position, bp.worldWidth, errors, allowNegativeY: true);
                if (!rolesWithPads.Contains(rg.requiredRole))
                    warnings.Add($"[{tag}] role gate '{rg.name}' requires role {rg.requiredRole} but no role pad grants it anywhere in this level.");
            }

            if (eslCatalog != null)
            {
                var ids = (bp.villainGates ?? System.Array.Empty<VillainGateDef>()).Select(g => g.encounterId);
                foreach (var err in EslContentValidator.ValidateGateReferences(ids, eslCatalog))
                    errors.Add($"[{tag}] {err}");
            }

            return (errors, warnings);
        }

        static void CheckInBounds(string tag, string what, Vector2 pos, float worldWidth, List<string> errors, bool allowNegativeY = false)
        {
            if (float.IsNaN(pos.x) || float.IsNaN(pos.y))
            {
                errors.Add($"[{tag}] {what} has a NaN position.");
                return;
            }

            if (pos.x < -BoundsMargin || pos.x > worldWidth + BoundsMargin)
                errors.Add($"[{tag}] {what} x={pos.x} is outside the level's world bounds (0..{worldWidth}).");

            if (!allowNegativeY && pos.y < -100f)
                errors.Add($"[{tag}] {what} y={pos.y} looks like it fell out of the level.");
        }

        static void CheckPlatformOverlaps(string tag, PlatformDef[] platforms, List<string> errors)
        {
            if (platforms == null) return;
            for (int i = 0; i < platforms.Length; i++)
            {
                for (int j = i + 1; j < platforms.Length; j++)
                {
                    if (RectsOverlap(platforms[i], platforms[j]))
                        errors.Add($"[{tag}] platforms '{platforms[i].name}' and '{platforms[j].name}' overlap.");
                }
            }
        }

        static bool RectsOverlap(PlatformDef a, PlatformDef b)
        {
            float aLeft = a.position.x - a.size.x / 2f, aRight = a.position.x + a.size.x / 2f;
            float aBottom = a.position.y - a.size.y / 2f, aTop = a.position.y + a.size.y / 2f;
            float bLeft = b.position.x - b.size.x / 2f, bRight = b.position.x + b.size.x / 2f;
            float bBottom = b.position.y - b.size.y / 2f, bTop = b.position.y + b.size.y / 2f;

            return aLeft < bRight && aRight > bLeft && aBottom < bTop && aTop > bBottom;
        }
    }
}
