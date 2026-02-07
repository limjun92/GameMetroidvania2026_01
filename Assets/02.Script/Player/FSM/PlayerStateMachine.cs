using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 현재 상태를 관리하고 상태 전이를 담당하는 클래스입니다.
/// </summary>
public class PlayerStateMachine
{
    // 현재 활성화된 상태
    public PlayerState CurrentState { get; private set; }

    // 초기 상태를 설정하고 실행합니다.
    public void Initialize(PlayerState startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    // 새로운 상태로 변경합니다. (이전 상태 Exit -> 새 상태 Enter)
    public void ChangeState(PlayerState newState)
    {
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}
