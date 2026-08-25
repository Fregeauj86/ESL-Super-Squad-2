using UnityEngine;
using UnityEngine.Events;

namespace FromCell.ThirdPerson
{
    /// <summary>
    /// 3D interaction seam for NPCs, lesson objects, gates, and future converted level content.
    /// The UnityEvent keeps existing game systems decoupled from the 3D targeting layer.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ThirdPersonInteractable : MonoBehaviour
    {
        [SerializeField] string displayName = "object";
        [SerializeField] string actionLabel = "interact";
        [SerializeField] float interactionRadius = 2.5f;
        [SerializeField] UnityEvent onInteracted = new UnityEvent();

        public string DisplayName => string.IsNullOrEmpty(displayName) ? name : displayName;
        public string ActionLabel => string.IsNullOrEmpty(actionLabel) ? "interact" : actionLabel;
        public float InteractionRadius => interactionRadius;

        public void Interact(GameObject actor)
        {
            onInteracted?.Invoke();
            Debug.Log($"From Cell 3D: {actor.name} interacted with {DisplayName}.");
        }

        public Vector3 GetApproachPoint(Vector3 fromPosition)
        {
            Vector3 direction = (fromPosition - transform.position);
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.01f)
                direction = Vector3.back;

            return transform.position + direction.normalized * Mathf.Max(0.75f, interactionRadius * 0.75f);
        }
    }
}