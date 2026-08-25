using FromCell.Core;
using UnityEngine;

namespace FromCell.Level
{
    public class LevelCompletionSystem : MonoBehaviour
    {
        public static LevelCompletionSystem Instance { get; private set; }

        [SerializeField] int requiredCollectibles;
        bool completed;

        void Awake()
        {
            Instance = this;
        }

        public void Configure(LevelData levelData)
        {
            requiredCollectibles = levelData != null ? levelData.requiredCollectibles : 0;
            completed = false;
        }

        public bool TryCompleteLevel()
        {
            if (completed) return false;

            if (Collectible.CollectedCount < requiredCollectibles)
            {
                Debug.Log($"Need {requiredCollectibles} collectibles before finishing.");
                return false;
            }

            completed = true;

            if (GameFlowSystem.Instance != null)
            {
                GameSignals.RaiseLevelCompleted(GameFlowSystem.Instance.CurrentLevelIndex);
                GameFlowSystem.Instance.CompleteLevel();
            }

            return true;
        }
    }
}
