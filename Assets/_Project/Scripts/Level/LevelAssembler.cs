using FromCell.Art;
using FromCell.ESL;
using FromCell.Level.Enemy;
using FromCell.Level.Platforms;
using UnityEngine;

namespace FromCell.Level
{
    /// <summary>
    /// Reads the LevelBlueprint for `levelIndex` from LevelCatalog and instantiates its
    /// content at play time - the runtime counterpart to the editor-time
    /// CreateGroundPlatform/CreateKillZone/CreateCollectible/etc. helpers in
    /// FromCellSetupMenu.cs, mirroring their exact GameObject shape (tags, collider sizing
    /// via transform.localScale for platforms/hazards, CircleCollider2D.radius for pickups)
    /// so a runtime-built level looks and behaves identically to a hand-built one. Uses
    /// RuntimeShapes instead of AssetDatabase, since AssetDatabase is editor-only and this
    /// runs during actual gameplay.
    ///
    /// Dropped into a level scene's setupEnvironment callback by BuildGrayboxLevelPublic
    /// (unmodified) - everything else (camera, PlayerSpawn, CheckpointSystem, FinishZone,
    /// LevelBootstrap, GameFlowSystem, EvolutionSystem, LevelCompletionSystem, mobile UI,
    /// player instantiation) stays exactly as that method already builds it. Runs in Awake(),
    /// which is guaranteed to complete before LevelBootstrap.Start() positions the player.
    ///
    /// If LevelCatalog.Get(levelIndex) returns null (level not authored yet), this is a
    /// deliberate no-op - nothing is built, nothing errors.
    /// </summary>
    public class LevelAssembler : MonoBehaviour
    {
        public int levelIndex;

        void Awake()
        {
            var bp = LevelCatalog.Get(levelIndex);
            if (bp == null) return;

            Collectible.ResetTotal();

            foreach (var p in bp.platforms) BuildPlatform(p);
            foreach (var h in bp.hazards) BuildHazard(h);
            foreach (var c in bp.collectibles) BuildCollectible(c);
            foreach (var g in bp.growthPickups) BuildGrowthPickup(g);
            foreach (var cp in bp.checkpoints) BuildCheckpoint(cp);
            foreach (var rp in bp.rolePads) BuildRolePad(rp);
            foreach (var rg in bp.roleGates) BuildRoleGate(rg);
            foreach (var w in bp.windZones) BuildWindZone(w);
            foreach (var vg in bp.villainGates) BuildVillainGate(vg);
            foreach (var mp in bp.movingPlatforms) BuildMovingPlatform(mp);
            foreach (var cp in bp.crumblingPlatforms) BuildCrumblingPlatform(cp);
            foreach (var op in bp.oneWayPlatforms) BuildOneWayPlatform(op);
            foreach (var e in bp.enemies) BuildEnemy(e);
            foreach (var s in bp.secrets) BuildSecret(s);
        }

        static void BuildPlatform(PlatformDef def)
        {
            var go = new GameObject(def.name);
            go.tag = "Ground";
            go.transform.position = def.position;
            go.transform.localScale = new Vector3(def.size.x, def.size.y, 1f);
            go.AddComponent<BoxCollider2D>();

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = RuntimeShapes.Square();
            sr.color = def.color;
            sr.drawMode = SpriteDrawMode.Sliced;
        }

        static void BuildHazard(HazardDef def)
        {
            var go = new GameObject(def.name);
            go.transform.position = def.position;
            go.transform.localScale = new Vector3(def.size.x, def.size.y, 1f);

            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            go.AddComponent<KillZone>();

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = RuntimeShapes.Square();
            sr.color = new Color(0.75f, 0.2f, 0.25f, 0.45f);
        }

        static void BuildCollectible(CollectibleDef def)
        {
            var go = new GameObject(def.name);
            go.transform.position = def.position;

            var col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.35f;
            go.AddComponent<Collectible>();

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = RuntimeShapes.Circle();
            sr.color = new Color(0.95f, 0.85f, 0.3f);
            go.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        }

