using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Unity의 New Input System을 사용하여 플레이어의 입력을 처리하는 클래스입니다.
/// </summary>
public class PlayerInputHandler : MonoBehaviour
{
    // 외부에서 접근 가능한 입력 상태 프로퍼티들
    public Vector2 MovementInput { get; private set; }
    public bool JumpInput { get; private set; }
    public bool DashInput { get; private set; }

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction dashAction;

    private void Awake()
    {
        // Define Actions programmatically to avoid Asset dependency
        moveAction = new InputAction(name: "Move", binding: "<Gamepad>/leftStick");
        moveAction.AddCompositeBinding("Dpad")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");

        jumpAction = new InputAction(name: "Jump", binding: "<Keyboard>/space");
        jumpAction.AddBinding("<Gamepad>/buttonSouth");

        dashAction = new InputAction(name: "Dash", binding: "<Keyboard>/leftShift");
        dashAction.AddBinding("<Gamepad>/buttonWest"); // X on Xbox, Square on PS
    }

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        dashAction.Enable();

        jumpAction.performed += OnJump;
        dashAction.performed += OnDash;
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        dashAction.Disable();

        jumpAction.performed -= OnJump;
        dashAction.performed -= OnDash;
    }

    [SerializeField] private float jumpInputBufferTime = 0.2f;
    private float jumpInputStartTime;

    [SerializeField] private float dashInputBufferTime = 0.2f;
    private float dashInputStartTime;

    private void Update()
    {
        MovementInput = moveAction.ReadValue<Vector2>();
        CheckJumpInputBuffer();
        CheckDashInputBuffer();
    }

    public void UseJumpInput() => JumpInput = false;
    public void UseDashInput() => DashInput = false;

    private void CheckJumpInputBuffer()
    {
        if (Time.time >= jumpInputStartTime + jumpInputBufferTime)
        {
            JumpInput = false;
        }
    }

    private void CheckDashInputBuffer()
    {
        if (Time.time >= dashInputStartTime + dashInputBufferTime)
        {
            DashInput = false;
        }
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        JumpInput = true;
        jumpInputStartTime = Time.time;
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        DashInput = true;
        dashInputStartTime = Time.time;
    }

}
