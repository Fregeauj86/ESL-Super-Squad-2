using FromCell.Core;
using UnityEngine;

namespace FromCell.Player
{
    /// <summary>
    /// Squash-and-stretch on landing/jump/dash, applied to the child "Visual" transform
    /// (set by FromCellSetupMenu.CreatePlayerGameObject) - never the player root, so it can
    /// never distort the CapsuleCollider2D or GroundCheck's child offset. No coroutine: a
    /// target squash factor is set on each triggering event, then eased back toward
    /// Vector2.one every Update().
    /// </summary>
    public class PlayerJuice : MonoBehaviour
    {
        public Transform visualTransform;

        [SerializeField] float recoverySpeed = 10f;
        [SerializeField] float landingImpactThreshold = 3f;
        [SerializeField] float maxLandingSquash = 0.35f;
        [SerializeField] float jumpStretch = 0.22f;
        [SerializeField] float dashStretch = 0.18f;

        Vector2 squash = Vector2.one;

        void OnEnable()
        {
            GameSignals.PlayerLandedImpact += OnLandedImpact;
            GameSignals.PlayerJumped += OnJumped;
            GameSignals.PlayerDoubleJumped += OnJumped;
            GameSignals.PlayerDashed += OnDashed;
        }

        void OnDisable()
        {
            GameSignals.PlayerLandedImpact -= OnLandedImpact;
            GameSignals.PlayerJumped -= OnJumped;
            GameSignals.PlayerDoubleJumped -= OnJumped;
            GameSignals.PlayerDashed -= OnDashed;
        }

        void Update()
        {
            if (visualTransform == null) return;

            squash = Vector2.Lerp(squash, Vector2.one, Time.deltaTime * recoverySpeed);
            visualTransform.localScale = new Vector3(squash.x, squash.y, 1f);
        }

        void OnLandedImpact(float impactSpeed)
        {
            if (impactSpeed < landingImpactThreshold) return;

            float amount = Mathf.Clamp01((impactSpeed - landingImpactThreshold) / 10f) * maxLandingSquash;
            squash = new Vector2(1f + amount, 1f - amount);
        }

        void OnJumped(string context)
        {
            squash = new Vector2(1f - jumpStretch, 1f + jumpStretch);
        }

        void OnDashed(string context)
        {
            squash = new Vector2(1f + dashStretch, 1f - dashStretch * 0.5f);
        }
    }
}
