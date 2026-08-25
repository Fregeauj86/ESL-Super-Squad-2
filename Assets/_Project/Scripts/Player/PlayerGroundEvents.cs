using FromCell.Core;
using UnityEngine;

namespace FromCell.Player
{
    /// <summary>
    /// Detects the exact frame the player lands (GroundChecker.IsGrounded going
    /// false-&gt;true) and raises GameSignals.RaisePlayerLandedImpact with how fast they were
    /// falling, so game-feel systems (PlayerJuice, screen shake, landing dust) can scale to
    /// impact strength. Pure polling, no modification to GroundChecker or PlayerMovement -
    /// the false-&gt;true edge is exact regardless of GroundChecker's own coyote-time grace
    /// period, since coyote time only extends the true-&gt;false tail, never delays reporting
    /// a genuine landing.
    /// </summary>
    public class PlayerGroundEvents : MonoBehaviour
    {
        Rigidbody2D rb;
        GroundChecker groundChecker;
        bool wasGrounded;
        float lastFallSpeed;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            groundChecker = GetComponent<GroundChecker>();
        }

        void Update()
        {
            if (groundChecker == null || rb == null) return;

            bool isGrounded = groundChecker.IsGrounded;

            if (!isGrounded && rb.linearVelocity.y < 0f)
                lastFallSpeed = -rb.linearVelocity.y;

            if (isGrounded && !wasGrounded)
                GameSignals.RaisePlayerLandedImpact(lastFallSpeed);

            wasGrounded = isGrounded;
        }
    }
}
