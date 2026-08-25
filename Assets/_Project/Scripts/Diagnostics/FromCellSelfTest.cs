using System.Collections.Generic;
using System.Linq;
using FromCell.Art;
using FromCell.Core;
using FromCell.ESL;
using FromCell.Level;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FromCell.Diagnostics
{
    /// <summary>
    /// PASS/FAIL self-check for setup mistakes that would otherwise only surface as a silent
    /// missing sprite, a broken level, or a crash deep in gameplay. Deliberately in a plain
    /// runtime folder (not Editor/) so the [RuntimeInitializeOnLoadMethod] entry point still
    /// runs in real builds, not just the editor - the [MenuItem] entry point is wrapped in
    /// #if UNITY_EDITOR and gets stripped from player builds automatically, same as any other
    /// editor-only code living outside an Editor/ folder.
    ///
    /// Two entry points sharing the same runtime-safe checks (GameConfig array lengths, all
    /// 10 level blueprints + ESL catalog validating, every ArtKeys sprite actually resolving
    /// to a baked sprite rather than a placeholder fallback):
    ///  - RuntimeInitializeOnLoadMethod runs once whenever the game boots (editor Play mode
    ///    AND real builds alike) - logs PASS/FAIL to the console, never blocks play.
    ///  - The editor menu item additionally checks tags/layers/the GameConfig asset itself
    ///    existing (things only AssetDatabase/UnityEditorInternal can see), so a forgotten
    ///    "Run Full Prototype Setup" step is caught before the game has even been played once.
    /// </summary>
    public static class FromCellSelfTest
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void RuntimeCheck()
        {
            var (errors, warnings) = RunRuntimeSafeChecks();
            Report("From Cell Self-Test (runtime)", errors, warnings);
        }

#if UNITY_EDITOR
        [MenuItem("From Cell/Setup/Run Self-Test")]
        public static void EditorCheck()
        {
            var (errors, warnings) = RunRuntimeSafeChecks();
            RunEditorOnlyChecks(errors, warnings);
            Report("From Cell Self-Test (editor)", errors, warnings);
        }

        static void RunEditorOnlyChecks(List<string> errors, List<string> warnings)
        {
            foreach (var tag in new[] { "Player", "Ground", "Enemy" })
                if (!UnityEditorInternal.InternalEditorUtility.tags.Contains(tag))
                    errors.Add($"Tag '{tag}' is not registered - run From Cell > Setup > Run Full Prototype Setup.");

            foreach (var layer in new[] { "Player", "Ground", "Hazard", "Trigger", "Enemy" })
                if (LayerMask.NameToLayer(layer) < 0)
                    errors.Add($"Layer '{layer}' is not registered - run From Cell > Setup > Run Full Prototype Setup.");

            var config = AssetDatabase.LoadAssetAtPath<GameConfig>("Assets/_Project/ScriptableObjects/GameConfig.asset");
            if (config == null)
                errors.Add("GameConfig.asset not found - run From Cell > Setup > Create Default Game Data.");
        }
#endif

        static (List<string> errors, List<string> warnings) RunRuntimeSafeChecks()
        {
            var errors = new List<string>();
            var warnings = new List<string>();

            CheckGameConfig(errors, warnings);
            CheckLevels(errors, warnings);
            CheckArtKeys(warnings);

            return (errors, warnings);
        }

        static void CheckGameConfig(List<string> errors, List<string> warnings)
        {
            var config = GameFlowSystem.Instance != null ? GameFlowSystem.Instance.Config : null;
            if (config == null)
            {
                warnings.Add("GameConfig not loaded yet (no GameFlowSystem in this scene) - skipped the array-length check.");
                return;
            }

            if (config.evolutionStages == null || config.evolutionStages.Length != 10)
                errors.Add($"GameConfig.evolutionStages should have 10 entries, has {config.evolutionStages?.Length ?? 0}.");
            if (config.levels == null || config.levels.Length != 10)
                errors.Add($"GameConfig.levels should have 10 entries, has {config.levels?.Length ?? 0}.");
        }

        static void CheckLevels(List<string> errors, List<string> warnings)
        {
            var (eslErrors, eslWarnings) = EslContentValidator.Validate(EslContentCatalog.All);
            errors.AddRange(eslErrors);
            warnings.AddRange(eslWarnings);

            for (int i = 0; i < 10; i++)
            {
                var bp = LevelCatalog.Get(i);
                if (bp == null)
                {
                    warnings.Add($"Level {i}: no blueprint authored.");
                    continue;
                }

                var (lvlErrors, lvlWarnings) = LevelBlueprintValidator.Validate(bp, EslContentCatalog.All);
                errors.AddRange(lvlErrors);
                warnings.AddRange(lvlWarnings);
            }
        }

        static void CheckArtKeys(List<string> warnings)
        {
            foreach (var (key, _) in ArtKeys.AllSourceSprites)
            {
                var sprite = Resources.Load<Sprite>("FromCell/" + key);
                if (sprite == null)
                    warnings.Add($"ArtKeys '{key}' has no baked sprite yet (falls back to a placeholder) - run From Cell > Setup > Bake Character Art.");
            }
        }

        static void Report(string label, List<string> errors, List<string> warnings)
        {
            foreach (var w in warnings) Debug.LogWarning($"{label}: {w}");
            foreach (var e in errors) Debug.LogError($"{label}: {e}");

            if (errors.Count == 0)
                Debug.Log($"{label}: PASS ({warnings.Count} warning(s)).");
            else
                Debug.LogError($"{label}: FAIL - {errors.Count} error(s), {warnings.Count} warning(s).");
        }
    }
}
