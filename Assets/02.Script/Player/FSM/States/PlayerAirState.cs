using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 공중에 있는 상태입니다. (점프 후 낙하, 그냥 떨어짐 등)
/// 공중 이동 및 더블 점프, 공중 대시 입력을 처리합니다.
/// </summary>
public class PlayerAirState : PlayerState
{
    public PlayerAirState(MovementController player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    private bool isJumping; // 점프 상승 중인지 여부

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // 공중 이동 처리
        Vector2 input = player.InputHandler.MovementInput;
        player.CheckIfShouldFlip(input.x);
        player.SetVelocityX(player.moveSpeed * input.x);

        if (isJumping)
        {
            // 상승하다가 y속도가 음수가 되면 낙하 시작
            if (player.CurrentVelocity.y < 0)
                isJumping = false;
        }

        // 땅에 닿으면 Idle 상태로 전환
        if (player.CheckIfGrounded() && player.CurrentVelocity.y < 0.01f && !isJumping)
        {
            stateMachine.ChangeState(player.IdleState);
        }
        // 더블 점프 입력 처리
        else if (player.InputHandler.JumpInput && player.JumpState.CanDoubleJump())
        {
            player.InputHandler.UseJumpInput();
            stateMachine.ChangeState(player.JumpState);
        }
        // 공중 대시 입력 처리 (해금 여부, 쿨타임, 공중 사용 여부 확인)
        else if (player.InputHandler.DashInput && player.isDashUnlocked && player.DashState.CheckIfCanDash() && !player.hasDashedInAir)
        {
            player.InputHandler.UseDashInput();
            stateMachine.ChangeState(player.DashState);
        }
    }

    public void SetIsJumping()
    {
        isJumping = true;
    }
}
