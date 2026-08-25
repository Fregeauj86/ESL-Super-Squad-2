using FromCell.Input;
using UnityEngine;

namespace FromCell.ESL
{
    /// <summary>
    /// A physical trigger in a level blocking progress until the linked VillainEncounter is
    /// passed. One-shot like FinishZone (does NOT re-check every frame the way RoleGate does)
    /// - once passed, it stays open permanently.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class VillainGate : MonoBehaviour
    {
        public string encounterId;
        public Collider2D blockingCollider;

        bool passed;

        void Awake()
        {
            var trigger = GetComponent<Collider2D>();
            trigger.isTrigger = true;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (passed || !other.CompareTag("Player")) return;

            // Only start a challenge when input is currently free - never steal ownership
            // from whichever system already has it (pause, death, another challenge).
            if (InputGate.Instance == null || !InputGate.Instance.InputEnabled) return;

            if (EslChallengeController.Instance == null)
            {
                Debug.LogWarning($"VillainGate '{name}': no EslChallengeController in scene.");
                return;
            }

            EslChallengeController.Instance.Begin(encounterId, this);
        }

        public void MarkPassed()
        {
            passed = true;
            if (blockingCollider != null)
                blockingCollider.enabled = false;
        }
    }
}
