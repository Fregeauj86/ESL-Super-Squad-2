using FromCell.ESL;
using FromCell.UI;
using UnityEngine;

namespace FromCell.Feel
{
    /// <summary>
    /// Brief global freeze-frame on impact, done by dropping Time.timeScale to a near-zero
    /// value for a short unscaled-time duration and restoring it. Refuses to trigger while
    /// PauseManager or EslChallengeController already owns timeScale (both set it to exactly
    /// 0) - triggering on top of that would restore it to 1 afterward and unpause the game
    /// out from under them.
    /// </summary>
    public class HitStop : MonoBehaviour
    {
        public static HitStop Instance { get; private set; }

        [SerializeField] float slowScale = 0.03f;

        float remainingUnscaled;
        float previousTimeScale = 1f;
        bool active;

        void Awake() => Instance = this;

        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        void Update()
        {
            if (!active) return;

            remainingUnscaled -= Time.unscaledDeltaTime;
            if (remainingUnscaled <= 0f)
            {
                active = false;
                Time.timeScale = previousTimeScale;
            }
        }

        public void Trigger(float durationUnscaled)
        {
            bool suspended = (PauseManager.Instance != null && PauseManager.Instance.IsPaused)
                || (EslChallengeController.Instance != null && EslChallengeController.Instance.IsActive);
            if (suspended) return;

            if (!active)
                previousTimeScale = Time.timeScale;

            active = true;
            remainingUnscaled = durationUnscaled;
            Time.timeScale = slowScale;
        }
    }
}
