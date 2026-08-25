using FromCell.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FromCell.UI
{
    /// <summary>
    /// One level-select grid button. levelIndex is a public field set by
    /// LevelSelectController.Refresh() at runtime, not a captured-index lambda - OnClick()
    /// reads its own instance's field, so the button can still be wired at editor-build time
    /// via UnityEventTools.AddPersistentListener(button.onClick, entry.OnClick), matching
    /// every other button in this project.
    /// </summary>
    public class LevelSelectEntry : MonoBehaviour
    {
        public int levelIndex;
        public TextMeshProUGUI label;
        public TextMeshProUGUI rankLabel;
        public Button button;
        public GameObject lockedOverlay;

        public void Configure(int index, string displayName, string rankText, bool unlocked)
        {
            levelIndex = index;
            if (label != null) label.text = displayName;
            if (rankLabel != null) rankLabel.text = rankText;
            if (button != null) button.interactable = unlocked;
            if (lockedOverlay != null) lockedOverlay.SetActive(!unlocked);
        }

        public void OnClick()
        {
            GameFlowSystem.Instance?.PlayLevel(levelIndex);
        }
    }
}
