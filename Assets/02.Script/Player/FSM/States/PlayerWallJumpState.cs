using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 벽 점프 (Wall Jump) 상태입니다.
/// 벽에서 반대 방향으로 튀어오르는 로직을 처리합니다.
/// </summary>
public class PlayerWallJumpState : PlayerState
{
    public PlayerWallJumpState(MovementController player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        // 벽 점프 수행
        // 현재 바라보는 방향의 반대 방향으로 힘을 가함
        int wallJumpDirection = player.FacingRight ? -1 : 1;
        Vector2 force = new Vector2(player.wallJumpDirection.x * wallJumpDirection, player.wallJumpDirection.y);
        force.Normalize();
        force *= player.wallJumpForce;

        player.SetVelocity(force.x, force.y); // X, Y 속도 직접 설정
        
        // 캐릭터 방향 뒤집기 (벽 반대 등지고 점프하므로)
        player.CheckIfShouldFlip(wallJumpDirection);
        
        // 점프 횟수 감소 (선택 사항, 벽 점프 후 더블 점프 허용 여부에 따라 다름)
        // 점프 중임을 AirState에 알림 (AirState로 전환된 후에도 Variable Jump Height 적용을 위해)
        player.AirState.SetIsJumping();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // 일정 시간 후 AirState로 전환하여 자유 이동 허용
        if (Time.time >= startTime + player.wallJumpTime)
        {
            stateMachine.ChangeState(player.AirState);
        }

        // 벽 점프 후 대시 입력 처리 (방향 전환 포함)
        if (player.InputHandler.DashInput && player.isDashUnlocked && player.DashState.CheckIfCanDash() && !player.hasDashedInAir)
        {
            // 입력 방향으로 회전
            float xInput = player.InputHandler.MovementInput.x;
            if (xInput != 0)
            {
                player.CheckIfShouldFlip(xInput);
            }
            
            player.InputHandler.UseDashInput();
            stateMachine.ChangeState(player.DashState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // 벽 점프 중 점프 키를 뗐을 때 속도 감소 (Variable Jump Height)
        if (player.InputHandler.JumpInputStop)
        {
            if (player.CurrentVelocity.y > 0)
            {
                player.SetVelocityY(player.CurrentVelocity.y * player.variableJumpHeightMultiplier);
            }
            player.InputHandler.UseJumpInputStop();
        }
    }
}
