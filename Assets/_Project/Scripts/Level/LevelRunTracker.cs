using FromCell.Core;
using UnityEngine;

namespace FromCell.Level
{
    /// <summary>
    /// Per-level run stats (elapsed time, death count) for the not-yet-built Phase 7 results/
    /// level-select screens - not consumed by any UI yet, but wired to GameSignals now so the
    /// data already exists when that UI is built. Scene-local singleton, same lifetime pattern
    /// as CheckpointSystem/LevelCompletionSystem. Timer runs on unscaled time so pausing (or an
    /// ESL challenge, which also zeroes Time.timeScale) doesn't inflate the recorded run time.
    /// </summary>
    public class LevelRunTracker : MonoBehaviour
    {
        public static LevelRunTracker Instance { get; private set; }

        public float ElapsedTime { get; private set; }
        public int DeathCount { get; private set; }
        public bool IsRunning { get; private set; } = true;

        void Awake() => Instance = this;

        void OnEnable()
        {
            GameSignals.PlayerDied += OnPlayerDied;
            GameSignals.LevelCompleted += OnLevelCompleted;
        }

        void OnDisable()
        {
            GameSignals.PlayerDied -= OnPlayerDied;
            GameSignals.LevelCompleted -= OnLevelCompleted;
        }

        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        void Update()
        {
            if (IsRunning)
                ElapsedTime += Time.unscaledDeltaTime;
        }

        void OnPlayerDied(string context) => DeathCount++;
        void OnLevelCompleted(int levelIndex) => IsRunning = false;
    }
}
