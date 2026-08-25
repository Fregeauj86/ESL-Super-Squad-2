using System;

namespace FromCell.Core
{
    /// <summary>
    /// Static event hub - the codebase otherwise has no C# events, everything else is
    /// polling or direct singleton calls. Kept deliberately small and general-purpose.
    ///
    /// IMPORTANT lifetime rule for every subscriber: subscribe in OnEnable, unsubscribe in
    /// OnDisable. These are static events living alongside DontDestroyOnLoad singletons, so
    /// a subscriber that forgets to unsubscribe leaves a stale delegate pointing at a
    /// destroyed scene object across every future scene load - the events keep firing into
    /// nothing and any resulting NullReferenceException is hard to trace back to this file.
    /// EslChallengeController only ever raises these signals and never subscribes, so it is
    /// exempt from this rule.
    /// </summary>
    public static class GameSignals
    {
        public static event Action<string> PlayerDied;
        public static event Action<string> PlayerRespawned;
        public static event Action<string> PlayerJumped;
        public static event Action<string> PlayerDoubleJumped;
        public static event Action<string> PlayerDashed;
        public static event Action<string> PlayerLanded;
        public static event Action<float> PlayerLandedImpact;
        public static event Action<string> CollectiblePicked;
        public static event Action<string> GrowthPickupCollected;
        public static event Action<string> CheckpointReached;
        public static event Action<int> StageApplied;
        public static event Action<int> LevelCompleted;
        public static event Action<string> ChallengeStarted;
        public static event Action<string, bool, int, int> ChallengeCompleted;

        public static void RaisePlayerDied(string context = null) => PlayerDied?.Invoke(context);
        public static void RaisePlayerRespawned(string context = null) => PlayerRespawned?.Invoke(context);
        public static void RaisePlayerJumped(string context = null) => PlayerJumped?.Invoke(context);
        public static void RaisePlayerDoubleJumped(string context = null) => PlayerDoubleJumped?.Invoke(context);
        public static void RaisePlayerDashed(string context = null) => PlayerDashed?.Invoke(context);
        public static void RaisePlayerLanded(string context = null) => PlayerLanded?.Invoke(context);

        /// <summary>impactSpeed: absolute downward velocity (units/sec) at the moment of
        /// landing - lets game-feel systems scale squash/shake/dust to how hard the landing
        /// was, rather than treating every landing identically.</summary>
        public static void RaisePlayerLandedImpact(float impactSpeed) => PlayerLandedImpact?.Invoke(impactSpeed);
        public static void RaiseCollectiblePicked(string context = null) => CollectiblePicked?.Invoke(context);
        public static void RaiseGrowthPickupCollected(string context = null) => GrowthPickupCollected?.Invoke(context);
        public static void RaiseCheckpointReached(string context = null) => CheckpointReached?.Invoke(context);
        public static void RaiseStageApplied(int stageIndex) => StageApplied?.Invoke(stageIndex);
        public static void RaiseLevelCompleted(int levelIndex) => LevelCompleted?.Invoke(levelIndex);
        public static void RaiseChallengeStarted(string encounterId) => ChallengeStarted?.Invoke(encounterId);

        public static void RaiseChallengeCompleted(string encounterId, bool passed, int correctCount, int totalTasks) =>
            ChallengeCompleted?.Invoke(encounterId, passed, correctCount, totalTasks);
    }
}
