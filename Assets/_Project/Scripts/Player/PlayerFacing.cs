using FromCell.Input;
using UnityEngine;

namespace FromCell.Player
{
    public class PlayerFacing : MonoBehaviour
    {
        [SerializeField] float flipThreshold = 0.1f;

        void Update()
        {
            float x = TouchInputManager.Instance != null
                ? TouchInputManager.Instance.Horizontal
                : UnityEngine.Input.GetAxisRaw("Horizontal");

            if (Mathf.Abs(x) < flipThreshold) return;

            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(x) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }
}
