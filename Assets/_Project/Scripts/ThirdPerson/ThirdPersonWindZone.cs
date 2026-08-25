using UnityEngine;
using UnityEngine.AI;

namespace FromCell.ThirdPerson
{
    /// <summary>
    /// Adapts the original 2D wind current to a NavMeshAgent. Wind nudges the agent while it
    /// remains in the current, preserving the route's drift mechanic without physics bodies.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class ThirdPersonWindZone : MonoBehaviour
    {
        [SerializeField] Vector3 force = new Vector3(2f, 0f, 0f);

        void Awake()
        {
            GetComponent<BoxCollider>().isTrigger = true;
        }

        public void Configure(Vector3 windForce)
        {
            force = windForce;
        }

        void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            var agent = other.GetComponentInParent<NavMeshAgent>();
            if (agent != null && agent.isOnNavMesh)
                agent.Move(force * Time.deltaTime);
        }
    }
}