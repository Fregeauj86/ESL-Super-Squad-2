using UnityEngine;

namespace FromCell.Level.Enemy
{
    /// <summary>
    /// Idle until the player comes within detectRadius, then walks straight at their current X
    /// position. Loses the player and stops the moment they leave the radius again - no
    /// memory/last-known-position chasing, kept deliberately simple.
    /// </summary>
    public class ChaserEnemy : EnemyBrain
    {
        [SerializeField] float speed = 2.5f;
        [SerializeField] float detectRadius = 5f;

        SpriteRenderer sr;
        Transform player;

        public void Configure(float moveSpeed, float radius)
        {
            speed = moveSpeed;
            detectRadius = radius;
        }

        protected override void Awake()
        {
            base.Awake();
            sr = GetComponent<SpriteRenderer>();
        }

        protected override void Move()
        {
            if (player == null)
            {
                var playerGo = GameObject.FindGameObjectWithTag("Player");
                if (playerGo != null) player = playerGo.transform;
            }

            if (player == null || Vector2.Distance(transform.position, player.position) > detectRadius)
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                return;
            }

            float dir = Mathf.Sign(player.position.x - transform.position.x);
            rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);
            if (sr != null) sr.flipX = dir < 0;
        }
    }
}
