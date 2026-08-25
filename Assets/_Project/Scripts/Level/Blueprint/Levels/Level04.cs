using UnityEngine;

namespace FromCell.Level
{
    /// <summary>
    /// Level 4 - "Gentle Giant" (Max Elephant, stage 3: Crawl movement, weak jump). Stepped
    /// climb over a wide hazard pool, ending at the Question Owl (B1) gate. Scaled/extended
    /// from the original stepped-climb design.
    /// </summary>
    public static class Level04
    {
        public static LevelBlueprint Build() =>
            LevelBlueprintBuilder.Create("Level 4 - Gentle Giant", levelIndex: 3, stageIndex: 3)
                .WorldWidth(150f)
                .Spawn(new Vector2(-9f, -1f))
                .Finish(new Vector2(148f, 4f))
                .RequireCollectibles(0)
                .Tutorial("Weak jump unlocked. Climb toward the outpost.")

                .Ground("Ground_Start", new Vector2(-7f, -3f), new Vector2(8f, 1f))
                .Ground("Step1", new Vector2(5f, -1f), new Vector2(6f, 0.6f))
                .Ground("Step2", new Vector2(20f, 1f), new Vector2(6f, 0.6f))
                .Ground("Step3", new Vector2(35f, 3f), new Vector2(6f, 0.6f))
                .Ground("Step4", new Vector2(55f, 3.5f), new Vector2(10f, 0.6f))
                .Ground("Step5", new Vector2(80f, 3f), new Vector2(8f, 0.6f))
                .Ground("Step6", new Vector2(100f, 2f), new Vector2(8f, 0.6f))
                .Ground("Ground_Approach", new Vector2(125f, 3f), new Vector2(20f, 1f))
                .Ground("Ground_End", new Vector2(148f, 3.5f), new Vector2(10f, 1f))

                .Hazard("Tide_Pool", new Vector2(20f, -4.5f), new Vector2(80f, 1f))

                .Collectible("VocabGem_1", new Vector2(5f, 0f))
                .Collectible("VocabGem_2", new Vector2(35f, 4f))
                .Collectible("VocabGem_3", new Vector2(80f, 4f))

                .Checkpoint("Checkpoint_Mid", new Vector2(55f, 4.5f))

                .VillainGate("questionowl_questions", new Vector2(139f, 3.5f))

                .Build();
    }
}
