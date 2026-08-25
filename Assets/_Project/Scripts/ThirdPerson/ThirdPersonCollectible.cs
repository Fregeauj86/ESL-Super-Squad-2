using FromCell.Core;
using UnityEngine;

namespace FromCell.ThirdPerson
{
    /// <summary>
    /// 3D counterpart to the existing 2D collectible. It reports through the same game signal
    /// while keeping its counter isolated until the shared level completion adapter is ready.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ThirdPersonCollectible : MonoBehaviour
    {
        public static int CollectedCount { get; private set; }
        public static int TotalInLevel { get; private set; }

        bool collected;

        void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        public static void ResetForLevel(int total)
        {
            CollectedCount = 0;
            TotalInLevel = Mathf.Max(0, total);
        }

        void OnTriggerEnter(Collider other)
        {
            if (collected || !other.CompareTag("Player"))
                return;

            collected = true;
            CollectedCount++;
            GameSignals.RaiseCollectiblePicked(name);
            Destroy(gameObject);
        }
    }
}