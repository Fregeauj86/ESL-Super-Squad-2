using UnityEngine;

namespace FromCell.Level
{
    /// <summary>
    /// Level 9 - "Master Mentor" (Dr Imperfecto, stage 8: Walk movement, dash unlocked).
    /// A long runway of dash-only gaps over a pit - scaled/extended from the original dash
    /// gauntlet. No villain gate; the dash mechanic itself is the whole beat, one level
    /// before the finale.
    /// </summary>
    public static class Level09
    {
        public static LevelBlueprint Build() =>
            LevelBlueprintBuilder.Create("Level 9 - Master Mentor", levelIndex: 8, stageIndex: 8)
                .WorldWidth(190f)
                .Spawn(new Vector2(-10f, 0f))
                .Finish(new Vector2(185f, 1f))
                .RequireCollectibles(0)
                .Tutorial("Dash through the gaps. Mind the pit.")

                .Ground("Runway_Start", new Vector2(-6f, -1f), new Vector2(12f, 1f))
                .Ground("Runway_1", new Vector2(20f, -1f), new Vector2(12f, 1f))
                .Ground("Gap_A", new Vector2(40f, -1f), new Vector2(4f, 1f))
                .Ground("Runway_2", new Vector2(60f, -1f), new Vector2(12f, 1f))
                .Ground("Gap_B", new Vector2(80f, -1f), new Vector2(4f, 1f))
                .Ground("Runway_3", new Vector2(100f, -1f), new Vector2(12f, 1f))
                .Ground("Gap_C", new Vector2(120f, -1f), new Vector2(4f, 1f))
                .Ground("Runway_4", new Vector2(145f, 0f), new Vector2(16f, 1f))
                .Ground("Runway_End", new Vector2(183f, 1f), new Vector2(16f, 1f))

                .Hazard("Pit", new Vector2(75f, -4f), new Vector2(120f, 1f))

                .Collectible("VocabGem_1", new Vector2(20f, 1f))
                .Collectible("VocabGem_2", new Vector2(60f, 1f))
                .Collectible("VocabGem_3", new Vector2(100f, 1f))

                .Checkpoint("Checkpoint_Mid", new Vector2(100f, 1f))

                // Phase 4 enrichment: a hovering floater over the pit (avoid-only, well above
                // the dash line) and an optional one-way bonus platform + secret above
                // Runway_2 - all purely additive, the dash-the-gaps critical path is untouched.
                .Enemy("Enemy_Floater1", new Vector2(75f, 2f), EnemyKind.Floater, speed: 1f, range: 4f, secondary: new Vector2(1.2f, 1.2f))
                .OneWayPlatform("Bonus_Platform", new Vector2(60f, 3.5f))
                .Secret("Secret_1", new Vector2(60f, 4f))

                .Build();
    }
}
