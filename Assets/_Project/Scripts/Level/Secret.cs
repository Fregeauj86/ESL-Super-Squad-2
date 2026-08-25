using FromCell.Core;
using UnityEngine;

namespace FromCell.Level
{
    /// <summary>
    /// Optional bonus pickup, separate from Collectible - does NOT count toward
    /// requiredCollectibles or the HUD's Collectible.CollectedCount, just tracks its own count
    /// for a possible future secrets-found stat. Reuses GameSignals.CollectiblePicked purely so
    /// FxBinder/audio hooks react the same way a normal pickup does; that signal isn't read by
    /// anything that increments a level-progress counter, so this can't affect completion.
    /// </summary>
    public class Secret : MonoBehaviour
    {
        public static int FoundCount { get; private set; }
        public static void ResetCount() => FoundCount = 0;

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            FoundCount++;
            GameSignals.RaiseCollectiblePicked();
            Destroy(gameObject);
        }
    }
}
