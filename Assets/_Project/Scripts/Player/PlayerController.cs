using FromCell.Evolution;
using FromCell.Input;
using UnityEngine;

namespace FromCell.Player
{
    /// <summary>
    /// Evolved from original PlayerController — orchestrates movement and jump for mobile + editor.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(GroundChecker))]
    public class PlayerController : MonoBehaviour
    {
        public float moveSpeed = 5f;
        public float jumpForce = 12f;

        [SerializeField] Joystick joystick;

        Rigidbody2D rb;
        PlayerMovement movement;
        GroundChecker groundChecker;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            movement = GetComponent<PlayerMovement>();
            groundChecker = GetComponent<GroundChecker>();
        }

        public void Jump()
        {
            if (movement != null)
                movement.TryJump(jumpForce);
            else if (groundChecker != null && groundChecker.IsGrounded)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        public void ApplyStageSettings(EvolutionStageData data)
        {
            moveSpeed = data.moveSpeed;
            jumpForce = data.jumpForce;
        }

        // Legacy collision fallback if GroundChecker layers are not configured yet.
        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ground") && groundChecker == null)
                Debug.LogWarning("Assign GroundChecker for reliable grounded detection.");
        }
    }
}
