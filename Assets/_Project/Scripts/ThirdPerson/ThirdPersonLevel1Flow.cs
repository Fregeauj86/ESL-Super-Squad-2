using FromCell.Core;
using UnityEngine;
using UnityEngine.UI;

namespace FromCell.ThirdPerson
{
    /// <summary>
    /// Keeps Level 1's completion rules visible in the converted scene without changing the
    /// existing 2D LevelCompletionSystem. It is intentionally level-specific until shared
    /// completion adapters are proven on one representative level.
    /// </summary>
    public class ThirdPersonLevel1Flow : MonoBehaviour
    {
        [SerializeField] ThirdPersonVillainGate villainGate;
        [SerializeField] Text statusText;
        [SerializeField] int requiredCollectibles;

        public bool IsComplete { get; private set; }

        void OnEnable()
        {
            GameSignals.CollectiblePicked += OnCollectiblePicked;
            GameSignals.ChallengeCompleted += OnChallengeCompleted;
        }

        void OnDisable()
        {
            GameSignals.CollectiblePicked -= OnCollectiblePicked;
            GameSignals.ChallengeCompleted -= OnChallengeCompleted;
        }

        void Start()
        {
            int total = FindObjectsByType<ThirdPersonCollectible>(FindObjectsSortMode.None).Length;
            ThirdPersonCollectible.ResetForLevel(total);
            RefreshStatus("Drift toward the glowing exit. Jump unlocks soon.");
        }

        public bool TryFinish()
        {
            if (IsComplete)
                return true;

            if (ThirdPersonCollectible.CollectedCount < requiredCollectibles)
            {
                RefreshStatus($"Collect all vocabulary gems first ({ThirdPersonCollectible.CollectedCount}/{requiredCollectibles}).");
                return false;
            }

            if (villainGate != null && !villainGate.IsPassed)
            {
                RefreshStatus("Pass the Echo Fox English challenge to open the exit.");
                return false;
            }

            IsComplete = true;
            RefreshStatus("Level 1 complete! Echo Fox defeated — First Steps cleared.");
            GameSignals.RaiseLevelCompleted(0);
            SaveProgressService.Instance?.SaveProgress(0, 0);
            return true;
        }

        void OnCollectiblePicked(string context)
        {
            if (!IsComplete)
                RefreshStatus($"Vocabulary gems: {ThirdPersonCollectible.CollectedCount}/{ThirdPersonCollectible.TotalInLevel}");
        }

        void OnChallengeCompleted(string encounterId, bool passed, int correctCount, int totalTasks)
        {
            if (villainGate != null && encounterId == villainGate.EncounterId && passed)
                RefreshStatus("Echo Fox defeated! Reach the glowing exit.");
        }

        void RefreshStatus(string message)
        {
            if (statusText != null)
                statusText.text = message;
        }

        public void Configure(ThirdPersonVillainGate gate, Text status, int collectibleRequirement)
        {
            villainGate = gate;
            statusText = status;
            requiredCollectibles = Mathf.Max(0, collectibleRequirement);
        }
    }
}