using UnityEngine;

namespace FromCell.Level.Platforms
{
    /// <summary>
    /// How a rider on top gets carried along as the platform moves.
    /// PositionDelta: the rider's transform is nudged by the platform's frame-to-frame
    /// movement (safe, no hierarchy change - the default). Reparent: the rider is actually
    /// parented under the platform while riding, so it inherits movement automatically and
    /// naturally survives the platform reversing direction mid-ride; un-parented the moment it
    /// stops standing on top.
    /// </summary>
    public enum CarryMode { PositionDelta, Reparent }

    /// <summary>
    /// Ping-pongs (or loops) between authored waypoints and carries whatever is standing on
    /// top. Rider detection is bounds-based (rider's collider bottom at/above this platform's
    /// collider top), not contact-normal-based - normal direction/sign for Collision2D
    /// contacts isn't something I can verify without running the physics engine, so this
    /// avoids depending on it. Tag stays "Ground" (set by whoever builds this, e.g.
    /// LevelAssembler) so GroundChecker/PlayerController treat it exactly like static ground.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class MovingPlatform : MonoBehaviour
    {
        [SerializeField] Vector2[] waypoints = System.Array.Empty<Vector2>();
        [SerializeField] float speed = 2f;
        [SerializeField] bool loop;
        [SerializeField] CarryMode carryMode = CarryMode.PositionDelta;

        Collider2D platformCollider;
        Transform rider;
        Vector3 previousPosition;
        int targetIndex = 1;
        int direction = 1;

        public void Configure(Vector2[] points, float moveSpeed, CarryMode mode, bool loopPath = false)
        {
            waypoints = points;
            speed = moveSpeed;
            carryMode = mode;
            loop = loopPath;
        }

        void Awake()
        {
            platformCollider = GetComponent<Collider2D>();
        }

        void Start()
        {
            if (waypoints.Length > 0)
                transform.position = waypoints[0];
            previousPosition = transform.position;
        }

        void FixedUpdate()
        {
            if (waypoints.Length < 2) return;

            Vector3 target = waypoints[targetIndex];
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.fixedDeltaTime);

            if (Vector3.Distance(transform.position, target) < 0.02f)
                AdvanceTarget();

            Vector3 delta = transform.position - previousPosition;
            if (carryMode == CarryMode.PositionDelta && rider != null && delta.sqrMagnitude > 0f)
                rider.position += delta;

            previousPosition = transform.position;
            UpdateRider();
        }

        void AdvanceTarget()
        {
            if (loop)
            {
                targetIndex = (targetIndex + 1) % waypoints.Length;
                return;
            }

            targetIndex += direction;
            if (targetIndex >= waypoints.Length)
            {
                targetIndex = waypoints.Length - 2;
                direction = -1;
            }
            else if (targetIndex < 0)
            {
                targetIndex = 1;
                direction = 1;
            }
        }

        void UpdateRider()
        {
            var playerGo = GameObject.FindGameObjectWithTag("Player");
            if (playerGo == null)
            {
                ReleaseRider();
                return;
            }

            var playerCollider = playerGo.GetComponent<Collider2D>();
            if (playerCollider == null)
            {
                ReleaseRider();
                return;
            }

            bool onTop = playerCollider.bounds.min.y >= platformCollider.bounds.max.y - 0.15f
                && playerCollider.bounds.max.x > platformCollider.bounds.min.x
                && playerCollider.bounds.min.x < platformCollider.bounds.max.x;

            if (onTop && rider == null)
            {
                rider = playerGo.transform;
                if (carryMode == CarryMode.Reparent)
                    rider.SetParent(transform, true);
            }
            else if (!onTop && rider != null)
            {
                ReleaseRider();
            }
        }

        void ReleaseRider()
        {
            if (rider != null && carryMode == CarryMode.Reparent)
                rider.SetParent(null, true);
            rider = null;
        }
    }
}
