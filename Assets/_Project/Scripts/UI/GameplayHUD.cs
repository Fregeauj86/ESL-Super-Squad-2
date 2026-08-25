using FromCell.Core;
using FromCell.Evolution;
using FromCell.Level;
using TMPro;
using UnityEngine;

namespace FromCell.UI
{
    public class GameplayHUD : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI stageLabel;
        [SerializeField] TextMeshProUGUI levelLabel;
        [SerializeField] TextMeshProUGUI collectiblesLabel;

        void Start() => Refresh();

        void Update()
        {
            if (collectiblesLabel != null)
                collectiblesLabel.text = $"Vocabulary Gems: {Collectible.CollectedCount}";
        }

        public void Refresh()
        {
            var evolution = FindFirstObjectByType<EvolutionSystem>();
            var flow = GameFlowSystem.Instance;

            if (stageLabel != null)
            {
                var stage = evolution?.CurrentStageData;
                stageLabel.text = stage != null ? stage.displayName : string.Empty;
            }

            if (levelLabel != null && flow?.Config?.levels != null)
            {
                int index = flow.CurrentLevelIndex;
                if (index >= 0 && index < flow.Config.levels.Length && flow.Config.levels[index] != null)
                    levelLabel.text = flow.Config.levels[index].displayName;
            }
        }
    }
}
