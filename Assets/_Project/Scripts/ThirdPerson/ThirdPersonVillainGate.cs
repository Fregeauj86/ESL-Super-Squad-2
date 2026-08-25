using FromCell.Core;
using FromCell.ESL;
using UnityEngine;
using UnityEngine.AI;

namespace FromCell.ThirdPerson
{
    /// <summary>
    /// 3D counterpart to VillainGate. It opens only after the existing ESL encounter reports a
    /// pass, so the authored Echo Fox challenge remains the gate's source of truth.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ThirdPersonVillainGate : MonoBehaviour
    {
        [SerializeField] string encounterId;
        [SerializeField] Collider blockingCollider;
        [SerializeField] ThirdPersonRuntimeNavMesh runtimeNavMesh;

        public bool IsPassed { get; private set; }
        public string EncounterId => encounterId;

        bool challengeStarted;

        void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        void OnEnable()
        {
            GameSignals.ChallengeCompleted += OnChallengeCompleted;
        }

        void OnDisable()
        {
            GameSignals.ChallengeCompleted -= OnChallengeCompleted;
        }

        void OnTriggerEnter(Collider other)
        {
            if (IsPassed || challengeStarted || !other.CompareTag("Player"))
                return;

            if (EslChallengeController.Instance == null)
            {
                Debug.LogError($"ThirdPersonVillainGate '{name}': no ESL challenge overlay is available.");
                return;
            }

            challengeStarted = true;
            EslChallengeController.Instance.Begin(encounterId, null);
        }

        void OnChallengeCompleted(string completedEncounterId, bool passed, int correctCount, int totalTasks)
        {
            if (completedEncounterId != encounterId)
                return;

            challengeStarted = false;
            if (passed)
                MarkPassed();
        }

        public void MarkPassed()
        {
            IsPassed = true;
            if (blockingCollider != null)
                blockingCollider.enabled = false;
            if (runtimeNavMesh != null)
                runtimeNavMesh.Build();
        }

        public void Configure(string id, Collider blocker, ThirdPersonRuntimeNavMesh navMesh)
        {
            encounterId = id;
            blockingCollider = blocker;
            runtimeNavMesh = navMesh;
        }
    }
}