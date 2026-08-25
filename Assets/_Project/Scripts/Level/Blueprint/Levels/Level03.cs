using UnityEngine;

namespace FromCell.Level
{
    /// <summary>
    /// Level 3 - "Shell Guard" (Timmy Turtle, stage 2: Float movement, jump unlocked).
    /// Introduces the Squad-role switch mechanic (Nerve/Muscle pads opening role gates).
    /// Scaled/extended from the original role-gate design; no villain gate this level -
    /// the new mechanic is the whole beat.
    /// </summary>
    public static class Level03
    {
        public static LevelBlueprint Build() =>
            LevelBlueprintBuilder.Create("Level 3 - Shell Guard", levelIndex: 2, stageIndex: 2)
                .WorldWidth(140f)
                .Spawn(new Vector2(-7f, 0f))
                .Finish(new Vector2(135f, 4f))
                .RequireCollectibles(0)
                .Tutorial("Step on colored pads to switch Squad roles.")

                .Ground("Ground_Start", new Vector2(-5f, -2f), new Vector2(10f, 1f))
                .Ground("Ground_Mid", new Vector2(20f, 2f), new Vector2(16f, 0.8f))
                .RolePad("NervePad", new Vector2(5f, 1f), PlayerRoleState.SquadRole.Nerve)
                .RolePad("MusclePad", new Vector2(35f, 4f), PlayerRoleState.SquadRole.Muscle)
                .RoleGate("NerveGate", new Vector2(15f, 0f), PlayerRoleState.SquadRole.Nerve)
                .RoleGate("MuscleGate", new Vector2(60f, 3f), PlayerRoleState.SquadRole.Muscle)
                .Ground("Ground_AfterNerveGate", new Vector2(45f, 3f), new Vector2(16f, 0.8f))
                .Ground("Ground_AfterMuscleGate", new Vector2(85f, 5f), new Vector2(20f, 0.8f))
                .Ground("Ground_End", new Vector2(125f, 6f), new Vector2(22f, 1f))

                .Hazard("Spike_Hazard", new Vector2(30f, -4f), new Vector2(20f, 1f))

                .Collectible("VocabGem_1", new Vector2(20f, 4f))
                .Collectible("VocabGem_2", new Vector2(85f, 7f))

                .Checkpoint("Checkpoint_MidGate", new Vector2(45f, 4.5f))

                // Phase 4 enrichment: a patrolling guard on the post-Nerve-gate stretch, and a
                // bonus secret tucked above the mid platform - both purely additive, neither
                // sits on the required traversal path (the finish is still reachable with only
                // the platforms/gates above).
                .Enemy("Enemy_Patrol1", new Vector2(45f, 3.8f), EnemyKind.Patrol, speed: 1.2f, range: 6f)
                .Secret("Secret_1", new Vector2(25f, 3.2f))

                .Build();
    }
}
