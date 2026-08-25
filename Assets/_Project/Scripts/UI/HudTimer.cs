using FromCell.Level;
using TMPro;
using UnityEngine;

namespace FromCell.UI
{
    /// <summary>
    /// Reads LevelRunTracker.Instance.ElapsedTime and formats it mm:ss - kept separate from
    /// GameplayHUD so that existing component's field wiring never has to change.
    /// </summary>
    public class HudTimer : MonoBehaviour
    {
        public TextMeshProUGUI timerLabel;

        void Update()
        {
            if (timerLabel == null || LevelRunTracker.Instance == null) return;

            float t = LevelRunTracker.Instance.ElapsedTime;
            int minutes = Mathf.FloorToInt(t / 60f);
            int seconds = Mathf.FloorToInt(t % 60f);
            timerLabel.text = $"{minutes:00}:{seconds:00}";
        }
    }
}
