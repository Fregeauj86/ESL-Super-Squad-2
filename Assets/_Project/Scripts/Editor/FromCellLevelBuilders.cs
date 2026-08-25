#if UNITY_EDITOR
using FromCell.Level;
using UnityEngine;

namespace FromCell.Editor
{
    static class FromCellLevelBuilders
    {
        public static void BuildLevel03Organism()
        {
            FromCellSetupMenu.BuildGrayboxLevelPublic(
                2, "Level_03_ShellGuard",
                new Vector3(-7f, 0f, 0f),
                new Vector3(13f, 2f, 0f),
                _ =>
                {
                    FromCellSetupMenu.CreateGroundPlatformPublic("Ground_Start", new Vector3(-5f, -1f, 0f), new Vector3(5f, 1f, 1f));
                    FromCellSetupMenu.CreateGroundPlatformPublic("Ground_Mid", new Vector3(2f, 0.5f, 0f), new Vector3(4f, 0.6f, 1f));

                    var nervePad = FromCellSetupMenu.CreateRolePad("NervePad", new Vector3(-2f, 0f, 0f), PlayerRoleState.SquadRole.Nerve);
                    var musclePad = FromCellSetupMenu.CreateRolePad("MusclePad", new Vector3(4f, 1.5f, 0f), PlayerRoleState.SquadRole.Muscle);

                    FromCellSetupMenu.CreateRoleGate("NerveGate", new Vector3(0f, -0.5f, 0f), PlayerRoleState.SquadRole.Nerve);
                    FromCellSetupMenu.CreateGroundPlatformPublic("Ground_AfterGate", new Vector3(6f, 1f, 0f), new Vector3(3f, 0.6f, 1f));
                    FromCellSetupMenu.CreateGroundPlatformPublic("Ground_End", new Vector3(11f, 2f, 0f), new Vector3(4f, 1f, 1f));
                    FromCellSetupMenu.CreateKillZonePublic("Spike_Hazard", new Vector3(3f, -2.5f, 0f), new Vector3(2f, 0.5f, 1f));
                });
        }

        public static void BuildLevel04Primitive()
        {
            FromCellSetupMenu.BuildGrayboxLevelPublic(
                3, "Level_04_GentleGiant",
                new Vector3(-8f, -1f, 0f),
                new Vector3(14f, 3f, 0f),
                _ =>
                {
                    FromCellSetupMenu.CreateGroundPlatformPublic("Ground_Start", new Vector3(-6f, -2f, 0f), new Vector3(4f, 1f, 1f));
                    FromCellSetupMenu.CreateGroundPlatformPublic("Ground_Step1", new Vector3(-2f, -0.5f, 0f), new Vector3(2f, 0.5f, 1f));
                    FromCellSetupMenu.CreateGroundPlatformPublic("Ground_Step2", new Vector3(2f, 0.5f, 0f), new Vector3(2f, 0.5f, 1f));
                    FromCellSetupMenu.CreateGroundPlatformPublic("Ground_Step3", new Vector3(6f, 1.5f, 0f), new Vector3(2f, 0.5f, 1f));
                    FromCellSetupMenu.CreateGroundPlatformPublic("Ground_End", new Vector3(12f, 2.5f, 0f), new Vector3(4f, 1f, 1f));
                    FromCellSetupMenu.CreateKillZonePublic("Tide_Pool", new Vector3(0f, -3.5f, 0f), new Vector3(14f, 1f, 1f));
                });
        }

        public static void BuildLevel05Embryo()
        {
            FromCellSetupMenu.BuildGrayboxLevelPublic(
                4, "Level_05_DeepDiver",
                new Vector3(-2f, -4f, 0f),
                new Vector3(2f, 8f, 0f),
                _ =>
                {
                    FromCellSetupMenu.CreateGroundPlatformPublic("Ground_Shaft_Left", new Vector3(-3f, 0f, 0f), new Vector3(1f, 12f, 1f));
                    FromCellSetupMenu.CreateGroundPlatformPublic("Ground_Shaft_Right", new Vector3(3f, 0f, 0f), new Vector3(1f, 12f, 1f));
                    FromCellSetupMenu.CreateGroundPlatformPublic("Platform_1", new Vector3(0f, -2f, 0f), new Vector3(2.5f, 0.4f, 1f));
                    FromCellSetupMenu.CreateGroundPlatformPublic("Platform_2", new Vector3(0f, 1f, 0f), new Vector3(2.5f, 0.4f, 1f));
                    FromCellSetupMenu.CreateGroundPlatformPublic("Platform_3", new Vector3(0f, 4f, 0f), new Vector3(2.5f, 0.4f, 1f));
                    FromCellSetupMenu.CreateGroundPlatformPublic("Ground_Exit", new Vector3(0f, 7.5f, 0f), new Vector3(3f, 0.6f, 1f));
                    FromCellSetupMenu.CreateGrowthPickupPublic("ConfidenceStar_1", new Vector3(0f, -1f, 0f));
                    FromCellSetupMenu.CreateGrowthPickupPublic("ConfidenceStar_2", new Vector3(0f, 2f, 0f));
                    FromCellSetupMenu.CreateGrowthPickupPublic("ConfidenceStar_3", new Vector3(0f, 5f, 0f));
                });
        }

