using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public bool canDash;
    public bool canDoubleJump;

    private Rigidbody2D rb;

    public float dashForce = 15f;
    private bool hasDoubleJumped;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (canDoubleJump)
        {
            HandleDoubleJump();
        }

        if (canDash)
        {
            HandleDash();
        }
    }

    void HandleDoubleJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (!IsGrounded() && !hasDoubleJumped)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 10f);
                hasDoubleJumped = true;
            }
        }

        if (IsGrounded())
            hasDoubleJumped = false;
    }

    void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            rb.linearVelocity = new Vector2(dashForce, rb.linearVelocity.y);
        }
    }

    bool IsGrounded()
    {
        return Physics2D.Raycast(transform.position, Vector2.down, 1.1f);
    }
}
