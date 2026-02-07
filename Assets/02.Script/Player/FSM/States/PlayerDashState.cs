using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 대시를 수행하는 상태입니다. 중력을 무시하고 빠르게 이동합니다.
/// </summary>
public class PlayerDashState : PlayerState
{
    private float lastDashTime; // 마지막 대시 시간 (쿨타임 계산용)
    
    public PlayerDashState(MovementController player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.InputHandler.UseDashInput();
        
        // 바라보는 방향으로 대시
        float direction = player.FacingRight ? 1 : -1;
        player.SetVelocity(player.dashSpeed * direction, 0); 
        player.RB.gravityScale = 0; // 대시 중에는 중력 무시
        
        lastDashTime = Time.time;

        // 공중에서 사용했다면 공중 대시 사용 처리
        if (!player.CheckIfGrounded())
        {
            player.hasDashedInAir = true;
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.RB.gravityScale = player.defaultGravityScale; // 중력 복구
        player.SetVelocityZero(); // 대시 끝지 정지
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // 대시 지속 시간이 끝나면 상태 전환
        if (Time.time >= startTime + player.dashDuration)
        {
            if (player.CheckIfGrounded())
                stateMachine.ChangeState(player.IdleState);
            else
                stateMachine.ChangeState(player.AirState);
        }
    }

    // 대시 쿨타임 확인 메서드
    public bool CheckIfCanDash()
    {
        return Time.time >= lastDashTime + player.dashCooldown;
    }
}
