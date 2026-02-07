using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerJump))]
[RequireComponent(typeof(PlayerDash))]
[RequireComponent(typeof(PlayerMovement))]
public class MovementController : MonoBehaviour
{
    //=====================
    //01.이동 관련
    //=====================
    private PlayerMovement movement;

    //=====================
    //01.점프관련
    //=====================
    private PlayerJump jump;


    //=====================
    //01.대시관련
    //=====================
    private PlayerDash dash;
    
    private Vector2 input;
    private bool facingRight = true;

    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        jump = GetComponent<PlayerJump>();
        dash = GetComponent<PlayerDash>();
        //doubleJump = true;
    }

    void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");

        if (input.x > 0 && !facingRight) Flip();
        else if (input.x < 0 && facingRight) Flip();

        //01.점프관련
        if (Input.GetButtonDown("Jump"))
            jump.TryJump();

        //02.대시관련
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            dash.TryDash(facingRight);
        };

        


        if (dash.IsDashing) return;
    }

    void FixedUpdate()
    {
        movement.Move(input.x, dash.IsDashing);
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
