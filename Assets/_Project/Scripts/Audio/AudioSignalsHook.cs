using FromCell.Core;
using UnityEngine;

namespace FromCell.Audio
{
    /// <summary>
    /// Subscribes to GameSignals and plays the matching procedural SFX via the existing
    /// AudioManager.PlaySfx(clip) - AudioManager itself is untouched, this is just another
    /// listener alongside PlayerJuice/FxBinder. Subscribe in OnEnable, unsubscribe in
    /// OnDisable per GameSignals' documented lifetime rule.
    /// </summary>
    public class AudioSignalsHook : MonoBehaviour
    {
        void OnEnable()
        {
            GameSignals.PlayerJumped += OnPlayerJumped;
            GameSignals.PlayerDoubleJumped += OnPlayerDoubleJumped;
            GameSignals.PlayerDashed += OnPlayerDashed;
            GameSignals.PlayerRespawned += OnPlayerRespawned;
            GameSignals.CollectiblePicked += OnCollectiblePicked;
            GameSignals.GrowthPickupCollected += OnGrowthPickupCollected;
            GameSignals.CheckpointReached += OnCheckpointReached;
            GameSignals.StageApplied += OnStageApplied;
            GameSignals.LevelCompleted += OnLevelCompleted;
            GameSignals.ChallengeCompleted += OnChallengeCompleted;
        }

        void OnDisable()
        {
            GameSignals.PlayerJumped -= OnPlayerJumped;
            GameSignals.PlayerDoubleJumped -= OnPlayerDoubleJumped;
            GameSignals.PlayerDashed -= OnPlayerDashed;
            GameSignals.PlayerRespawned -= OnPlayerRespawned;
            GameSignals.CollectiblePicked -= OnCollectiblePicked;
            GameSignals.GrowthPickupCollected -= OnGrowthPickupCollected;
            GameSignals.CheckpointReached -= OnCheckpointReached;
            GameSignals.StageApplied -= OnStageApplied;
            GameSignals.LevelCompleted -= OnLevelCompleted;
            GameSignals.ChallengeCompleted -= OnChallengeCompleted;
        }

        void OnPlayerJumped(string context) => Play(SfxBank.Jump);
        void OnPlayerDoubleJumped(string context) => Play(SfxBank.DoubleJump);
        void OnPlayerDashed(string context) => Play(SfxBank.Dash);
        void OnPlayerRespawned(string context) => Play(SfxBank.Respawn);
        void OnCollectiblePicked(string context) => Play(SfxBank.Collect);
        void OnGrowthPickupCollected(string context) => Play(SfxBank.Growth);
        void OnCheckpointReached(string context) => Play(SfxBank.Checkpoint);
        void OnStageApplied(int stageIndex) => Play(SfxBank.Evolution);
        void OnLevelCompleted(int levelIndex) => Play(SfxBank.Finish);

        void OnChallengeCompleted(string encounterId, bool passed, int correctCount, int totalTasks) =>
            Play(passed ? SfxBank.ChallengePass : SfxBank.ChallengeFail);

        static void Play(string key)
        {
            var clip = SfxBank.Get(key);
            AudioManager.Instance?.PlaySfx(clip);
        }
    }
}
