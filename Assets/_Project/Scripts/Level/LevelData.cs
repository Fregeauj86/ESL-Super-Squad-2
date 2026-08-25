using FromCell.Core;
using UnityEngine;

namespace FromCell.Level
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "From Cell/Level Data")]
    public class LevelData : ScriptableObject
    {
        public int levelIndex;
        public string sceneName;
        public EvolutionStageId stageId;
        public string displayName;
        [Min(1)] public int targetDurationMinutes = 3;
        public int requiredCollectibles;
        [TextArea] public string[] tutorialPrompts;
    }
}
