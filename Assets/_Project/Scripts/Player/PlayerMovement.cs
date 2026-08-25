using FromCell.Core;
using FromCell.Evolution;
using FromCell.Input;
using UnityEngine;

namespace FromCell.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        Rigidbody2D rb;
        GroundChecker groundChecker;

        MovementMode movementMode = MovementMode.Walk;
        float moveSpeed = 5f;
        float acceleration = 50f;
        float jumpForce = 12f;
        float gravityScale = 3f;
        float airControl = 1f;
        bool canJump = true;

        float jumpBufferTimer;
        const float JumpBufferTime = 0.12f;

        // Variable jump height ("jump cut"): falling gets extra gravity for a snappier
        // descent, and releasing the jump button early while still ascending cuts the jump
        // short instead of always reaching full height. Deliberately modest multipliers -
        // this rides on top of each stage's already-tuned base gravity, not replacing it.
        const float FallGravityMultiplier = 1.3f;
        const float JumpCutGravityMultiplier = 2.0f;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            groundChecker = GetComponent<GroundChecker>();
        }

        void FixedUpdate()
        {
            if (InputGate.Instance != null && !InputGate.Instance.InputEnabled) return;

            float inputX = TouchInputManager.Instance != null
                ? TouchInputManager.Instance.Horizontal
                : UnityEngine.Input.GetAxisRaw("Horizontal");

            float inputY = TouchInputManager.Instance != null
                ? TouchInputManager.Instance.Vertical
                : UnityEngine.Input.GetAxisRaw("Vertical");

            ApplyHorizontalMovement(inputX, inputY);
            ApplyGravityMode();
        }

        void Update()
        {
            if (InputGate.Instance != null && !InputGate.Instance.InputEnabled) return;

            bool jumpRequested = TouchInputManager.Instance != null
                ? TouchInputManager.Instance.JumpPressedThisFrame
                : UnityEngine.Input.GetButtonDown("Jump");

            if (jumpRequested)
                jumpBufferTimer = JumpBufferTime;
            else
                jumpBufferTimer -= Time.deltaTime;

            if (jumpBufferTimer > 0f && canJump && groundChecker != null && groundChecker.IsGrounded)
                TryJump(jumpForce);
        }

        void ApplyHorizontalMovement(float inputX, float inputY = 0f)
        {
            if (movementMode == MovementMode.Float)
            {
                Vector2 target = new Vector2(inputX, inputY) * moveSpeed;
                rb.linearVelocity = Vector2.MoveTowards(
                    rb.linearVelocity,
                    target,
                    acceleration * Time.fixedDeltaTime);
                return;
            }

            float targetSpeed = inputX * moveSpeed;
            float control = groundChecker != null && groundChecker.IsGrounded ? 1f : airControl;

            if (movementMode == MovementMode.Float && groundChecker != null && !groundChecker.IsGrounded)
                control *= 0.6f;

            float newX = Mathf.MoveTowards(
                rb.linearVelocity.x,
                targetSpeed,
                acceleration * control * Time.fixedDeltaTime);

            rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);
        }

        void ApplyGravityMode()
        {
            if (movementMode == MovementMode.Float)
            {
                rb.gravityScale = 0f;
                return;
            }

            float baseGravity;
            if (movementMode == MovementMode.Crawl)
                baseGravity = gravityScale * 0.85f;
            else
                baseGravity = gravityScale;

            bool jumpHeld = TouchInputManager.Instance != null
                ? TouchInputManager.Instance.JumpHeld
                : UnityEngine.Input.GetButton("Jump");

            if (rb.linearVelocity.y < 0f)
                rb.gravityScale = baseGravity * FallGravityMultiplier;
            else if (rb.linearVelocity.y > 0f && !jumpHeld)
                rb.gravityScale = baseGravity * JumpCutGravityMultiplier;
            else
                rb.gravityScale = baseGravity;
        }

        public void TryJump(float force)
        {
            if (!canJump || force <= 0f) return;

            jumpBufferTimer = 0f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, force);
            GameSignals.RaisePlayerJumped();
        }

        public void ApplyStageSettings(EvolutionStageData data)
        {
            movementMode = data.movementMode;
            moveSpeed = data.moveSpeed;
            acceleration = data.acceleration;
            jumpForce = data.jumpForce;
            gravityScale = data.gravityScale;
            airControl = data.airControl;
            canJump = data.canJump && data.jumpForce > 0f;
        }

        public void ApplyGrowthBonus(float speedBonus, float jumpBonus)
        {
            moveSpeed += speedBonus;
            jumpForce += jumpBonus;
            canJump = jumpForce > 0f;
        }

        public float JumpForce => jumpForce;
        public bool CanJump => canJump;
    }
}
