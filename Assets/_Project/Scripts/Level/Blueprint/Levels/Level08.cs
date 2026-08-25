using UnityEngine;

namespace FromCell.Level
{
    /// <summary>
    /// Level 8 - "Power Hop" (Big Tick, stage 7: Walk movement, double jump unlocked). Tall
    /// ledges only reachable with the new double jump, ending at the Debate Hawk (C1) gate.
    /// Scaled/extended from the original high-ledge design.
    /// </summary>
    public static class Level08
    {
        public static LevelBlueprint Build() =>
            LevelBlueprintBuilder.Create("Level 8 - Power Hop", levelIndex: 7, stageIndex: 7)
                .WorldWidth(160f)
                .Spawn(new Vector2(-8f, 0f))
                .Finish(new Vector2(155f, 12f))
                .RequireCollectibles(0)
                .Tutorial("Double jump is online. Go explore upward.")

                .Ground("Ground_Start", new Vector2(-6f, -1f), new Vector2(10f, 1f))
                .Ground("Ground_Mid", new Vector2(20f, 2f), new Vector2(10f, 0.8f))
                .Ground("Ledge_1", new Vector2(45f, 5f), new Vector2(8f, 0.8f))
                .Ground("Ledge_2", new Vector2(70f, 8f), new Vector2(8f, 0.8f))
                .Ground("High_Ledge", new Vector2(100f, 11f), new Vector2(10f, 0.8f))
                .Ground("Ground_Approach", new Vector2(130f, 11.5f), new Vector2(16f, 1f))
                .Ground("Ground_End", new Vector2(155f, 12.5f), new Vector2(14f, 1f))

                .Hazard("Fall_Zone", new Vector2(60f, -4f), new Vector2(130f, 1f))

                .Collectible("VocabGem_1", new Vector2(20f, 4f))
                .Collectible("VocabGem_2", new Vector2(45f, 7f))
                .Collectible("VocabGem_3", new Vector2(70f, 10f))
                .Collectible("VocabGem_4", new Vector2(100f, 13f))

                .Checkpoint("Checkpoint_Mid", new Vector2(70f, 10f))

                .VillainGate("debatehawk_opinions", new Vector2(146f, 13f))

                .Build();
    }
}
