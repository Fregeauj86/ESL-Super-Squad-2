using UnityEngine;

namespace FromCell.Level
{
    /// <summary>
    /// Level 1 - "First Steps" (Milo Mouse, stage 0: Float movement, no jump). Traversed
    /// purely by drifting on wind currents, matching the original Level 1's mechanic -
    /// extended to the new 120-220 unit target (was ~22 units). Ends at an EchoFox (A1)
    /// villain gate right before the finish.
    /// </summary>
    public static class Level01
    {
        public static LevelBlueprint Build() =>
            LevelBlueprintBuilder.Create("Level 1 - First Steps", levelIndex: 0, stageIndex: 0)
                .WorldWidth(150f)
                .Spawn(new Vector2(-8f, -0.5f))
                .Finish(new Vector2(145f, 0f))
                .RequireCollectibles(0)
                .Tutorial("Drift toward the glowing exit. Jump unlocks soon.")

                .Ground("Floor_Main", new Vector2(65f, -2f), new Vector2(170f, 2f))

                .Wind("Current_1", new Vector2(10f, 2f), new Vector2(50f, 6f), new Vector2(6f, 0f))
                .Wind("Current_2", new Vector2(65f, 2f), new Vector2(50f, 6f), new Vector2(7f, 0f))
                .Wind("Current_3", new Vector2(115f, 2f), new Vector2(50f, 6f), new Vector2(8f, 0f))

                .Collectible("VocabGem_1", new Vector2(0f, -0.5f))
                .Collectible("VocabGem_2", new Vector2(30f, -0.5f))
                .Collectible("VocabGem_3", new Vector2(60f, -0.5f))
                .Collectible("VocabGem_4", new Vector2(90f, -0.5f))
                .Collectible("VocabGem_5", new Vector2(120f, -0.5f))

                .Checkpoint("Checkpoint_Mid", new Vector2(70f, -0.5f))

                .VillainGate("echofox_intro", new Vector2(132f, -0.5f))

                .Build();
    }
}