        static void BuildGrowthPickup(GrowthPickupDef def)
        {
            var go = new GameObject(def.name);
            go.transform.position = def.position;

            var col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.4f;
            go.AddComponent<GrowthPickup>();

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = RuntimeShapes.Circle();
            sr.color = new Color(0.95f, 0.55f, 0.75f);
            go.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
        }

        static void BuildCheckpoint(CheckpointDef def)
        {
            var go = new GameObject(def.name);
            go.transform.position = def.position;

            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(1.5f, 2f);
            go.AddComponent<Checkpoint>();
        }

        static void BuildRolePad(RolePadDef def)
        {
            var go = new GameObject(def.name);
            go.transform.position = def.position;

            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(1.5f, 0.5f);

            var pad = go.AddComponent<RoleSwitchPad>();
            pad.Configure(def.role);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = RuntimeShapes.Square();
            sr.color = def.role == PlayerRoleState.SquadRole.Nerve
                ? new Color(0.5f, 0.6f, 0.95f, 0.8f)
                : new Color(0.95f, 0.5f, 0.45f, 0.8f);
            go.transform.localScale = new Vector3(1.5f, 0.4f, 1f);
        }

        static void BuildRoleGate(RoleGateDef def)
        {
            var gateRoot = new GameObject(def.name);
            gateRoot.transform.position = def.position;

            var triggerCol = gateRoot.AddComponent<BoxCollider2D>();
            triggerCol.isTrigger = true;
            triggerCol.size = new Vector2(2f, 2f);

            var blocker = new GameObject("Blocker");
            blocker.transform.SetParent(gateRoot.transform);
            blocker.transform.localPosition = Vector3.zero;
            var blockCol = blocker.AddComponent<BoxCollider2D>();
            blockCol.size = new Vector2(0.5f, 2f);

            var blockSr = blocker.AddComponent<SpriteRenderer>();
            blockSr.sprite = RuntimeShapes.Square();
            blockSr.color = new Color(0.6f, 0.3f, 0.7f, 0.9f);
            blocker.transform.localScale = new Vector3(0.5f, 2f, 1f);

            var gate = gateRoot.AddComponent<RoleGate>();
            gate.Configure(def.requiredRole, blockCol);
        }

        static void BuildWindZone(WindZoneDef def)
        {
            var go = new GameObject(def.name);
            go.transform.position = def.position;
            go.transform.localScale = new Vector3(def.size.x, def.size.y, 1f);

            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;

            var wind = go.AddComponent<FromCell.Level.WindZone>();
            wind.Configure(def.force);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = RuntimeShapes.Square();
            sr.color = new Color(0.4f, 0.7f, 0.95f, 0.25f);
        }

        static void BuildVillainGate(VillainGateDef def)
        {
            var gateRoot = new GameObject($"VillainGate_{def.encounterId}");
            gateRoot.transform.position = def.position;

            var triggerCol = gateRoot.AddComponent<BoxCollider2D>();
            triggerCol.isTrigger = true;
            triggerCol.size = def.gateSize;

            var blocker = new GameObject("Blocker");
            blocker.transform.SetParent(gateRoot.transform);
            blocker.transform.localPosition = Vector3.zero;
            var blockCol = blocker.AddComponent<BoxCollider2D>();
            blockCol.size = def.gateSize;

            // Show the real villain sprite when its art is baked; SpriteBank falls back to
            // a plain tinted placeholder otherwise, so a missing bake never breaks the gate.
            string villainKey = ArtKeys.VillainForEncounter(def.encounterId);
            var villainSprite = villainKey != null ? SpriteBank.Get(villainKey) : RuntimeShapes.Square();

            var blockSr = blocker.AddComponent<SpriteRenderer>();
            blockSr.sprite = villainSprite;
            blockSr.color = villainKey != null ? Color.white : new Color(0.55f, 0.15f, 0.2f, 0.9f);

            // Scale relative to the sprite's OWN natural size (not a fixed assumption) -
            // RuntimeShapes.Square() is exactly 1 unit at its PPU, but a baked character
            // sprite has a different natural size at the higher PPU FromCellArtBaker uses
            // for characters, so a fixed multiplier would be wrong for one case or the other.
            float targetSize = Mathf.Max(def.gateSize.x, def.gateSize.y) * 1.4f;
            float naturalSize = villainSprite != null ? Mathf.Max(villainSprite.bounds.size.x, villainSprite.bounds.size.y) : 1f;
            float scale = naturalSize > 0f ? targetSize / naturalSize : 1f;
            blocker.transform.localScale = new Vector3(scale, scale, 1f);

            var gate = gateRoot.AddComponent<VillainGate>();
            gate.encounterId = def.encounterId;
            gate.blockingCollider = blockCol;
        }

