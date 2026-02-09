using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어가 좌우로 이동하는 상태입니다.
/// </summary>
public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(MovementController player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (stateMachine.CurrentState != this) return;

        // 입력에 따라 방향 전환 (Visual)
        player.CheckIfShouldFlip(input.x);

        // 입력이 없으면 Idle 상태로 전환
        if (input.x == 0)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        // 물리 이동은 FixedUpdate에서 처리
        player.SetVelocityX(player.moveSpeed * input.x);
    }
}
