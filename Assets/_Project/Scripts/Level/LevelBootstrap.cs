using FromCell.Core;
using FromCell.Evolution;
using FromCell.UI;
using UnityEngine;

namespace FromCell.Level
{
    public class LevelBootstrap : MonoBehaviour
    {
        [SerializeField] GameConfig gameConfig;
        [SerializeField] int levelIndex;
        [SerializeField] Transform playerSpawn;
        [SerializeField] GameObject playerPrefab;

        void Start()
        {
            Collectible.ResetCount();

            if (GameFlowSystem.Instance != null)
            {
                if (gameConfig != null)
                    GameFlowSystem.Instance.SetGameConfig(gameConfig);
                else if (GameFlowSystem.Instance.Config != null)
                    gameConfig = GameFlowSystem.Instance.Config;
            }

            LevelData level = ResolveLevelData();
            if (level == null) return;

            ConfigureCompletion(level);
            ApplyEvolution(level);
            ShowTutorial(level);
            SpawnPlayer();
            RefreshHud();
        }

        void ConfigureCompletion(LevelData level)
        {
            var completion = FindFirstObjectByType<LevelCompletionSystem>();
            if (completion == null)
            {
                var go = new GameObject("LevelCompletionSystem");
                completion = go.AddComponent<LevelCompletionSystem>();
            }

            completion.Configure(level);
        }

        void ApplyEvolution(LevelData level)
        {
            var evolution = FindFirstObjectByType<EvolutionSystem>();
            if (evolution == null) return;

            if (gameConfig != null)
                evolution.SetGameConfig(gameConfig);

            evolution.ApplyStage((int)level.stageId);
        }

        void ShowTutorial(LevelData level)
        {
            if (level.tutorialPrompts == null || level.tutorialPrompts.Length == 0) return;

            var banner = FindFirstObjectByType<TutorialBanner>();
            if (banner != null)
                banner.Show(level.tutorialPrompts[0]);
        }

        void RefreshHud()
        {
            var hud = FindFirstObjectByType<GameplayHUD>();
            if (hud != null)
                hud.Refresh();
        }

        LevelData ResolveLevelData()
        {
            if (gameConfig?.levels == null) return null;
            if (levelIndex < 0 || levelIndex >= gameConfig.levels.Length) return null;
            return gameConfig.levels[levelIndex];
        }

        void SpawnPlayer()
        {
            if (playerSpawn == null) return;

            var existing = GameObject.FindGameObjectWithTag("Player");
            if (existing != null)
            {
                existing.transform.position = playerSpawn.position;
                return;
            }

            if (playerPrefab != null)
                Instantiate(playerPrefab, playerSpawn.position, Quaternion.identity);
        }
    }
}
