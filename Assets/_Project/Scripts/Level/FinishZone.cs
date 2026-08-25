using FromCell.Core;
using UnityEngine;

namespace FromCell.Level
{
    /// <summary>
    /// Evolved from original LevelComplete trigger.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class FinishZone : MonoBehaviour
    {
        void Awake()
        {
            var col = GetComponent<Collider2D>();
            col.isTrigger = true;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            if (LevelCompletionSystem.Instance != null)
                LevelCompletionSystem.Instance.TryCompleteLevel();
            else
                GameFlowSystem.Instance?.CompleteLevel();
        }
    }
}
