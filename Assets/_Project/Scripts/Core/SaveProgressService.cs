using UnityEngine;

namespace FromCell.Core
{
    public class SaveProgressService : MonoBehaviour
    {
        public static SaveProgressService Instance { get; private set; }

        const string LastCompletedLevelKey = "fromcell_last_completed_level";
        const string LastStageKey = "fromcell_last_stage";

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public int GetLastCompletedLevel() => PlayerPrefs.GetInt(LastCompletedLevelKey, -1);

        public int GetLastStageIndex() => PlayerPrefs.GetInt(LastStageKey, 0);

        public void SaveProgress(int completedLevelIndex, int stageIndex)
        {
            PlayerPrefs.SetInt(LastCompletedLevelKey, completedLevelIndex);
            PlayerPrefs.SetInt(LastStageKey, stageIndex);
            PlayerPrefs.Save();
        }

        public bool HasSave() => GetLastCompletedLevel() >= 0;

        public void ClearSave()
        {
            PlayerPrefs.DeleteKey(LastCompletedLevelKey);
            PlayerPrefs.DeleteKey(LastStageKey);
            PlayerPrefs.Save();
        }
    }
}
