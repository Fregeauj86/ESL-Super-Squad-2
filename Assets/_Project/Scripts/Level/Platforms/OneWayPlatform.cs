using UnityEngine;

namespace FromCell.Level.Platforms
{
    /// <summary>
    /// Solid from above, passable from below/the side - no PlatformEffector2D (unverifiable
    /// asset-config behavior without running the editor); instead toggles
    /// Physics2D.IgnoreCollision against the player directly, driven by comparing the
    /// player's collider bottom against this platform's collider top each frame. Single-player
    /// only (this game has exactly one player), matching CameraFollow2D's lazy tag-find.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class OneWayPlatform : MonoBehaviour
    {
        [SerializeField] float surfaceMargin = 0.05f;

        Collider2D platformCollider;
        Collider2D playerCollider;
        bool passable;

        void Awake()
        {
            platformCollider = GetComponent<Collider2D>();
        }

        void Update()
        {
            if (playerCollider == null)
            {
                var playerGo = GameObject.FindGameObjectWithTag("Player");
                if (playerGo == null) return;
                playerCollider = playerGo.GetComponent<Collider2D>();
                if (playerCollider == null) return;
            }

            bool shouldBePassable = playerCollider.bounds.min.y < platformCollider.bounds.max.y - surfaceMargin;

            if (shouldBePassable != passable)
            {
                passable = shouldBePassable;
                Physics2D.IgnoreCollision(platformCollider, playerCollider, passable);
            }
        }
    }
}
