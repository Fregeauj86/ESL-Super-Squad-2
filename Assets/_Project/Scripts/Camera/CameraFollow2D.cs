using UnityEngine;

namespace FromCell.Cameras
{
    public class CameraFollow2D : MonoBehaviour
    {
        [SerializeField] Transform target;
        [SerializeField] Vector3 offset = new Vector3(0f, 1f, -10f);
        [SerializeField] float smoothTime = 0.15f;

        Vector3 velocity;

        void LateUpdate()
        {
            if (target == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                    target = player.transform;
                else
                    return;
            }

            Vector3 goal = target.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, goal, ref velocity, smoothTime);
        }
    }
}
