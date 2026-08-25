using TMPro;
using UnityEngine;

namespace FromCell.Evolution
{
    public class EvolutionPresenter : MonoBehaviour
    {
        [SerializeField] GameObject overlayRoot;
        [SerializeField] TextMeshProUGUI titleText;
        [SerializeField] TextMeshProUGUI humorText;
        [SerializeField] float displayDuration = 2f;

        float hideTimer;

        void Awake()
        {
            if (overlayRoot != null)
                overlayRoot.SetActive(false);
        }

        void Update()
        {
            if (hideTimer <= 0f) return;

            hideTimer -= Time.unscaledDeltaTime;
            if (hideTimer <= 0f && overlayRoot != null)
                overlayRoot.SetActive(false);
        }

        public void ShowEvolution(EvolutionStageData stage)
        {
            if (overlayRoot == null) return;

            overlayRoot.SetActive(true);
            hideTimer = displayDuration;

            if (titleText != null)
                titleText.text = stage != null ? stage.displayName : "Powered Up!";

            if (humorText != null)
                humorText.text = stage != null ? stage.humorLine : string.Empty;
        }
    }
}
