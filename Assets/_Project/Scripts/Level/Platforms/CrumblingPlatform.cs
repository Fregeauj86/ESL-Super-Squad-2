using UnityEngine;

namespace FromCell.Level.Platforms
{
    /// <summary>
    /// Solid until the player lands on it, then disables its collider/visual after a short
    /// delay and re-enables both after a respawn delay - Invoke-based like every other timer
    /// in this codebase, no coroutine.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class CrumblingPlatform : MonoBehaviour
    {
        [SerializeField] float crumbleDelay = 0.5f;
        [SerializeField] float respawnDelay = 3f;

        Collider2D platformCollider;
        SpriteRenderer sr;
        bool triggered;

        void Awake()
        {
            platformCollider = GetComponent<Collider2D>();
            sr = GetComponent<SpriteRenderer>();
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (triggered || !collision.gameObject.CompareTag("Player")) return;

            triggered = true;
            Invoke(nameof(Crumble), crumbleDelay);
        }

        void Crumble()
        {
            platformCollider.enabled = false;
            if (sr != null) sr.enabled = false;
            Invoke(nameof(Respawn), respawnDelay);
        }

        void Respawn()
        {
            platformCollider.enabled = true;
            if (sr != null) sr.enabled = true;
            triggered = false;
        }
    }
}
