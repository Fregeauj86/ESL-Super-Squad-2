using FromCell.Core;
using FromCell.Level;
using TMPro;
using UnityEngine;

namespace FromCell.UI
{
    /// <summary>
    /// Shows a brief stats readout (time/deaths) when GameSignals.LevelCompleted fires -
    /// purely informational, layered next to the existing EvolutionPresenter overlay rather
    /// than replacing anything about it. Does not touch GameFlowSystem's auto-advance timing
    /// (evolutionDelay keeps working exactly as before); this panel just gets destroyed for
    /// free on the next scene load, same as everything else in the level scene.
    /// </summary>
    public class ResultsScreen : MonoBehaviour
    {
        public GameObject overlayRoot;
        public TextMeshProUGUI statsText;

        void Awake()
        {
            if (overlayRoot != null)
                overlayRoot.SetActive(false);
        }

        void OnEnable() => GameSignals.LevelCompleted += OnLevelCompleted;
        void OnDisable() => GameSignals.LevelCompleted -= OnLevelCompleted;

        void OnLevelCompleted(int levelIndex)
        {
            if (overlayRoot != null)
                overlayRoot.SetActive(true);

            if (statsText == null) return;

            float time = LevelRunTracker.Instance != null ? LevelRunTracker.Instance.ElapsedTime : 0f;
            int deaths = LevelRunTracker.Instance != null ? LevelRunTracker.Instance.DeathCount : 0;
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);

            statsText.text = $"Time: {minutes:00}:{seconds:00}\nDeaths: {deaths}";
        }
    }
}
