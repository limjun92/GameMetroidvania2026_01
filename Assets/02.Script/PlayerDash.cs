using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerJump))]
public class PlayerDash : MonoBehaviour
{
    [Header("대시 설정")]
    public float dashSpeed = 20f;         // 대시 속도
    public float dashDuration = 0.2f;     // 대시 유지 시간
    public float dashCooldown = 0.2f;       // 대시 쿨타임
    private bool canDash = true;          // 쿨타임 중인지 확인
    private bool isDashing = false;       // 현재 대시 중인지 확인

    private PlayerJump jump;

    //isDashing을 외부에서 변경 못하도록 하고 IsDashing으로 외부에서 사용
    private Rigidbody2D rb;
    public bool IsDashing => isDashing;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        jump = GetComponent<PlayerJump>();
    }
    void Update()
    {
        if (jump.IsGrounded)
            canDash = true;
    }
    public void TryDash(bool facingRight)
    {
        if (!canDash || isDashing) 
            return;

        if (jump.IsJumping && canDash)
            canDash = false; // 공중 대시 사용

        StartCoroutine(DashRoutine(facingRight));
    }

    IEnumerator DashRoutine(bool facingRight)
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;

        float direction = facingRight ? 1 : -1;
        rb.velocity = new Vector2(direction * dashSpeed, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        if (jump.IsGrounded)
            canDash = true;
    }
}
