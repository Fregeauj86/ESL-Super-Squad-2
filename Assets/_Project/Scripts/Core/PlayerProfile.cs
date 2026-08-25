using System;

namespace FromCell.Core
{
    /// <summary>
    /// One level's best-run record - plain data, JSON-serialized by SaveProfile via
    /// JsonUtility. bestTimeSeconds stays -1 until the level has been completed at least
    /// once (JsonUtility can't represent "no value" for a float, so -1 is the explicit
    /// not-yet-set sentinel RankCalculator/LevelSelectController check for).
    /// </summary>
    [Serializable]
    public class LevelRecord
    {
        public bool completed;
        public float bestTimeSeconds = -1f;
        public int deaths;
        public bool anyChallengeFailed;
    }

    /// <summary>
    /// The full save profile - one LevelRecord per level, fixed at 10 entries (matching
    /// GameConfig's fixed 10 levels). Wrapped in a class (not a bare array) because
    /// JsonUtility can't serialize a top-level array directly.
    /// </summary>
    [Serializable]
    public class PlayerProfile
    {
        public LevelRecord[] levels;

        public PlayerProfile()
        {
            levels = new LevelRecord[10];
            for (int i = 0; i < levels.Length; i++)
                levels[i] = new LevelRecord();
        }
    }
}
