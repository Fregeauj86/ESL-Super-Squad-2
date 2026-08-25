using UnityEngine;

namespace FromCell.ThirdPerson
{
    /// <summary>
    /// 3D finish trigger for a converted level. The Level 1 flow owns its authored validation
    /// (all vocabulary gems and the Echo Fox gate) before reporting completion.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ThirdPersonFinishZone3D : MonoBehaviour
    {
        [SerializeField] ThirdPersonLevel1Flow levelFlow;

        bool triggered;

        void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        void OnTriggerEnter(Collider other)
        {
            if (triggered || !other.CompareTag("Player"))
                return;

            if (levelFlow != null)
                triggered = levelFlow.TryFinish();
        }

        public void Configure(ThirdPersonLevel1Flow flow)
        {
            levelFlow = flow;
        }
    }
}