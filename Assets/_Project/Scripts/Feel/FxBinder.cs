using FromCell.Core;
using UnityEngine;

namespace FromCell.Feel
{
    /// <summary>
    /// Subscribes to GameSignals and turns gameplay events into Fx bursts / hit-stop, so no
    /// gameplay script needs to know about the Feel system directly - same pattern as
    /// PlayerJuice/PlayerGroundEvents. Subscribe in OnEnable, unsubscribe in OnDisable per
    /// GameSignals' documented lifetime rule.
    /// </summary>
    public class FxBinder : MonoBehaviour
    {
        [SerializeField] Color collectibleColor = new Color(1f, 0.85f, 0.2f);
        [SerializeField] Color checkpointColor = new Color(0.3f, 0.9f, 1f);
        [SerializeField] Color challengePassColor = new Color(0.4f, 1f, 0.4f);
        [SerializeField] Color challengeFailColor = new Color(1f, 0.3f, 0.3f);

        void OnEnable()
        {
            GameSignals.CollectiblePicked += OnCollectiblePicked;
            GameSignals.CheckpointReached += OnCheckpointReached;
            GameSignals.ChallengeCompleted += OnChallengeCompleted;
            GameSignals.PlayerDied += OnPlayerDied;
        }

        void OnDisable()
        {
            GameSignals.CollectiblePicked -= OnCollectiblePicked;
            GameSignals.CheckpointReached -= OnCheckpointReached;
            GameSignals.ChallengeCompleted -= OnChallengeCompleted;
            GameSignals.PlayerDied -= OnPlayerDied;
        }

        void OnCollectiblePicked(string context) => BurstAtPlayer(collectibleColor);
        void OnCheckpointReached(string context) => BurstAtPlayer(checkpointColor);

        void OnChallengeCompleted(string encounterId, bool passed, int correctCount, int totalTasks)
        {
            BurstAtPlayer(passed ? challengePassColor : challengeFailColor, passed ? 10 : 6);
            if (!passed)
                HitStop.Instance?.Trigger(0.06f);
        }

        void OnPlayerDied(string context) => HitStop.Instance?.Trigger(0.05f);

        void BurstAtPlayer(Color color, int count = 8)
        {
            var playerGo = GameObject.FindGameObjectWithTag("Player");
            if (playerGo != null)
                Fx.Burst(playerGo.transform.position, color, count);
        }
    }
}
