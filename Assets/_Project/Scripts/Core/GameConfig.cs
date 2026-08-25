using FromCell.Evolution;
using FromCell.Level;
using UnityEngine;

namespace FromCell.Core
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "From Cell/Game Config")]
    public class GameConfig : ScriptableObject
    {
        public EvolutionStageData[] evolutionStages = new EvolutionStageData[10];
        public LevelData[] levels = new LevelData[10];
    }
}