        static void BuildMovingPlatform(MovingPlatformDef def)
        {
            var go = new GameObject(def.name);
            go.tag = "Ground";

            var col = go.AddComponent<BoxCollider2D>();
            col.size = def.size;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = RuntimeShapes.Square();
            sr.color = def.color;
            sr.drawMode = SpriteDrawMode.Sliced;
            sr.size = def.size;

            var platform = go.AddComponent<MovingPlatform>();
            platform.Configure(def.waypoints, def.speed, def.carryMode, def.loop);
        }

        static void BuildCrumblingPlatform(CrumblingPlatformDef def)
        {
            var go = new GameObject(def.name);
            go.tag = "Ground";
            go.transform.position = def.position;
            go.transform.localScale = new Vector3(def.size.x, def.size.y, 1f);

            go.AddComponent<BoxCollider2D>();

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = RuntimeShapes.Square();
            sr.color = new Color(0.7f, 0.55f, 0.3f);
            sr.drawMode = SpriteDrawMode.Sliced;

            go.AddComponent<CrumblingPlatform>();
        }

        static void BuildOneWayPlatform(OneWayPlatformDef def)
        {
            var go = new GameObject(def.name);
            go.tag = "Ground";
            go.transform.position = def.position;
            go.transform.localScale = new Vector3(def.size.x, def.size.y, 1f);

            go.AddComponent<BoxCollider2D>();

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = RuntimeShapes.Square();
            sr.color = new Color(0.4f, 0.75f, 0.5f, 0.85f);
            sr.drawMode = SpriteDrawMode.Sliced;

            go.AddComponent<OneWayPlatform>();
        }

        static void BuildEnemy(EnemyDef def)
        {
            var go = new GameObject(def.name);
            go.tag = "Enemy";
            go.transform.position = def.position;

            var rb = go.AddComponent<Rigidbody2D>();
            rb.freezeRotation = true;

            var col = go.AddComponent<CircleCollider2D>();
            col.radius = 0.4f;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = RuntimeShapes.Circle();
            sr.color = new Color(0.75f, 0.25f, 0.3f);
            go.transform.localScale = new Vector3(0.7f, 0.7f, 1f);

            switch (def.kind)
            {
                case EnemyKind.Patrol:
                    go.AddComponent<PatrolEnemy>().Configure(def.speed, def.range);
                    break;
                case EnemyKind.Floater:
                    go.AddComponent<FloaterEnemy>().Configure(def.secondary.x, def.secondary.y, def.speed, def.range);
                    break;
                case EnemyKind.Chaser:
                    go.AddComponent<ChaserEnemy>().Configure(def.speed, def.range);
                    break;
            }

            var hitboxGo = new GameObject("Hitbox");
            hitboxGo.transform.SetParent(go.transform, false);
            var hitboxCol = hitboxGo.AddComponent<CircleCollider2D>();
            hitboxCol.isTrigger = true;
            hitboxCol.radius = 0.4f;
            hitboxGo.AddComponent<EnemyHitbox>();
        }

        static void BuildSecret(SecretDef def)
        {
            var go = new GameObject(def.name);
            go.transform.position = def.position;

            var col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.35f;
            go.AddComponent<Secret>();

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = RuntimeShapes.Circle();
            sr.color = new Color(0.8f, 0.6f, 0.95f);
            go.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        }
    }
}
