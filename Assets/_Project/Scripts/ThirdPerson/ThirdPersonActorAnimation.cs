using UnityEngine;
using UnityEngine.AI;

namespace FromCell.ThirdPerson
{
    /// <summary>
    /// Animation bridge for the conversion path. Real Animator controllers can be assigned
    /// later; the fallback bob and facing keep placeholder actors readable in the test scene.
    /// </summary>
    public class ThirdPersonActorAnimation : MonoBehaviour
    {
        [SerializeField] Animator animator;
        [SerializeField] Transform visualRoot;
        [SerializeField] string speedParameter = "Speed";
        [SerializeField] float bobHeight = 0.035f;
        [SerializeField] float bobFrequency = 9f;

        Vector3 visualStartLocalPosition;
        float speed;

        void Awake()
        {
            if (animator == null)
                animator = GetComponentInChildren<Animator>();
            if (visualRoot == null)
                visualRoot = transform;

            visualStartLocalPosition = visualRoot.localPosition;
        }

        void Update()
        {
            if (animator != null && animator.runtimeAnimatorController != null)
                animator.SetFloat(speedParameter, speed);

            if (visualRoot != null && visualRoot != transform)
            {
                float bob = speed > 0.1f
                    ? Mathf.Abs(Mathf.Sin(Time.time * bobFrequency)) * bobHeight
                    : 0f;
                visualRoot.localPosition = visualStartLocalPosition + Vector3.up * bob;
            }

            if (speed > 0.05f)
            {
                Vector3 direction = GetMovementDirection();
                if (direction.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation,
                        targetRotation,
                        720f * Time.deltaTime);
                }
            }
        }

        public void SetSpeed(float value)
        {
            speed = value;
        }

        Vector3 GetMovementDirection()
        {
            var agent = GetComponent<NavMeshAgent>();
            if (agent == null)
                return Vector3.zero;

            Vector3 direction = agent.velocity;
            direction.y = 0f;
            return direction.normalized;
        }
    }
}