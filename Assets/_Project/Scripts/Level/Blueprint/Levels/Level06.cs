using UnityEngine;

namespace FromCell.Level
{
    /// <summary>
    /// Level 6 - "Rising Wings" (Sky Eagle, stage 5: Walk movement begins, faster). Precision
    /// jumps across rising pillars over a long gap, ending at the Connector Snake (B2) gate.
    /// Scaled/extended from the original precision-jump design.
    /// </summary>
    public static class Level06
    {
        public static LevelBlueprint Build() =>
            LevelBlueprintBuilder.Create("Level 6 - Rising Wings", levelIndex: 5, stageIndex: 5)
                .WorldWidth(170f)
                .Spawn(new Vector2(-8f, 0f))
                .Finish(new Vector2(165f, 3f))
                .RequireCollectibles(0)
                .Tutorial("Precision jumps. Don't fall.")

                .Ground("Ground_Start", new Vector2(-6f, -1f), new Vector2(8f, 1f))
                .Ground("Pillar_1", new Vector2(10f, 0f), new Vector2(4f, 0.5f))
                .Ground("Pillar_2", new Vector2(24f, 1.2f), new Vector2(4f, 0.5f))
                .Ground("Pillar_3", new Vector2(38f, 2.4f), new Vector2(4f, 0.5f))
                .Ground("Pillar_4", new Vector2(52f, 3.4f), new Vector2(4f, 0.5f))
                .Ground("Mid_Rest", new Vector2(70f, 4f), new Vector2(14f, 1f))
                .Ground("Pillar_5", new Vector2(90f, 3.2f), new Vector2(4f, 0.5f))
                .Ground("Pillar_6", new Vector2(104f, 2.4f), new Vector2(4f, 0.5f))
                .Ground("Pillar_7", new Vector2(118f, 1.6f), new Vector2(4f, 0.5f))
                .Ground("Ground_Approach", new Vector2(140f, 1.2f), new Vector2(20f, 1f))
                .Ground("Ground_End", new Vector2(163f, 2.5f), new Vector2(12f, 1f))

                .Hazard("Gap_Fall", new Vector2(30f, -4f), new Vector2(90f, 1f))

                .Collectible("VocabGem_1", new Vector2(24f, 3f))
                .Collectible("VocabGem_2", new Vector2(70f, 6f))
                .Collectible("VocabGem_3", new Vector2(104f, 4.2f))

                .Checkpoint("Checkpoint_Mid", new Vector2(70f, 6f))

                .VillainGate("connectorsnake_linking", new Vector2(153f, 1.5f))

                .Build();
    }
}
