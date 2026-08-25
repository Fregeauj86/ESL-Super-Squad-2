using UnityEngine;

namespace FromCell.Cameras
{
    /// <summary>
    /// Moves this transform's X a fraction of how far the camera has moved from its own start
    /// position - purely visual background/foreground scenery, no gameplay effect. Ordered
    /// after CameraDirector (100) so it reads the camera's fully-resolved position for this
    /// frame instead of lagging a frame behind.
    /// </summary>
    [DefaultExecutionOrder(150)]
    public class ParallaxLayer : MonoBehaviour
    {
        [SerializeField] float parallaxFactor = 0.5f;

        UnityEngine.Camera cam;
        Vector3 startPosition;
        float startCamX;
        bool initialized;

        void LateUpdate()
        {
            if (cam == null)
            {
                cam = UnityEngine.Camera.main;
                if (cam == null) return;
            }

            if (!initialized)
            {
                startPosition = transform.position;
                startCamX = cam.transform.position.x;
                initialized = true;
            }

            float camDelta = cam.transform.position.x - startCamX;
            transform.position = new Vector3(startPosition.x + camDelta * parallaxFactor, startPosition.y, startPosition.z);
        }
    }
}
