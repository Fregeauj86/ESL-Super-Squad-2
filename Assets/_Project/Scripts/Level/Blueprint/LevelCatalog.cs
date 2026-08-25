namespace FromCell.Level
{
    /// <summary>
    /// Static lookup from level index (0-9) to its authored LevelBlueprint. Levels are
    /// authored incrementally as separate files under Level/Blueprint/Levels/ (Phase 6) -
    /// Get() returns null for any index not yet authored, and LevelAssembler treats that as
    /// "nothing to build" (a no-op) rather than an error, so the pipeline works end-to-end
    /// before all 10 levels exist.
    /// </summary>
    public static class LevelCatalog
    {
        public static LevelBlueprint Get(int levelIndex)
        {
            switch (levelIndex)
            {
                case 0: return Level01.Build();
                case 1: return Level02.Build();
                case 2: return Level03.Build();
                case 3: return Level04.Build();
                case 4: return Level05.Build();
                case 5: return Level06.Build();
                case 6: return Level07.Build();
                case 7: return Level08.Build();
                case 8: return Level09.Build();
                case 9: return Level10.Build();
                default: return null;
            }
        }

        public static LevelBlueprint[] All()
        {
            var list = new System.Collections.Generic.List<LevelBlueprint>();
            for (int i = 0; i < 10; i++)
            {
                var bp = Get(i);
                if (bp != null) list.Add(bp);
            }
            return list.ToArray();
        }
    }
}
