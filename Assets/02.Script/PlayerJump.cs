using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerDash))]
public class PlayerJump : MonoBehaviour
{
    [Header("점프 설정")]
    public float jumpForce = 12f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("더블 점프")]
    public bool canDoubleJump = false;  // 아이템으로 활성화됨
    private bool allowDoubleJump = false;

    private Rigidbody2D rb;
    private bool isGrounded;

    public bool IsGrounded => isGrounded;
    public bool IsJumping => !isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
            allowDoubleJump = true;
    }
    public void TryJump()
    {
        if (isGrounded)
        {
            Jump();
        }
        else if (canDoubleJump && allowDoubleJump)
        {
            Jump();
            allowDoubleJump = false;
        }
    }
    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }
}
