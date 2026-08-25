using UnityEngine;

namespace FromCell.Level
{
    /// <summary>
    /// Level 5 - "Deep Diver" (Finn Whale, stage 4: Crawl movement, jump). A vertical shaft
    /// climb where confidence stars grant permanent speed/jump bonuses partway up - scaled
    /// from the original vertical-shaft design. No villain gate; this level's whole beat is
    /// the growth-pickup climb.
    /// </summary>
    public static class Level05
    {
        public static LevelBlueprint Build() =>
            LevelBlueprintBuilder.Create("Level 5 - Deep Diver", levelIndex: 4, stageIndex: 4)
                .WorldWidth(40f)
                .Spawn(new Vector2(-10f, -22f))
                .Finish(new Vector2(0f, 44f))
                .RequireCollectibles(0)
                .Tutorial("Grab confidence stars to get stronger.")

                .Ground("Shaft_Left", new Vector2(-16f, 0f), new Vector2(5f, 66f))
                .Ground("Shaft_Right", new Vector2(16f, 0f), new Vector2(5f, 66f))
                .Ground("Platform_1", new Vector2(0f, -12f), new Vector2(14f, 2f))
                .Ground("Platform_2", new Vector2(0f, 6f), new Vector2(14f, 2f))
                .Ground("Platform_3", new Vector2(0f, 24f), new Vector2(14f, 2f))
                .Ground("Exit_Ledge", new Vector2(0f, 41f), new Vector2(16f, 3f))

                .Growth("ConfidenceStar_1", new Vector2(0f, -6f))
                .Growth("ConfidenceStar_2", new Vector2(0f, 12f))
                .Growth("ConfidenceStar_3", new Vector2(0f, 30f))

                .Collectible("VocabGem_1", new Vector2(-10f, -12f))
                .Collectible("VocabGem_2", new Vector2(10f, 6f))
                .Collectible("VocabGem_3", new Vector2(-10f, 24f))

                .Checkpoint("Checkpoint_Mid", new Vector2(0f, 6f))

                .Build();
    }
}
