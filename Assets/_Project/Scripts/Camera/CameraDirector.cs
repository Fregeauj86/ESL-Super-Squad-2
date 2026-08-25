using FromCell.Core;
using FromCell.Feel;
using UnityEngine;

namespace FromCell.Cameras
{
    /// <summary>
    /// Adds look-ahead, level-bounds clamping, screen shake and zoom easing on top of
    /// CameraFollow2D's smooth-follow position. [DefaultExecutionOrder(100)] guarantees this
    /// component's LateUpdate runs AFTER CameraFollow2D's (default order 0), so it always
    /// starts from the already-followed position rather than racing it. Its own Update() (which
    /// always runs before any LateUpdate regardless of script order) first undoes last frame's
    /// shake offset, so CameraFollow2D's SmoothDamp never sees the shaken position as its
    /// target/velocity basis - shake stays a transient visual jitter, not a permanent drift.
    /// </summary>
    [DefaultExecutionOrder(100)]
    public class CameraDirector : MonoBehaviour
    {
        [SerializeField] float lookAheadDistance = 1.5f;
        [SerializeField] float lookAheadSmoothTime = 0.25f;
        [SerializeField] float baseOrthoSize = 5f;
        [SerializeField] float zoomedOrthoSize = 6f;
        [SerializeField] float zoomSmoothTime = 0.3f;

        UnityEngine.Camera cam;
        Rigidbody2D playerRb;

        float minWorldX = float.NegativeInfinity;
        float maxWorldX = float.PositiveInfinity;

        Vector3 lastShakeOffset;
        float lookAheadX;
        float lookAheadVelocity;
        float zoomVelocity;
        bool zoomedOut;

        void Awake()
        {
            cam = GetComponent<UnityEngine.Camera>();
        }

        void OnEnable()
        {
            GameSignals.PlayerLandedImpact += OnLandedImpact;
            GameSignals.PlayerDied += OnPlayerDied;
            GameSignals.ChallengeCompleted += OnChallengeCompleted;
        }

        void OnDisable()
        {
            GameSignals.PlayerLandedImpact -= OnLandedImpact;
            GameSignals.PlayerDied -= OnPlayerDied;
            GameSignals.ChallengeCompleted -= OnChallengeCompleted;
        }

        void Update()
        {
            transform.position -= lastShakeOffset;
            lastShakeOffset = Vector3.zero;

            if (playerRb == null)
            {
                var playerGo = GameObject.FindGameObjectWithTag("Player");
                if (playerGo != null)
                    playerRb = playerGo.GetComponent<Rigidbody2D>();
            }

            float targetLookAhead = 0f;
            if (playerRb != null)
                targetLookAhead = Mathf.Clamp(playerRb.linearVelocity.x, -1f, 1f) * lookAheadDistance;
            lookAheadX = Mathf.SmoothDamp(lookAheadX, targetLookAhead, ref lookAheadVelocity, lookAheadSmoothTime);
        }

        void LateUpdate()
        {
            Vector3 pos = transform.position;
            pos.x += lookAheadX;
            pos.x = Mathf.Clamp(pos.x, minWorldX, maxWorldX);

            Vector3 shakeOffset = ScreenShake.Instance != null ? ScreenShake.Instance.CurrentOffset : Vector3.zero;
            transform.position = pos + shakeOffset;
            lastShakeOffset = shakeOffset;

            if (cam != null && cam.orthographic)
            {
                float targetSize = zoomedOut ? zoomedOrthoSize : baseOrthoSize;
                cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetSize, ref zoomVelocity, zoomSmoothTime);
            }
        }

        public void SetZoomedOut(bool value) => zoomedOut = value;

        public void SetWorldBounds(float minX, float maxX)
        {
            minWorldX = minX;
            maxWorldX = maxX;
        }

        void OnLandedImpact(float impactSpeed)
        {
            if (impactSpeed > 6f)
                ScreenShake.Instance?.Trigger(0.25f);
        }

        void OnPlayerDied(string context) => ScreenShake.Instance?.Trigger(0.5f);

        void OnChallengeCompleted(string encounterId, bool passed, int correctCount, int totalTasks)
        {
            if (!passed)
                ScreenShake.Instance?.Trigger(0.3f);
        }
    }
}
