namespace FromCell.Core
{
    public enum EvolutionStageId
    {
        Cell = 0,
        Cluster = 1,
        Organism = 2,
        Primitive = 3,
        Embryo = 4,
        Nervous = 5,
        Newborn = 6,
        Child = 7,
        Teen = 8,
        Adult = 9
    }

    public enum MovementMode
    {
        Float,
        Crawl,
        Walk
    }

    public enum GameFlowState
    {
        Boot,
        MainMenu,
        LevelLoad,
        Playing,
        Paused,
        PlayerDead,
        LevelComplete,
        Evolution,
        Credits
    }
}
