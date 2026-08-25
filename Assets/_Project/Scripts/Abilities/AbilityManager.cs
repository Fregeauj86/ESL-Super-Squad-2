using FromCell.Core;
using FromCell.Evolution;
using FromCell.Input;
using FromCell.Player;
using UnityEngine;

namespace FromCell.Abilities
{
    /// <summary>
    /// Evolved from original AbilityManager — touch-first double jump and dash.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class AbilityManager : MonoBehaviour
    {
        public bool canDash;
        public bool canDoubleJump;

        public float dashForce = 15f;
        public float doubleJumpForce = 10f;
        [SerializeField] float dashCooldown = 0.5f;

        Rigidbody2D rb;
        GroundChecker groundChecker;
        PlayerMovement movement;

        bool hasDoubleJumped;
        float dashCooldownTimer;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            groundChecker = GetComponent<GroundChecker>();
            movement = GetComponent<PlayerMovement>();
        }

        void Update()
        {
            if (InputGate.Instance != null && !InputGate.Instance.InputEnabled) return;

            if (canDoubleJump)
                HandleDoubleJump();

            if (canDash)
                HandleDash();

            if (groundChecker != null && groundChecker.IsGrounded)
                hasDoubleJumped = false;

            if (dashCooldownTimer > 0f)
                dashCooldownTimer -= Time.deltaTime;
        }

        void HandleDoubleJump()
        {
            bool jumpPressed = TouchInputManager.Instance != null
                ? TouchInputManager.Instance.JumpPressedThisFrame
                : UnityEngine.Input.GetButtonDown("Jump");

            if (!jumpPressed) return;
            if (groundChecker != null && groundChecker.IsGrounded) return;
            if (hasDoubleJumped) return;

            float force = movement != null ? movement.JumpForce : doubleJumpForce;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, force > 0f ? force : doubleJumpForce);
            hasDoubleJumped = true;
            GameSignals.RaisePlayerDoubleJumped();
        }

        void HandleDash()
        {
            if (dashCooldownTimer > 0f) return;

            bool dashPressed = TouchInputManager.Instance != null
                ? TouchInputManager.Instance.DashPressedThisFrame
                : UnityEngine.Input.GetKeyDown(KeyCode.LeftShift);

            if (!dashPressed) return;

            float direction = TouchInputManager.Instance != null
                ? Mathf.Sign(TouchInputManager.Instance.Horizontal)
                : Mathf.Sign(UnityEngine.Input.GetAxisRaw("Horizontal"));

            if (Mathf.Approximately(direction, 0f))
                direction = transform.localScale.x >= 0f ? 1f : -1f;

            rb.linearVelocity = new Vector2(direction * dashForce, rb.linearVelocity.y);
            dashCooldownTimer = dashCooldown;
            GameSignals.RaisePlayerDashed();
        }

        public void ApplyStageSettings(EvolutionStageData data)
        {
            canDoubleJump = data.canDoubleJump;
            canDash = data.canDash;
            dashForce = data.dashForce;
            doubleJumpForce = data.doubleJumpForce;
        }

        public void ResetAirborneAbilities()
        {
            hasDoubleJumped = false;
            dashCooldownTimer = 0f;
        }

        public float DashReadyNormalized =>
            dashCooldown <= 0f ? 1f : Mathf.Clamp01(1f - dashCooldownTimer / dashCooldown);
    }
}
