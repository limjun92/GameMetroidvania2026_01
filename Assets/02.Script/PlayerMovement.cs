using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float moveSpeed = 3f;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Move(float horizontal, bool isDashing)
    {
        if (isDashing) return; // 대시 중엔 이동하지 않음
        rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);
    }

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed; // 나중에 외부에서 속도 조절 가능
    }
}
