using UnityEngine;
using UnityEngine.EventSystems;

namespace FromCell.Input
{
    /// <summary>
    /// Saved from Untitled-9 — on-screen virtual joystick for mobile.
    /// </summary>
    public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        public RectTransform background;
        public RectTransform handle;

        Vector2 inputVector;

        public float Horizontal => inputVector.x;
        public float Vertical => inputVector.y;

        public void OnDrag(PointerEventData eventData)
        {
            if (background == null || handle == null)
                return;

            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                background,
                eventData.position,
                eventData.pressEventCamera,
                out pos);

            pos.x /= Mathf.Max(1f, background.sizeDelta.x);
            pos.y /= Mathf.Max(1f, background.sizeDelta.y);

            inputVector = new Vector2(pos.x * 2, pos.y * 2);
            inputVector = inputVector.magnitude > 1.0f ? inputVector.normalized : inputVector;

            handle.anchoredPosition = new Vector2(
                inputVector.x * (background.sizeDelta.x / 3),
                inputVector.y * (background.sizeDelta.y / 3));
        }

        public void OnPointerDown(PointerEventData eventData) => OnDrag(eventData);

        public void OnPointerUp(PointerEventData eventData)
        {
            inputVector = Vector2.zero;
            handle.anchoredPosition = Vector2.zero;
        }
    }
}
