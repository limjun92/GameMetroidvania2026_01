using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 벽 타기 (Wall Slide) 상태입니다.
/// 벽에 붙어서 천천히 미끄러져 내려오는 로직을 처리합니다.
/// </summary>
public class PlayerWallSlideState : PlayerState
{
    public PlayerWallSlideState(MovementController player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        // 벽에 닿는 순간 상승 관성을 초기화
        player.SetVelocityY(0f);
        
        // 벽 점프가 해금되었을 때만 중력을 0으로 설정하여 완벽하게 멈춤
        if (player.isWallJumpUnlocked)
        {
            player.RB.gravityScale = 0f;
        }

        // 벽에 붙으면 공중 대시 횟수 초기화
        player.hasDashedInAir = false;  

        // 벽에 붙으면 2단 점프 횟수 초기화
        player.JumpState.ResetJumps();
    }

    public override void Exit()
    {
        base.Exit();
        // 상태를 나갈 때 중력 복구
        player.RB.gravityScale = player.defaultGravityScale;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // 사용자 입력 확인
        float xInput = player.InputHandler.MovementInput.x;

        // 벽 점프 입력 확인 (해금되었을 때만)
        if (player.isWallJumpUnlocked && player.InputHandler.JumpInput)
        {
            player.InputHandler.UseJumpInput();
            stateMachine.ChangeState(player.WallJumpState);
            return;
        }

        // 벽 대시 입력 확인 (해금되었을 때만)
        if (player.isDashUnlocked && player.InputHandler.DashInput && player.DashState.CheckIfCanDash())
        {
            player.Flip();
            stateMachine.ChangeState(player.DashState);
            return;
        }

        // 벽에서 떨어지거나, 땅에 닿거나, 벽 반대 방향으로 입력을 주면 Air/Idle 상태로 전환
        // 주의: 벽 타기는 "벽 쪽으로 입력을 유지"해야 함
        if (!player.CheckIfTouchingWall() || (xInput != 0 && xInput != (player.FacingRight ? 1 : -1)))
        {
            stateMachine.ChangeState(player.AirState);
            return; 
        }
        else if (player.CheckIfGrounded())
        {
            stateMachine.ChangeState(player.IdleState);
            return; 
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // 벽 타기 중에는 Y 속도를 제한하여 천천히 미끄러짐
        
        // 벽 점프가 해금되었을 때: 벽 붙기(Stick) 시간 적용
        if (player.isWallJumpUnlocked && Time.time < startTime + player.wallStickTime)
        {
            player.SetVelocityY(0f);
            player.RB.gravityScale = 0f; // 중력 0 유지
        }
        else
        {
            player.RB.gravityScale = player.defaultGravityScale; // 중력 복구 (혹은 원래대로)
            player.SetVelocityY(-player.wallSlideSpeed);
        }
    }
}
