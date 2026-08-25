using FromCell.Core;
using UnityEngine;

namespace FromCell.ThirdPerson
{
    /// <summary>
    /// 3D checkpoint marker. Respawn routing will consume this same signal when hazards are
    /// adapted; Level 1 includes it so its authored checkpoint beat is already represented.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ThirdPersonCheckpoint : MonoBehaviour
    {
        public static Vector3 ActivePosition { get; private set; }

        bool reached;

        void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
            ActivePosition = transform.position;
        }

        void OnTriggerEnter(Collider other)
        {
            if (reached || !other.CompareTag("Player"))
                return;

            reached = true;
            ActivePosition = transform.position;
            GameSignals.RaiseCheckpointReached(name);
        }
    }
}