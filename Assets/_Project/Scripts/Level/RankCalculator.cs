namespace FromCell.Level
{
    /// <summary>
    /// Pure function from a level's run stats to a display rank - no MonoBehaviour/Unity
    /// object access, so the same thresholds are used by both ResultsScreen (right after a
    /// run) and LevelSelectController (reading a saved record) without duplicating them.
    /// </summary>
    public static class RankCalculator
    {
        public enum Rank { None, C, B, A, S }

        public static Rank Calculate(float timeSeconds, int deaths, int targetDurationMinutes, bool anyChallengeFailed)
        {
            if (anyChallengeFailed) return Rank.C;

            float targetSeconds = targetDurationMinutes * 60f;
            if (deaths == 0 && timeSeconds <= targetSeconds)
                return Rank.S;
            if (deaths <= 1 && timeSeconds <= targetSeconds * 1.5f)
                return Rank.A;
            if (deaths <= 3)
                return Rank.B;
            return Rank.C;
        }

        public static string Label(Rank rank)
        {
            switch (rank)
            {
                case Rank.S: return "S";
                case Rank.A: return "A";
                case Rank.B: return "B";
                case Rank.C: return "C";
                default: return "-";
            }
        }
    }
}
