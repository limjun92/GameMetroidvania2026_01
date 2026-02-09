using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInputHandler))]
/// <summary>
/// 플레이어의 물리 기반 이동, 점프, 대시, 벽 타기 등을 총괄하는 컨트롤러입니다.
/// State Pattern을 사용하여 각 동작 상태(Idle, Move, Jump 등)를 관리합니다.
/// </summary>
public class MovementController : MonoBehaviour
{
    #region State Machine
    // 상태 머신 본체
    public PlayerStateMachine StateMachine { get; private set; }
    // 각 상태 인스턴스들
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerAirState AirState { get; private set; }
    public PlayerDashState DashState { get; private set; }
    public PlayerWallSlideState WallSlideState { get; private set; }
    public PlayerWallJumpState WallJumpState { get; private set; }
    #endregion

    #region Components
    public Rigidbody2D RB { get; private set; }
    public PlayerInputHandler InputHandler { get; private set; }
    public Animator Anim { get; private set; } // 애니메이션 추가 대비
    #endregion

    #region Variables
    [Header("이동 설정")]
    public float moveSpeed = 8f;
    public bool FacingRight { get; private set; } = true;

    [Header("점프 설정")]
    public float jumpForce = 16f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundLayer;
    public int amountOfJumps = 1;
    public bool canDoubleJump = false;
    public float variableJumpHeightMultiplier = 0.5f;

