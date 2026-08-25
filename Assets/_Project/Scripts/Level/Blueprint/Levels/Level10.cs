using UnityEngine;

namespace FromCell.Level
{
    /// <summary>
    /// Level 10 - "Squad Champion" (King Leo, stage 9: full ability kit, max stats). The
    /// final gauntlet mixing every mechanic introduced so far, ending at The Mimic (C2) -
    /// the last and hardest villain gate, right before the finish. Scaled/extended from the
    /// original final-gauntlet design.
    /// </summary>
    public static class Level10
    {
        public static LevelBlueprint Build() =>
            LevelBlueprintBuilder.Create("Level 10 - Squad Champion", levelIndex: 9, stageIndex: 9)
                .WorldWidth(210f)
                .Spawn(new Vector2(-10f, 0f))
                .Finish(new Vector2(205f, 8f))
                .RequireCollectibles(0)
                .Tutorial("Final gauntlet. All abilities unlocked.")

                .Ground("Phase1", new Vector2(-6f, -1f), new Vector2(12f, 1f))
                .Ground("Phase2", new Vector2(20f, 2f), new Vector2(10f, 0.8f))
                .Ground("Phase3", new Vector2(50f, 5f), new Vector2(10f, 0.8f))
                .Ground("Phase4", new Vector2(80f, 4f), new Vector2(14f, 0.8f))
                .Ground("Phase5_DashRunway", new Vector2(105f, 3f), new Vector2(10f, 1f))
                .Ground("Phase5_DashGap", new Vector2(120f, 3f), new Vector2(5f, 1f))
                .Ground("Phase6", new Vector2(140f, 4f), new Vector2(14f, 0.8f))
                .Ground("Phase7", new Vector2(170f, 6.5f), new Vector2(14f, 0.8f))
                .Ground("Finish_Platform", new Vector2(200f, 8.5f), new Vector2(16f, 1f))

                .Hazard("Gauntlet_Pit_1", new Vector2(20f, -3f), new Vector2(40f, 1f))
                .Hazard("Gauntlet_Pit_2", new Vector2(112f, -3f), new Vector2(30f, 1f))

                .Collectible("VocabGem_1", new Vector2(20f, 4f))
                .Collectible("VocabGem_2", new Vector2(50f, 7f))
                .Collectible("VocabGem_3", new Vector2(80f, 6f))
                .Collectible("VocabGem_4", new Vector2(140f, 6f))
                .Collectible("VocabGem_5", new Vector2(170f, 8.5f))

                .Checkpoint("Checkpoint_1", new Vector2(50f, 7f))
                .Checkpoint("Checkpoint_Final", new Vector2(140f, 6.5f))

                .VillainGate("themimic_fluency", new Vector2(190f, 9f))

                .Build();
    }
}
