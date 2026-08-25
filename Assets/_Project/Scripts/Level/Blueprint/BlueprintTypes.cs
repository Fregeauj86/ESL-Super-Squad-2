using System;
using UnityEngine;

namespace FromCell.Level
{
    /// <summary>
    /// Plain data describing one piece of level content - no live Unity object references,
    /// so a whole level can be authored, diffed, and statically validated as C# data before
    /// LevelAssembler ever turns it into real GameObjects at play time. Each Def type mirrors
    /// the parameters the existing editor-time builders in FromCellSetupMenu.cs already take,
    /// so LevelAssembler's job is mechanical: one Def -> one GameObject, same shape as before.
    /// </summary>
    [Serializable]
    public class PlatformDef
    {
        public string name;
        public Vector2 position;
        public Vector2 size;
        public Color color = new Color(0.35f, 0.55f, 0.4f);

        public PlatformDef() { }
        public PlatformDef(string name, Vector2 position, Vector2 size)
        {
            this.name = name;
            this.position = position;
            this.size = size;
        }
    }

    /// <summary>
    /// Mirrors FromCellSetupMenu.CreateRoleGate exactly: a trigger volume plus a separate
    /// solid "Blocker" child that disables while the player holds the matching role - not a
    /// variant of PlatformDef, a distinct trigger+blocker structure.
    /// </summary>
    [Serializable]
    public class RoleGateDef
    {
        public string name;
        public Vector2 position;
        public PlayerRoleState.SquadRole requiredRole;

        public RoleGateDef() { }
        public RoleGateDef(string name, Vector2 position, PlayerRoleState.SquadRole requiredRole)
        {
            this.name = name;
            this.position = position;
            this.requiredRole = requiredRole;
        }
    }

    [Serializable]
    public class HazardDef
    {
        public string name;
        public Vector2 position;
        public Vector2 size;

        public HazardDef() { }
        public HazardDef(string name, Vector2 position, Vector2 size)
        {
            this.name = name;
            this.position = position;
            this.size = size;
        }
    }

    [Serializable]
    public class CollectibleDef
    {
        public string name;
        public Vector2 position;

        public CollectibleDef() { }
        public CollectibleDef(string name, Vector2 position)
        {
            this.name = name;
            this.position = position;
        }
    }

    [Serializable]
    public class GrowthPickupDef
    {
        public string name;
        public Vector2 position;

        public GrowthPickupDef() { }
        public GrowthPickupDef(string name, Vector2 position)
        {
            this.name = name;
            this.position = position;
        }
    }

    [Serializable]
    public class CheckpointDef
    {
        public string name;
        public Vector2 position;

        public CheckpointDef() { }
        public CheckpointDef(string name, Vector2 position)
        {
            this.name = name;
            this.position = position;
        }
    }

    [Serializable]
    public class RolePadDef
    {
        public string name;
        public Vector2 position;
        public PlayerRoleState.SquadRole role;

        public RolePadDef() { }
        public RolePadDef(string name, Vector2 position, PlayerRoleState.SquadRole role)
        {
            this.name = name;
            this.position = position;
            this.role = role;
        }
    }

    [Serializable]
    public class WindZoneDef
    {
        public string name;
        public Vector2 position;
        public Vector2 size;
        public Vector2 force;

        public WindZoneDef() { }
        public WindZoneDef(string name, Vector2 position, Vector2 size, Vector2 force)
        {
            this.name = name;
            this.position = position;
            this.size = size;
            this.force = force;
        }
    }

    /// <summary>Mirrors Level.Platforms.MovingPlatform's authoring shape - waypoints must have
    /// at least 2 points for the platform to move at all (validated).</summary>
    [Serializable]
    public class MovingPlatformDef
    {
        public string name;
        public Vector2[] waypoints = Array.Empty<Vector2>();
        public Vector2 size = new Vector2(2f, 0.5f);
        public float speed = 2f;
        public bool loop;
        public Platforms.CarryMode carryMode = Platforms.CarryMode.PositionDelta;
        public Color color = new Color(0.5f, 0.45f, 0.65f);

        public MovingPlatformDef() { }
        public MovingPlatformDef(string name, Vector2[] waypoints, Vector2 size, float speed)
        {
            this.name = name;
            this.waypoints = waypoints;
            this.size = size;
            this.speed = speed;
        }
    }

    [Serializable]
    public class CrumblingPlatformDef
    {
        public string name;
        public Vector2 position;
        public Vector2 size = new Vector2(2f, 0.5f);

        public CrumblingPlatformDef() { }
        public CrumblingPlatformDef(string name, Vector2 position, Vector2 size)
        {
            this.name = name;
            this.position = position;
            this.size = size;
        }
    }

    [Serializable]
    public class OneWayPlatformDef
    {
        public string name;
        public Vector2 position;
        public Vector2 size = new Vector2(2f, 0.5f);

        public OneWayPlatformDef() { }
        public OneWayPlatformDef(string name, Vector2 position, Vector2 size)
        {
            this.name = name;
            this.position = position;
            this.size = size;
        }
    }

    public enum EnemyKind { Patrol, Floater, Chaser }

    /// <summary>One Def covers all three enemy movement types (rather than three near-
    /// identical Def classes) - `range` means patrol distance for Patrol/Floater and detect
    /// radius for Chaser; `secondary` is only used by Floater (bob amplitude/frequency
    /// packed as x/y, since a single extra float wasn't enough and a 4th Def-only field for
    /// one enemy type wasn't worth it).</summary>
    [Serializable]
    public class EnemyDef
    {
        public string name;
        public Vector2 position;
        public EnemyKind kind;
        public float speed = 1.5f;
        public float range = 3f;
        public Vector2 secondary = new Vector2(1f, 1.5f);

        public EnemyDef() { }
        public EnemyDef(string name, Vector2 position, EnemyKind kind, float speed, float range)
        {
            this.name = name;
            this.position = position;
            this.kind = kind;
            this.speed = speed;
            this.range = range;
        }
    }

    [Serializable]
    public class SecretDef
    {
        public string name;
        public Vector2 position;

        public SecretDef() { }
        public SecretDef(string name, Vector2 position)
        {
            this.name = name;
            this.position = position;
        }
    }
}
