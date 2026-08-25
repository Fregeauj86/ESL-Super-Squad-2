using FromCell.Feel;
using UnityEngine;

namespace FromCell.Level.Enemy
{
    /// <summary>
    /// Shared base for every enemy movement type - owns the Rigidbody2D and the shared
    /// death behavior (EnemyHitbox calls Die() on a stomp), leaves only Move() to each
    /// subtype. Matches this project's no-coroutine convention: FixedUpdate-driven, no
    /// ParticleSystem/Animator (uses the existing Feel.Fx burst instead).
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class EnemyBrain : MonoBehaviour
    {
        [SerializeField] Color deathBurstColor = new Color(0.8f, 0.3f, 0.3f);

        protected Rigidbody2D rb;

        public bool IsDead { get; private set; }

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        protected virtual void FixedUpdate()
        {
            if (IsDead) return;
            Move();
        }

        protected abstract void Move();

        public virtual void Die()
        {
            if (IsDead) return;
            IsDead = true;

            rb.linearVelocity = Vector2.zero;
            var col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            Fx.Burst(transform.position, deathBurstColor);
            Destroy(gameObject, 0.05f);
        }
    }
}
