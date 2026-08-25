using System;
using UnityEngine;

namespace FromCell.ThirdPerson
{
    /// <summary>
    /// Resolves tap-to-interact targets before tap-to-move. When a target is outside range,
    /// the player is sent toward it so a second tap can interact naturally.
    /// </summary>
    public class ThirdPersonInteractionSystem : MonoBehaviour
    {
        [SerializeField] float defaultInteractionRadius = 2.5f;
        [SerializeField] UnityEngine.UI.Text promptText;

        public ThirdPersonInteractable CurrentTarget { get; private set; }
        string feedbackMessage;
        float feedbackTimer;

        void Update()
        {
            if (promptText == null)
                return;

            if (feedbackTimer > 0f)
            {
                feedbackTimer -= Time.deltaTime;
                promptText.text = feedbackMessage;
                return;
            }

            if (CurrentTarget == null)
            {
                promptText.text = "Tap the ground to move";
                return;
            }

            float distance = Vector3.Distance(transform.position, CurrentTarget.transform.position);
            promptText.text = distance <= GetRadius(CurrentTarget)
                ? $"Tap to {CurrentTarget.ActionLabel}"
                : $"Move closer to {CurrentTarget.DisplayName}";
        }

        public bool TryHandleTap(Ray ray, Func<Vector3, bool> moveTo)
        {
            if (!Physics.Raycast(ray, out RaycastHit hit, 250f, ~0, QueryTriggerInteraction.Collide))
                return false;

            ThirdPersonInteractable interactable =
                hit.collider.GetComponentInParent<ThirdPersonInteractable>();
            if (interactable == null)
                return false;

            CurrentTarget = interactable;
            float radius = GetRadius(interactable);
            if (Vector3.Distance(transform.position, interactable.transform.position) <= radius)
            {
                interactable.Interact(gameObject);
                return true;
            }

            bool routeFound = moveTo != null &&
                              moveTo(interactable.GetApproachPoint(transform.position));
            if (!routeFound)
                ShowFeedback($"No route to {interactable.DisplayName}");
            return true;
        }

        void ShowFeedback(string message)
        {
            feedbackMessage = message;
            feedbackTimer = 1.5f;
        }

        float GetRadius(ThirdPersonInteractable interactable)
        {
            return interactable.InteractionRadius > 0f
                ? interactable.InteractionRadius
                : defaultInteractionRadius;
        }
    }
}