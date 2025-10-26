using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    //상태이름을 키, 상태의 객체를 값으로저장
    private Dictionary<string, CharacterMainState> stateDic = new Dictionary<string, CharacterMainState>();

    private CharacterMainState curState;
    void Start()
    {
        curState.Enter();   // 시작시 나오는 상태
    }

    void Update()
    {
        //현재 상태가 해야할 행동 실행
        curState.Update();
        //상태 전이(변경) 조건확인. 필요하면 다른 상태로 바꾸는 로직을 판단
        curState.Transition();
    }
    private void LateUpdate()
    {
        curState.LateUpdate();
    }
    private void FixedUpdate()
    {
        curState.FixedUpdate();
    }
    public void InitState(string stateName)
    {
        curState = stateDic[stateName];
    }
    //상태를 상태머신에 등록
    public void AddState(string stateName, CharacterMainState state)
    {
        state.SetStateMachine(this);
        stateDic.Add(stateName, state);
    }
    //변경
    public void ChangeState(string stateName)
    {
        curState.Exit();
        //딕셔너리에서 새 상태를 꺼내 현재 상태로 교체
        curState = stateDic[stateName];

        curState.Enter();
    }

    public void InitState<T>(T stateType) where T : Enum
    {
        InitState(stateType.ToString());
    }
    public void AddState<T>(T stateType, CharacterMainState state) where T : Enum
    {
        AddState(stateType.ToString(), state);
    }
    public void ChangeState<T>(T stateType) where T : Enum
    {
        ChangeState(stateType.ToString());
    }

}


public class CharacterMainState //상태 등록을 위한 클래스
{
    //캐릭터 상태 변경 변수 설정
    private StateMachine stateMachine;

    public void SetStateMachine(StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }
    //상태전이
    protected void ChangeState(string stateName)
    {
        //내부에 저장된 상태머신한테 이 이름 상태로 바꿈
        stateMachine.ChangeState(stateName);
    }
    protected void ChangeState<T>(T stateType) where T : Enum
    {
        ChangeState(stateType.ToString());
    }
    ///////////////////////////////////
    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void LateUpdate() { }
    public virtual void FixedUpdate() { }
    public virtual void Exit() { }
    public virtual void Transition() { }

}
