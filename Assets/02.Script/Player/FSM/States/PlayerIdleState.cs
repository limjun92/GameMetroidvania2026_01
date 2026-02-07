using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어가 가만히 서 있는 상태입니다.
/// </summary>
public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(MovementController player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.SetVelocityZero(); // 대기 상태 진입 시 이동 정지
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (stateMachine.CurrentState != this) return;

        // 이동 입력이 들어오면 Move 상태로 전환
        if (input.x != 0)
        {
            stateMachine.ChangeState(player.MoveState);
        }
    }
}
