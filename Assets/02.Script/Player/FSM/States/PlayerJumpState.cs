using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 점프를 수행하는 상태입니다. 이단 점프 로직도 포함합니다.
/// </summary>
public class PlayerJumpState : PlayerState
{
    private int amountOfJumpsLeft; // 남은 점프 횟수

    public PlayerJumpState(MovementController player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        amountOfJumpsLeft = player.amountOfJumps;
    }

    public override void Enter()
    {
        base.Enter();
        player.SetVelocityY(player.jumpForce); // 위로 힘을 가함
        amountOfJumpsLeft--;
        player.AirState.SetIsJumping(); // AirState에 점프 중임을 알림
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        // 일정 시간이 지나거나 땅에서 떨어지면 Air 상태로 전환
        if (Time.time > startTime + 0.1f || !player.CheckIfGrounded())
        {
             stateMachine.ChangeState(player.AirState);
        }
        
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // 점프 키를 뗐을 때 속도 감소 (Variable Jump Height)
        // PhysicsUpdate에서 처리하여 물리 연산 일관성 유지
        if (player.InputHandler.JumpInputStop)
        {
            if (player.CurrentVelocity.y > 0)
            {
                player.SetVelocityY(player.CurrentVelocity.y * player.variableJumpHeightMultiplier);
            }
            player.InputHandler.UseJumpInputStop();
        }
    }

    // 더블 점프 가능 여부 확인
    public bool CanDoubleJump()
    {
        if (player.canDoubleJump && amountOfJumpsLeft > 0)
        {
            return true;
        }
        return false;
    }

    // 땅에 닿았을 때 점프 횟수 리셋
    public void ResetJumps()
    {
        amountOfJumpsLeft = player.amountOfJumps;
    }
}
