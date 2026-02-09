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

        float inputX = player.InputHandler.MovementInput.x;
        player.CheckIfShouldFlip(inputX);

        // Wall Slide 전환
        // 땅에 있지 않고, 벽에 붙어 있고, 벽 쪽으로 입력을 하고 있을 때 (상승 중이더라도 붙음)
        if (!player.CheckIfGrounded() && player.CheckIfTouchingWall())
        {
            // 벽 쪽으로 입력 확인 (오른쪽 보고 있을 때 오른쪽 키, 왼쪽 보고 있을 때 왼쪽 키)
            if (inputX != 0 && inputX == (player.FacingRight ? 1 : -1))
            {
                stateMachine.ChangeState(player.WallSlideState);
            }
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

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // 공중 이동 물리 처리
        Vector2 input = player.InputHandler.MovementInput;
        player.SetVelocityX(player.moveSpeed * input.x);

        if (isJumping)
        {
             // 상승하다가 y속도가 음수가 되면 낙하 시작
            if (player.CurrentVelocity.y < 0)
            {
                isJumping = false;
            }
            // 점프 중 점프 키를 뗐을 때 속도 감소 (Variable Jump Height)
            // PhysicsUpdate에서 처리
            else if (player.InputHandler.JumpInputStop)
            {
                if (player.CurrentVelocity.y > 0)
                {
                    player.SetVelocityY(player.CurrentVelocity.y * player.variableJumpHeightMultiplier);
                    isJumping = false; 
                }
                player.InputHandler.UseJumpInputStop();
            }
        }
    }

    public void SetIsJumping()
    {
        isJumping = true;
    }
}
