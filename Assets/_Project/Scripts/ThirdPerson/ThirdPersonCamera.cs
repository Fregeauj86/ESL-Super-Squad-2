using UnityEngine;

namespace FromCell.ThirdPerson
{
    /// <summary>
    /// Smooth elevated/isometric camera for the 3D conversion path.
    /// It intentionally has no dependency on the existing 2D camera scripts.
    /// </summary>
    public class ThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] Transform target;
        [SerializeField] Vector3 followOffset = new Vector3(8f, 10f, -8f);
        [SerializeField] float followSmoothTime = 0.18f;
        [SerializeField] float rotationSmooth = 10f;
        [SerializeField] float lookHeight = 1f;
        [SerializeField] float zoom = 9f;
        [SerializeField] float minZoom = 6f;
        [SerializeField] float maxZoom = 14f;
        [SerializeField] float zoomSpeed = 1.5f;
        [SerializeField] bool useOrthographic = false;

        Camera cam;
        Vector3 followVelocity;

        public Transform Target => target;

        void Awake()
        {
            cam = GetComponent<Camera>();
            ApplyProjection();
        }

        void Update()
        {
            float zoomDelta = 0f;

            if (Mathf.Abs(Input.mouseScrollDelta.y) > 0.01f)
                zoomDelta = -Input.mouseScrollDelta.y * zoomSpeed;

            if (Input.touchCount == 2)
            {
                Touch first = Input.GetTouch(0);
                Touch second = Input.GetTouch(1);
                Vector2 previousFirst = first.position - first.deltaPosition;
                Vector2 previousSecond = second.position - second.deltaPosition;
                float previousDistance = Vector2.Distance(previousFirst, previousSecond);
                float currentDistance = Vector2.Distance(first.position, second.position);
                zoomDelta = (previousDistance - currentDistance) * 0.01f * zoomSpeed;
            }

            if (Mathf.Abs(zoomDelta) > 0.0001f)
            {
                zoom = Mathf.Clamp(zoom + zoomDelta, minZoom, maxZoom);
                ApplyProjection();
            }
        }

        void LateUpdate()
        {
            if (target == null)
                return;

            Vector3 desiredPosition = target.position + GetScaledOffset();
            transform.position = Vector3.SmoothDamp(
                transform.position,
                desiredPosition,
                ref followVelocity,
                followSmoothTime);

            Vector3 lookDirection = target.position + Vector3.up * lookHeight - transform.position;
            if (lookDirection.sqrMagnitude > 0.001f)
            {
                Quaternion desiredRotation = Quaternion.LookRotation(lookDirection.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    desiredRotation,
                    1f - Mathf.Exp(-rotationSmooth * Time.deltaTime));
            }
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        public void SetZoom(float value)
        {
            zoom = Mathf.Clamp(value, minZoom, maxZoom);
            ApplyProjection();
        }

        Vector3 GetScaledOffset()
        {
            if (followOffset.sqrMagnitude < 0.01f)
                followOffset = new Vector3(8f, 10f, -8f);

            Vector3 direction = followOffset.normalized;
            return direction * zoom;
        }

        void ApplyProjection()
        {
            if (cam == null)
                return;

            cam.orthographic = useOrthographic;
            if (useOrthographic)
                cam.orthographicSize = zoom;
            else
                cam.fieldOfView = Mathf.Clamp(zoom * 4f, 35f, 70f);
        }
    }
}