using UnityEngine;

namespace FromCell.Level
{
    /// <summary>
    /// Level 2 - "Steady Scout" (Milo Mouse, stage 1: Float movement, no jump, faster).
    /// Branching drift paths converging at a checkpoint, then the Builder Bear (A2) gate.
    /// Scaled/extended from the original branching-path design.
    /// </summary>
    public static class Level02
    {
        public static LevelBlueprint Build() =>
            LevelBlueprintBuilder.Create("Level 2 - Steady Scout", levelIndex: 1, stageIndex: 1)
                .WorldWidth(160f)
                .Spawn(new Vector2(-8f, 0f))
                .Finish(new Vector2(155f, 2f))
                .RequireCollectibles(3)
                .Tutorial("Collect 3 vocabulary gems before you leave.")

                .Ground("Ground_Start", new Vector2(-6f, -1.5f), new Vector2(10f, 1f))
                .Ground("Branch_A", new Vector2(20f, 1f), new Vector2(24f, 0.8f))
                .Ground("Branch_B", new Vector2(55f, 3f), new Vector2(28f, 0.8f))
                .Ground("Ground_Mid", new Vector2(95f, 2f), new Vector2(20f, 1f))
                .Ground("Ground_Approach", new Vector2(135f, 2.5f), new Vector2(24f, 1f))
                .Ground("Ground_End", new Vector2(158f, 3f), new Vector2(14f, 1f))

                .Hazard("Pit_Hazard_1", new Vector2(12f, -3f), new Vector2(14f, 1f))
                .Hazard("Pit_Hazard_2", new Vector2(78f, -3f), new Vector2(16f, 1f))

                .Wind("Current_Branch", new Vector2(20f, 5f), new Vector2(24f, 6f), new Vector2(4f, 0f))

                .Collectible("VocabGem_1", new Vector2(0f, 1f))
                .Collectible("VocabGem_2", new Vector2(20f, 3f))
                .Collectible("VocabGem_3", new Vector2(55f, 5f))
                .Collectible("VocabGem_4", new Vector2(95f, 4f))
                .Collectible("VocabGem_5", new Vector2(135f, 4.5f))

                .Checkpoint("Checkpoint_Mid", new Vector2(55f, 5f))

                .VillainGate("builderbear_sentences", new Vector2(146f, 2.5f))

                .Build();
    }
}
