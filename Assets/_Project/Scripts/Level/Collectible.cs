using FromCell.Core;
using UnityEngine;

namespace FromCell.Level
{
    public class Collectible : MonoBehaviour
    {
        public static int CollectedCount { get; private set; }

        // Per-level total, separate from CollectedCount so a HUD can show "3 / 7". Deliberately
        // NOT reset by ResetCount() - a fresh level load runs every Collectible's Awake (which
        // increments this) before LevelBootstrap.Start() calls ResetCount(), so the total is
        // already correct by the time collection starts. A future scene-reload path (a level
        // assembler spawning collectibles at runtime) is expected to call ResetTotal() itself
        // right before spawning, since it controls that ordering explicitly.
        public static int TotalInLevel { get; private set; }

        public static void ResetCount() => CollectedCount = 0;
        public static void ResetTotal() => TotalInLevel = 0;

        void Awake()
        {
            TotalInLevel++;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            CollectedCount++;
            GameSignals.RaiseCollectiblePicked();
            Destroy(gameObject);
        }
    }
}
