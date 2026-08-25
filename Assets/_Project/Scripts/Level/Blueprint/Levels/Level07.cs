using UnityEngine;

namespace FromCell.Level
{
    /// <summary>
    /// Level 7 - "Fast Track" (Dash Cheetah, stage 6: Walk movement, standard baseline).
    /// A long open run with light furniture obstacles - the "full body platformer baseline"
    /// beat, scaled/extended from the original. No villain gate this level - a breather
    /// before the double-jump/dash/gauntlet run of levels 8-10.
    /// </summary>
    public static class Level07
    {
        public static LevelBlueprint Build() =>
            LevelBlueprintBuilder.Create("Level 7 - Fast Track", levelIndex: 6, stageIndex: 6)
                .WorldWidth(180f)
                .Spawn(new Vector2(-9f, -1f))
                .Finish(new Vector2(176f, 1f))
                .RequireCollectibles(0)
                .Tutorial("Standard platforming begins here.")

                .Ground("Floor", new Vector2(85f, -2.5f), new Vector2(190f, 1f))
                .Ground("Furniture_1", new Vector2(-4f, -0.5f), new Vector2(6f, 1f))
                .Ground("Furniture_2", new Vector2(20f, 0.5f), new Vector2(8f, 1f))
                .Ground("Furniture_3", new Vector2(55f, -0.5f), new Vector2(10f, 1f))
                .Ground("Furniture_4", new Vector2(90f, 0.5f), new Vector2(8f, 1f))
                .Ground("Furniture_5", new Vector2(130f, -0.5f), new Vector2(10f, 1f))
                .Ground("Crib", new Vector2(170f, 1.5f), new Vector2(10f, 1f))

                .Collectible("VocabGem_1", new Vector2(20f, 2.5f))
                .Collectible("VocabGem_2", new Vector2(55f, 1.5f))
                .Collectible("VocabGem_3", new Vector2(90f, 2.5f))
                .Collectible("VocabGem_4", new Vector2(130f, 1.5f))

                .Checkpoint("Checkpoint_Mid", new Vector2(90f, 2.5f))

                // Phase 4 enrichment: a rising moving platform in the clear space between
                // Furniture_3 (right edge x=60) and Furniture_4 (left edge x=86), and a
                // chaser guard near Furniture_5 - the full-length Floor beneath everything
                // means the finish stays reachable with or without either.
                .MovingPlatform("MovingPlatform_1", new[] { new Vector2(65f, 0.5f), new Vector2(85f, 2.5f) }, speed: 2.5f)
                .Enemy("Enemy_Chaser1", new Vector2(130f, 1f), EnemyKind.Chaser, speed: 3f, range: 6f)

                .Build();
    }
}
