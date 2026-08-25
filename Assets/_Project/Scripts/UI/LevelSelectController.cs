using FromCell.Core;
using FromCell.Level;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FromCell.UI
{
    /// <summary>
    /// Populates the pre-built, fixed-size pool of LevelSelectEntry buttons (matching this
    /// project's "no runtime prefab instantiation" convention) from GameConfig - one entry
    /// per level, unlocked up to SaveProgressService's last-completed-level + 1, showing each
    /// level's saved rank from SaveProfile if it has a completed record.
    /// </summary>
    public class LevelSelectController : MonoBehaviour
    {
        public GameConfig gameConfig;
        public LevelSelectEntry[] entries = System.Array.Empty<LevelSelectEntry>();
        [SerializeField] string mainMenuScene = "_MainMenu";

        void Start() => Refresh();

        public void Refresh()
        {
            if (gameConfig?.levels == null) return;

            int lastCompleted = SaveProgressService.Instance != null
                ? SaveProgressService.Instance.GetLastCompletedLevel()
                : -1;

            for (int i = 0; i < entries.Length; i++)
            {
                if (entries[i] == null || i >= gameConfig.levels.Length) continue;

                var level = gameConfig.levels[i];
                string displayName = level != null ? level.displayName : $"Level {i + 1}";
                bool unlocked = i <= lastCompleted + 1;

                string rankText = "-";
                var record = SaveProfile.Instance != null ? SaveProfile.Instance.GetRecord(i) : null;
                if (record != null && record.completed && level != null)
                {
                    var rank = RankCalculator.Calculate(record.bestTimeSeconds, record.deaths, level.targetDurationMinutes, record.anyChallengeFailed);
                    rankText = RankCalculator.Label(rank);
                }

                entries[i].Configure(i, displayName, rankText, unlocked);
            }
        }

        public void OnBack()
        {
            if (GameFlowSystem.Instance != null)
                GameFlowSystem.Instance.ReturnToMainMenu();
            else
                SceneManager.LoadScene(mainMenuScene);
        }
    }
}
