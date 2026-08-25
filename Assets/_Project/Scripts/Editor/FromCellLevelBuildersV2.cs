#if UNITY_EDITOR
using FromCell.Level;
using UnityEditor;
using UnityEngine;

namespace FromCell.Editor
{
    /// <summary>
    /// Builds levels from LevelBlueprint/LevelCatalog data instead of the imperative
    /// CreateGrayboxLevelNN/BuildLevelNN calls in FromCellSetupMenu.cs/FromCellLevelBuilders.cs
    /// - those are left completely intact as a working rollback path (nothing is removed).
    /// Reuses BuildGrayboxLevelPublic unmodified: the setupEnvironment callback here only
    /// drops a LevelAssembler, so camera/spawn/finish/checkpoint-system/bootstrap wiring is
    /// not duplicated.
    /// </summary>
    static class FromCellLevelBuildersV2
    {
        [MenuItem("From Cell/Setup/Create All Levels (Blueprint)")]
        public static void CreateAllBlueprintLevels()
        {
            int built = 0, skipped = 0;
            for (int i = 0; i < 10; i++)
            {
                var bp = LevelCatalog.Get(i);
                if (bp == null)
                {
                    Debug.LogWarning($"From Cell: no LevelBlueprint authored yet for level index {i} ({FromCellSetupMenu.GetSceneNameForLevel(i)}) - skipped.");
                    skipped++;
                    continue;
                }

                BuildOneBlueprintLevel(i, bp);
                built++;
            }

            Debug.Log($"From Cell: blueprint levels built ({built} built, {skipped} not yet authored).");
        }

        static void BuildOneBlueprintLevel(int levelIndex, LevelBlueprint bp)
        {
            string sceneFileName = FromCellSetupMenu.GetSceneNameForLevel(levelIndex);

            FromCellSetupMenu.BuildGrayboxLevelPublic(
                levelIndex, sceneFileName,
                bp.spawn, bp.finish,
                _ =>
                {
                    var go = new GameObject("LevelAssembler");
                    go.AddComponent<LevelAssembler>().levelIndex = levelIndex;
                });
        }
    }
}
#endif
