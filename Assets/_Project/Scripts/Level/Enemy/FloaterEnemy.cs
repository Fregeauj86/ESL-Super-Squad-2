using UnityEngine;

namespace FromCell.Level.Enemy
{
    /// <summary>
    /// Hovers on a sine-wave vertical bob while drifting back and forth horizontally - gravity
    /// disabled (this is a flying enemy, not a walker), Y set directly from the sine wave since
    /// it should never be affected by physics, X still driven through the Rigidbody2D like
    /// every other mover in this codebase.
    /// </summary>
    public class FloaterEnemy : EnemyBrain
    {
        [SerializeField] float amplitude = 1f;
        [SerializeField] float frequency = 1.5f;
        [SerializeField] float horizontalSpeed = 0.8f;
        [SerializeField] float patrolDistance = 2f;

        Vector3 startPosition;
        int direction = 1;

        public void Configure(float bobAmplitude, float bobFrequency, float moveSpeed, float distance)
        {
            amplitude = bobAmplitude;
            frequency = bobFrequency;
            horizontalSpeed = moveSpeed;
            patrolDistance = distance;
        }

        protected override void Awake()
        {
            base.Awake();
            startPosition = transform.position;
            rb.gravityScale = 0f;
        }

        protected override void Move()
        {
            float offset = transform.position.x - startPosition.x;
            if (offset >= patrolDistance) direction = -1;
            else if (offset <= 0f) direction = 1;

            rb.linearVelocity = new Vector2(direction * horizontalSpeed, 0f);

            float y = startPosition.y + Mathf.Sin(Time.time * frequency) * amplitude;
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }
    }
}
