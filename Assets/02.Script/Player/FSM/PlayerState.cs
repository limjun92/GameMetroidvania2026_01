using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 플레이어 상태의 기본이 되는 추상 클래스입니다.
/// 공통적인 참조(Player, StateMachine)와 필수 메서드들을 정의합니다.
/// </summary>
public abstract class PlayerState
{
    protected MovementController player;
    protected PlayerStateMachine stateMachine;
    protected string animBoolName; // 애니메이션 파라미터 이름

    protected float startTime; // 상태 진입 시간

    public PlayerState(MovementController player, PlayerStateMachine stateMachine, string animBoolName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    // 상태에 진입할 때 호출됩니다.
    public virtual void Enter()
    {
        startTime = Time.time;
        // player.Anim.SetBool(animBoolName, true); // 나중에 애니메이션 추가 시 사용
        // Debug.Log($"Enter State: {animBoolName}");
    }

    // 상태에서 나갈 때 호출됩니다.
    public virtual void Exit()
    {
        // player.Anim.SetBool(animBoolName, false);
    }

    // 매 프레임(Update) 호출되는 로직입니다. 입력 확인 및 상태 변경 로직이 들어갑니다.
    public virtual void LogicUpdate()
    {

    }

    // 매 물리 프레임(FixedUpdate) 호출되는 로직입니다. 물리 연산이 들어갑니다.
    public virtual void PhysicsUpdate()
    {

    }
    
    // Input System 이벤트 처리를 위한 메서드들? 
    // 혹은 LogicUpdate에서 Input을 체크/처리할 수 있음.
}
