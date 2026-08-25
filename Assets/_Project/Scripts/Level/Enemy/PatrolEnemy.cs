using UnityEngine;

namespace FromCell.Level.Enemy
{
    /// <summary>
    /// Walks back and forth over a fixed horizontal range from its spawn point, reversing at
    /// each end - same fixed-range ping-pong shape as MovingPlatform's waypoint walking, just
    /// driven by distance-from-start instead of authored waypoints.
    /// </summary>
    public class PatrolEnemy : EnemyBrain
    {
        [SerializeField] float speed = 1.5f;
        [SerializeField] float patrolDistance = 3f;

        SpriteRenderer sr;
        Vector3 startPosition;
        int direction = 1;

        public void Configure(float moveSpeed, float distance)
        {
            speed = moveSpeed;
            patrolDistance = distance;
        }

        protected override void Awake()
        {
            base.Awake();
            sr = GetComponent<SpriteRenderer>();
            startPosition = transform.position;
        }

        protected override void Move()
        {
            float offset = transform.position.x - startPosition.x;
            if (offset >= patrolDistance) direction = -1;
            else if (offset <= 0f) direction = 1;

            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
            if (sr != null) sr.flipX = direction < 0;
        }
    }
}
