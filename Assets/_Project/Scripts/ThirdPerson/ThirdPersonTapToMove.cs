using System;
using FromCell.Input;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace FromCell.ThirdPerson
{
    /// <summary>
    /// Tap/click-to-move input for the 3D path. Editor mouse input is retained so the same
    /// scene can be tested without a device.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class ThirdPersonTapToMove : MonoBehaviour
    {
        [SerializeField] Camera inputCamera;
        [SerializeField] LayerMask groundMask = ~0;
        [SerializeField] float rayDistance = 250f;
        [SerializeField] float navMeshSampleDistance = 4f;
        [SerializeField] float destinationFeedbackDuration = 0.35f;
        [SerializeField] ThirdPersonInteractionSystem interactionSystem;

        NavMeshAgent agent;
        float feedbackTimer;
        int pendingTouchId = int.MinValue;
        Vector2 pendingTouchStart;
        bool touchSequenceCancelled;

        const float TapMovementThreshold = 28f;

        public Vector3 LastDestination { get; private set; }
        public bool HasDestination => agent != null && agent.hasPath;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            if (inputCamera == null)
                inputCamera = Camera.main;
        }

        void Update()
        {
            if (InputGate.Instance != null && !InputGate.Instance.InputEnabled)
                return;

            if (inputCamera == null)
                inputCamera = Camera.main;

            if (TryGetPointerTap(out Vector2 screenPosition, out int pointerId))
            {
                if (IsPointerOverUi(pointerId))
                    return;

                Ray ray = inputCamera.ScreenPointToRay(screenPosition);
                if (interactionSystem != null &&
                    interactionSystem.TryHandleTap(ray, TrySetDestination))
                    return;

                TryMoveToRay(ray);
            }

            if (feedbackTimer > 0f)
                feedbackTimer -= Time.deltaTime;
        }

        public void SetDestination(Vector3 worldPosition)
        {
            TrySetDestination(worldPosition);
        }

        public bool TrySetDestination(Vector3 worldPosition)
        {
            if (agent == null || !agent.isOnNavMesh)
                return false;

            if (!NavMesh.SamplePosition(
                    worldPosition,
                    out NavMeshHit navHit,
                    navMeshSampleDistance,
                    NavMesh.AllAreas))
                return false;

            if (!agent.SetDestination(navHit.position))
                return false;

            LastDestination = navHit.position;
            feedbackTimer = destinationFeedbackDuration;
            return true;
        }

        void TryMoveToRay(Ray ray)
        {
            if (!Physics.Raycast(ray, out RaycastHit hit, rayDistance, groundMask, QueryTriggerInteraction.Ignore))
                return;

            TrySetDestination(hit.point);
        }

        static bool IsPointerOverUi(int pointerId)
        {
            if (EventSystem.current == null)
                return false;

            return pointerId >= 0
                ? EventSystem.current.IsPointerOverGameObject(pointerId)
                : EventSystem.current.IsPointerOverGameObject();
        }

        bool TryGetPointerTap(out Vector2 screenPosition, out int pointerId)
        {
            if (UnityEngine.Input.touchCount > 1)
            {
                pendingTouchId = int.MinValue;
                touchSequenceCancelled = true;
                screenPosition = default;
                pointerId = -1;
                return false;
            }

            if (UnityEngine.Input.touchCount == 1)
            {
                Touch touch = UnityEngine.Input.GetTouch(0);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        pendingTouchId = touch.fingerId;
                        pendingTouchStart = touch.position;
                        touchSequenceCancelled = false;
                        break;

                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        if (pendingTouchId == touch.fingerId &&
                            Vector2.Distance(pendingTouchStart, touch.position) > TapMovementThreshold)
                            touchSequenceCancelled = true;
                        break;

                    case TouchPhase.Ended:
                        bool isTap = pendingTouchId == touch.fingerId &&
                                     !touchSequenceCancelled &&
                                     Vector2.Distance(pendingTouchStart, touch.position) <= TapMovementThreshold;
                        pendingTouchId = int.MinValue;
                        touchSequenceCancelled = false;
                        if (isTap)
                        {
                            screenPosition = touch.position;
                            pointerId = touch.fingerId;
                            return true;
                        }
                        break;

                    case TouchPhase.Canceled:
                        pendingTouchId = int.MinValue;
                        touchSequenceCancelled = false;
                        break;
                }

                screenPosition = default;
                pointerId = -1;
                return false;
            }

            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                screenPosition = UnityEngine.Input.mousePosition;
                pointerId = -1;
                return true;
            }

            screenPosition = default;
            pointerId = -1;
            return false;
        }
    }
}