using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 땅에 붙어있는 상태들의 부모 클래스입니다. (Idle, Move, Dash 등)
/// 공통적으로 땅 위에 있는지 확인하고, 점프/대시 입력을 처리합니다.
/// </summary>
public class PlayerGroundedState : PlayerState
{
    protected Vector2 input;

    public PlayerGroundedState(MovementController player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.JumpState.ResetJumps(); // 땅에 닿으면 점프 횟수 초기화
        player.hasDashedInAir = false; // 공중 대시 기록 초기화
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        input = player.InputHandler.MovementInput;

        // 점프 입력 처리
        if (player.InputHandler.JumpInput)
        {
            player.InputHandler.UseJumpInput();
            stateMachine.ChangeState(player.JumpState);
        }
        // 대시 입력 처리 (해금 여부 및 쿨타임 확인)
        else if (player.InputHandler.DashInput && player.isDashUnlocked && player.DashState.CheckIfCanDash())
        {
            player.InputHandler.UseDashInput();
            stateMachine.ChangeState(player.DashState);
        }
        // 땅에서 발이 떨어지면 Air 상태로 전환
        else if (!player.CheckIfGrounded())
        {
            stateMachine.ChangeState(player.AirState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
