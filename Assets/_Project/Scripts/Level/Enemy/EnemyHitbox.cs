using FromCell.Player;
using UnityEngine;

namespace FromCell.Level.Enemy
{
    /// <summary>
    /// Player-contact rules for an enemy: landing on top (stomp) kills the enemy and bounces
    /// the player upward; touching from the side/below funnels into the existing
    /// PlayerHealth.Die() - the same death/respawn path hazards already use, no separate
    /// damage system. Stomp detection is bounds-based (player's collider bottom at/above this
    /// hitbox's collider center), matching OneWayPlatform/MovingPlatform's rider checks rather
    /// than relying on Collision2D contact normal direction.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class EnemyHitbox : MonoBehaviour
    {
        [SerializeField] float stompBounceForce = 8f;

        EnemyBrain brain;
        Collider2D hitboxCollider;

        void Awake()
        {
            brain = GetComponentInParent<EnemyBrain>();
            hitboxCollider = GetComponent<Collider2D>();
        }

        void OnTriggerEnter2D(Collider2D other) => HandlePlayerContact(other);
        void OnCollisionEnter2D(Collision2D collision) => HandlePlayerContact(collision.collider);

        void HandlePlayerContact(Collider2D playerCollider)
        {
            if (brain != null && brain.IsDead) return;
            if (!playerCollider.CompareTag("Player")) return;

            bool isStomp = playerCollider.bounds.min.y >= hitboxCollider.bounds.center.y;

            if (isStomp)
            {
                brain?.Die();
                var rb = playerCollider.attachedRigidbody;
                if (rb != null)
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, stompBounceForce);
            }
            else
            {
                playerCollider.GetComponent<PlayerHealth>()?.Die();
            }
        }
    }
}
