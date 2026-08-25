using UnityEngine;
using UnityEngine.SceneManagement;

namespace FromCell.Core
{
    /// <summary>
    /// New, additive save data layered next to SaveProgressService's 2 legacy PlayerPrefs
    /// keys (last-completed-level, last-stage) - never reads, writes, or replaces either of
    /// them, so existing continue/resume behavior is completely untouched. Stores one JSON
    /// blob under its own key. Subscribes to GameSignals.ChallengeCompleted to track whether
    /// any villain encounter was failed during the current level attempt, and
    /// GameFlowSystem.CompleteLevel() calls RecordLevelComplete() to fold that plus the run's
    /// time/deaths into the saved record.
    /// </summary>
    public class SaveProfile : MonoBehaviour
    {
        public static SaveProfile Instance { get; private set; }

        const string ProfileKey = "fromcell_profile_v1";

        PlayerProfile profile;
        bool challengeFailedThisAttempt;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }

        void OnEnable()
        {
            GameSignals.ChallengeCompleted += OnChallengeCompleted;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnDisable()
        {
            GameSignals.ChallengeCompleted -= OnChallengeCompleted;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // A failed challenge should only count against the level attempt it happened in - if
        // the player dies/restarts and retries without ever completing, this must not bleed
        // into a later, actually-successful completion (of this level or any other).
        void OnSceneLoaded(Scene scene, LoadSceneMode mode) => challengeFailedThisAttempt = false;

        void OnChallengeCompleted(string encounterId, bool passed, int correctCount, int totalTasks)
        {
            if (!passed) challengeFailedThisAttempt = true;
        }

        public LevelRecord GetRecord(int levelIndex)
        {
            if (profile?.levels == null || levelIndex < 0 || levelIndex >= profile.levels.Length) return null;
            return profile.levels[levelIndex];
        }

        public void RecordLevelComplete(int levelIndex, float timeSeconds, int deaths)
        {
            if (profile?.levels == null || levelIndex < 0 || levelIndex >= profile.levels.Length) return;

            var record = profile.levels[levelIndex];
            record.completed = true;
            record.deaths = deaths;
            record.anyChallengeFailed = challengeFailedThisAttempt;
            if (record.bestTimeSeconds < 0f || timeSeconds < record.bestTimeSeconds)
                record.bestTimeSeconds = timeSeconds;

            challengeFailedThisAttempt = false;
            Save();
        }

        void Load()
        {
            string json = PlayerPrefs.GetString(ProfileKey, string.Empty);
            profile = string.IsNullOrEmpty(json) ? new PlayerProfile() : JsonUtility.FromJson<PlayerProfile>(json);
            if (profile == null || profile.levels == null || profile.levels.Length != 10)
                profile = new PlayerProfile();
        }

        void Save()
        {
            PlayerPrefs.SetString(ProfileKey, JsonUtility.ToJson(profile));
            PlayerPrefs.Save();
        }
    }
}
