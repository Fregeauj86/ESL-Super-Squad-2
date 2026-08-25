using UnityEngine;

namespace FromCell.Player
{
    public class GroundChecker : MonoBehaviour
    {
        [SerializeField] Transform groundCheckPoint;
        [SerializeField] float checkRadius = 0.2f;
        [SerializeField] LayerMask groundLayers = ~0;
        [SerializeField] string groundTag = "Ground";

        public bool IsGrounded { get; private set; }
        public float CoyoteTime { get; set; } = 0.12f;
        float coyoteTimer;

        // OverlapCircle returns a single ARBITRARY overlapping collider, and
        // Physics2D.queriesHitTriggers defaults to true - so any trigger (a
        // pickup, checkpoint, wind zone, enemy hitbox) overlapping this point
        // could be the one returned, making the player wrongly report airborne
        // while standing on solid ground. Check every overlap and accept the
        // first Ground-tagged one instead of trusting whichever comes back first.
        readonly Collider2D[] overlapBuffer = new Collider2D[8];

        void Update()
        {
            bool groundedNow = CheckGrounded();

            if (groundedNow)
            {
                IsGrounded = true;
                coyoteTimer = CoyoteTime;
            }
            else
            {
                coyoteTimer -= Time.deltaTime;
                IsGrounded = coyoteTimer > 0f;
            }
        }

        bool CheckGrounded()
        {
            Vector2 origin = groundCheckPoint != null
                ? groundCheckPoint.position
                : transform.position;

            int count = Physics2D.OverlapCircleNonAlloc(origin, checkRadius, overlapBuffer, groundLayers);
            for (int i = 0; i < count; i++)
            {
                Collider2D hit = overlapBuffer[i];
                if (hit == null) continue;
                if (string.IsNullOrEmpty(groundTag) || hit.CompareTag(groundTag))
                    return true;
            }
            return false;
        }

        void OnDrawGizmosSelected()
        {
            Vector2 origin = groundCheckPoint != null
                ? groundCheckPoint.position
                : transform.position;

            Gizmos.color = IsGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(origin, checkRadius);
        }
    }
}
