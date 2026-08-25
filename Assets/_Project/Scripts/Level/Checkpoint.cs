using FromCell.Core;
using UnityEngine;

namespace FromCell.Level
{
    public class Checkpoint : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            CheckpointSystem.Instance?.RegisterCheckpoint(transform);
            GameSignals.RaiseCheckpointReached();
        }
    }
}
