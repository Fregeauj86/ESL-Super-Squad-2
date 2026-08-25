using FromCell.Player;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FromCell.Input
{
    /// <summary>
    /// Saved from Untitled-7 — wire to UI Button OnClick.
    /// Supports direct PlayerController reference (original) or TouchInputManager (mobile pipeline).
    /// Also implements pointer down/up so TouchInputManager.JumpHeld reflects an actually-held
    /// button (for variable jump height) - existing OnJumpPressed/onClick wiring is untouched.
    /// </summary>
    public class JumpButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public PlayerController player;

        public void OnJumpPressed()
        {
            if (player != null)
            {
                player.Jump();
                return;
            }

            TouchInputManager.Instance?.RegisterJump();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (player == null)
                TouchInputManager.Instance?.RegisterJumpDown();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (player == null)
                TouchInputManager.Instance?.RegisterJumpUp();
        }
    }
}
