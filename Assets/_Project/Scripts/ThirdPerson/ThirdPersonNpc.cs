using UnityEngine;
using UnityEngine.AI;

namespace FromCell.ThirdPerson
{
    /// <summary>
    /// Lightweight 3D actor movement for converted NPCs. Patrol points can later be populated
    /// directly from the existing enemy/NPC blueprint data.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class ThirdPersonNpc : MonoBehaviour
    {
        [SerializeField] Transform[] patrolPoints;
        [SerializeField] float waitAtPoint = 1.25f;
        [SerializeField] ThirdPersonActorAnimation actorAnimation;

        NavMeshAgent agent;
        int nextPoint;
        float waitTimer;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.stoppingDistance = 0.15f;
        }

        void Update()
        {
            if (actorAnimation != null)
                actorAnimation.SetSpeed(agent.velocity.magnitude);

            if (patrolPoints == null || patrolPoints.Length == 0 || !agent.isOnNavMesh)
                return;

            if (!agent.hasPath)
            {
                if (waitTimer > 0f)
                {
                    waitTimer -= Time.deltaTime;
                    return;
                }

                agent.SetDestination(patrolPoints[nextPoint].position);
                return;
            }

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                nextPoint = (nextPoint + 1) % patrolPoints.Length;
                waitTimer = waitAtPoint;
                agent.ResetPath();
            }
        }

        public void SetPatrolPoints(Transform[] points)
        {
            patrolPoints = points;
            nextPoint = 0;
            waitTimer = 0f;
        }
    }
}