        public static void BuildLevel06Nervous()
        {
            FromCellSetupMenu.BuildGrayboxLevelPublic(
                5, "Level_06_RisingWings",
                new Vector3(-8f, 0f, 0f),
                new Vector3(15f, 2f, 0f),
                _ =>
                {
                    FromCellSetupMenu.CreateGroundPlatformPublic("Ground_Start", new Vector3(-6f, -1f, 0f), new Vector3(4f, 1f, 1f));
                    for (int i = 0; i < 4; i++)
                        FromCellSetupMenu.CreateGroundPlatformPublic($"Neuron_{i + 1}", new Vector3(-2f + i * 3f, i * 0.6f, 0f), new Vector3(1.5f, 0.4f, 1f));
                    FromCellSetupMenu.CreateGroundPlatformPublic("Ground_End", new Vector3(13f, 2f, 0f), new Vector3(4f, 1f, 1f));
                    FromCellSetupMenu.CreateKillZonePublic("Gap_Fall", new Vector3(4f, -3f, 0f), new Vector3(8f, 1f, 1f));
                });
        }

        public static void BuildLevel07Newborn()
        {
            FromCellSetupMenu.BuildGrayboxLevelPublic(
                6, "Level_07_FastTrack",
                new Vector3(-9f, -1f, 0f),
                new Vector3(16f, 2f, 0f),
                _ =>
                {
                    FromCellSetupMenu.CreateGroundPlatformPublic("Floor", new Vector3(0f, -2.5f, 0f), new Vector3(30f, 1f, 1f));
                    FromCellSetupMenu.CreateGroundPlatformPublic("Furniture_1", new Vector3(-4f, -0.5f, 0f), new Vector3(3f, 0.5f, 1f));
                    FromCellSetupMenu.CreateGroundPlatformPublic("Furniture_2", new Vector3(2f, 0.5f, 0f), new Vector3(2.5f, 0.5f, 1f));
                    FromCellSetupMenu.CreateGroundPlatformPublic("Crib", new Vector3(14f, 1.5f, 0f), new Vector3(3f, 0.6f, 1f));
                });
        }

        public static void BuildLevel08Child()
        {
            FromCellSetupMenu.BuildGrayboxLevelPublic(
                7, "Level_08_PowerHop",
                new Vector3(-8f, 0f, 0f),
                new Vector3(14f, 6f, 0f),
                _ =>
                {
                    FromCellSetupMenu.CreateGroundPlatformPublic("Ground_Start", new Vector3(-6f, -1f, 0f), new Vector3(5f, 1f, 1f));
                    FromCellSetupMenu.CreateGroundPlatformPublic("Ground_Mid", new Vector3(0f, 1f, 0f), new Vector3(3f, 0.6f, 1f));
                    FromCellSetupMenu.CreateGroundPlatformPublic("High_Ledge", new Vector3(8f, 4f, 0f), new Vector3(3f, 0.6f, 1f));
                    FromCellSetupMenu.CreateGroundPlatformPublic("Ground_End", new Vector3(13f, 5.5f, 0f), new Vector3(3f, 1f, 1f));
                });
        }

        public static void BuildLevel09Teen()
        {
            FromCellSetupMenu.BuildGrayboxLevelPublic(
                8, "Level_09_MasterMentor",
                new Vector3(-10f, 0f, 0f),
                new Vector3(22f, 1f, 0f),
                _ =>
                {
                    FromCellSetupMenu.CreateGroundPlatformPublic("Runway_Start", new Vector3(-6f, -1f, 0f), new Vector3(6f, 1f, 1f));
                    FromCellSetupMenu.CreateGroundPlatformPublic("Runway_Mid", new Vector3(4f, -1f, 0f), new Vector3(6f, 1f, 1f));
                    FromCellSetupMenu.CreateGroundPlatformPublic("Gap_A", new Vector3(10f, -1f, 0f), new Vector3(2f, 1f, 1f));
                    FromCellSetupMenu.CreateGroundPlatformPublic("Gap_B", new Vector3(16f, -1f, 0f), new Vector3(2f, 1f, 1f));
                    FromCellSetupMenu.CreateGroundPlatformPublic("Runway_End", new Vector3(21f, 0f, 0f), new Vector3(4f, 1f, 1f));
                    FromCellSetupMenu.CreateKillZonePublic("Pit", new Vector3(13f, -3f, 0f), new Vector3(4f, 1f, 1f));
                });
        }

        public static void BuildLevel10Adult()
        {
            FromCellSetupMenu.BuildGrayboxLevelPublic(
                9, "Level_10_SquadChampion",
                new Vector3(-10f, 0f, 0f),
                new Vector3(24f, 4f, 0f),
                _ =>
                {
                    FromCellSetupMenu.CreateGroundPlatformPublic("Phase1", new Vector3(-6f, -1f, 0f), new Vector3(6f, 1f, 1f));
                    FromCellSetupMenu.CreateGroundPlatformPublic("Phase2", new Vector3(2f, 1f, 0f), new Vector3(3f, 0.6f, 1f));
                    FromCellSetupMenu.CreateGroundPlatformPublic("Phase3", new Vector3(8f, 2.5f, 0f), new Vector3(3f, 0.6f, 1f));
                    FromCellSetupMenu.CreateGroundPlatformPublic("Phase4", new Vector3(14f, 1f, 0f), new Vector3(4f, 0.6f, 1f));
                    FromCellSetupMenu.CreateGroundPlatformPublic("Finish_Platform", new Vector3(22f, 3.5f, 0f), new Vector3(4f, 1f, 1f));

                    var checkpoint = new GameObject("Checkpoint_Final");
                    checkpoint.transform.position = new Vector3(8f, 3.5f, 0f);
                    var cpCol = checkpoint.AddComponent<BoxCollider2D>();
                    cpCol.isTrigger = true;
                    cpCol.size = new Vector2(2f, 2f);
                    checkpoint.AddComponent<Checkpoint>();

                    FromCellSetupMenu.CreateKillZonePublic("Gauntlet_Pit", new Vector3(5f, -3f, 0f), new Vector3(12f, 1f, 1f));
                });
        }
    }
}
#endif