    [Header("대시 설정")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1.0f;
    public bool isDashUnlocked = false; // 대시 해금 여부
    public bool hasDashedInAir = false; // 공중 대시 사용 여부
    public float defaultGravityScale { get; private set; }

    [Header("벽 타기 / 벽 점프 설정")]
    public Transform wallCheck;
    public float wallCheckDistance = 0.4f;
    public LayerMask wallLayer;
    public float wallSlideSpeed = 3f;
    public float wallJumpForce = 18f;
    public Vector2 wallJumpDirection = new Vector2(1, 2);
    public float wallJumpTime = 0.4f;
    public float wallStickTime = 0.5f;
    public bool isWallJumpUnlocked = false; // 벽 점프 해금 여부
    #endregion

    public Vector2 CurrentVelocity { get; private set; }
    private Vector2 workspace;

    private void Awake()
    {
        StateMachine = new PlayerStateMachine();

        IdleState = new PlayerIdleState(this, StateMachine, "Idle");
        MoveState = new PlayerMoveState(this, StateMachine, "Move");
        JumpState = new PlayerJumpState(this, StateMachine, "Jump");
        AirState = new PlayerAirState(this, StateMachine, "Jump"); // AirState Animation? Fall?
        AirState = new PlayerAirState(this, StateMachine, "Jump"); // AirState Animation? Fall?
        DashState = new PlayerDashState(this, StateMachine, "Dash");
        WallSlideState = new PlayerWallSlideState(this, StateMachine, "WallSlide");
        WallJumpState = new PlayerWallJumpState(this, StateMachine, "Jump"); // Jump 애니메이션 재사용 또는 WallJump 사용

        RB = GetComponent<Rigidbody2D>();
        
        // 벽에 붙는 현상 해결을 위해 마찰력이 0인 재질을 생성하여 할당
        // To prevent sticking to walls, assign a material with 0 friction
        if (RB.sharedMaterial == null)
        {
            RB.sharedMaterial = new PhysicsMaterial2D("NoFriction") { friction = 0f, bounciness = 0f };
        }
        RB.interpolation = RigidbodyInterpolation2D.Interpolate;
        InputHandler = GetComponent<PlayerInputHandler>();
        Anim = GetComponent<Animator>();

        defaultGravityScale = RB.gravityScale;

        if (groundCheck == null)
        {
            Transform existingCheck = transform.Find("GroundCheck");
            if (existingCheck != null)
            {
                groundCheck = existingCheck;
            }
            else
            {
                GameObject newCheck = new GameObject("GroundCheck");
                newCheck.transform.SetParent(transform);
                newCheck.transform.localPosition = new Vector3(0, -0.5f, 0);
                groundCheck = newCheck.transform;
                Debug.LogWarning("MovementController: 'GroundCheck' Transform이 할당되지 않아 자동으로 생성했습니다. 위치를 확인해주세요.");
            }
        }

        if (groundLayer.value == 0)
        {
            groundLayer = LayerMask.GetMask("Default", "Ground", "Terrain"); // 시도할 레이어들
            if (groundLayer.value == 0) groundLayer = -1; // Everything
            Debug.LogWarning($"MovementController: 'Ground Layer'가 설정되지 않았습니다. 임시로 {groundLayer.value} (Everything/Default)로 설정합니다.");
        }

        if (wallLayer.value == 0)
        {
            wallLayer = LayerMask.GetMask("WallStickable");
            if (wallLayer.value == 0)
            {
                Debug.LogWarning("MovementController: 'WallStickable' 레이어를 찾을 수 없습니다. Inspector에서 Wall Layer를 설정해주세요.");
            }
        }

        if (wallCheck == null)
        {
             Transform existingWallCheck = transform.Find("WallCheck");
             if (existingWallCheck != null)
             {
                 wallCheck = existingWallCheck;
             }
             else
             {
                 GameObject newWallCheck = new GameObject("WallCheck");
                 newWallCheck.transform.SetParent(transform);
                 newWallCheck.transform.localPosition = new Vector3(0.5f, 0, 0); // 플레이어 앞쪽
                 wallCheck = newWallCheck.transform;
                 Debug.LogWarning("MovementController: 'WallCheck' Transform이 할당되지 않아 자동으로 생성했습니다.");
             }
        }
    }

    private void Start()
    {
        StateMachine.Initialize(IdleState);
    }

    private void Update()
    {
        CurrentVelocity = RB.linearVelocity;
        if (StateMachine.CurrentState == null)
        {
            StateMachine.Initialize(IdleState);
        }
        
        StateMachine.CurrentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
    }

    #region Set Functions
    public void SetVelocityZero()
    {
        RB.linearVelocity = Vector2.zero;
        CurrentVelocity = Vector2.zero;
    }

    public void SetVelocity(float velocity, Vector2 angle, int direction)
    {
        angle.Normalize();
        workspace.Set(angle.x * velocity * direction, angle.y * velocity);
        RB.linearVelocity = workspace;
        CurrentVelocity = workspace;
    }

    public void SetVelocity(float velocity, float yVelocity) // Custom X, Y
    {
        workspace.Set(velocity, yVelocity);
        RB.linearVelocity = workspace;
        CurrentVelocity = workspace;
    }

    public void SetVelocityX(float velocity)
    {
        workspace.Set(velocity, CurrentVelocity.y);
        RB.linearVelocity = workspace;
        CurrentVelocity = workspace;
    }

    public void SetVelocityY(float velocity)
    {
        workspace.Set(CurrentVelocity.x, velocity);
        RB.linearVelocity = workspace;
        CurrentVelocity = workspace;
    }
    #endregion

    #region Check Functions
    public bool CheckIfGrounded()
    {
        bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        // Debug.Log($"CheckIfGrounded: {isGrounded} (Pos: {groundCheck.position}, Layer: {groundLayer.value})"); 
        // Debug.Log($"CheckIfGrounded: {isGrounded} (Pos: {groundCheck.position}, Layer: {groundLayer.value})"); 
        return isGrounded;
    }

    public bool CheckIfTouchingWall()
    {
        if (wallCheck == null) return false;
        return Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, wallLayer);
    }

    public void CheckIfShouldFlip(float xInput)
    {
        if (xInput != 0 && xInput != (FacingRight ? 1 : -1))
        {
            Flip();
        }
    }

    public void Flip()
    {
        FacingRight = !FacingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }
    #endregion
    
    private void OnDrawGizmos()
    {
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        
        if (wallCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + (Vector3)(Vector2.right * transform.localScale.x * wallCheckDistance));
        }
    }
}
