using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Character : MonoBehaviour
{
    // 상태 Enum: 캐릭터의 가능한 상태 정의
    enum State
    {
        Idle,   // 대기 상태
        Walk,   // 이동 상태
        Attack, // 공격 상태 (현재 미사용)
        Trace,  // 추격 상태
        Return, // 복귀 상태
        Die     // 캐릭터 사망
    }

    //타겟(적)
    [SerializeField] private Transform target;

    //이동 속도
    [SerializeField] private float chspeed;
    //적 감지 범위
    [SerializeField] private float enemyRadius;

    //스프라이트(좌우 변경 위해서 필수)
    private SpriteRenderer srr;
    //땅 위에서 다녀야하니 필수
    private Rigidbody2D rb;
    //적은 다수 가까운 적을 먼저 때리게 만들기 위해 만들어진 놈
    [SerializeField] private GameObject[] enemy;
    // 상태 머신 컴포넌트
    private StateMachine stateMachine;
    // 캐릭터의 시작 위치
    private Vector2 startPos;

    //적을 만나게 되면 공격 애니메이션 발동
    private Ray2D Attack;

    private void Awake()
    {
        // 컴포넌트 초기화: SpriteRenderer 가져오기
        srr = GetComponent<SpriteRenderer>();
        // 컴포넌트 초기화: Rigidbody2D 가져오기
        rb = GetComponent<Rigidbody2D>();
        // StateMachine 컴포넌트 동적 추가
        stateMachine = gameObject.AddComponent<StateMachine>();



        // 상태 등록: Idle 상태, Character 데이터를 StateMachine한테 전달
        stateMachine.AddState(State.Idle, new IdleState(this)); 
       
        stateMachine.AddState(State.Trace, new TraceState(this));
        stateMachine.AddState(State.Return, new ReturnState(this));

        // 초기 상태 설정: Idle 상태로 시작
        stateMachine.InitState(State.Idle);
    }

    void Start()
    {
        // 타겟 초기화: Enemy 태그 오브젝트 찾기, null 체크
        if (target == null)
        {
            target = GameObject.FindWithTag("Enemy")?.transform;
        }
        // 시작 위치 저장
        startPos = transform.position;
    }

    void Update()
    {
        // 매 프레임: StateMachine.Update가 상태의 Update/Transition 호출
    }

    private void FixedUpdate()
    {
        // 물리 업데이트: x축 이동, horizontal 입력 사용
        //rb.velocity = new Vector2(horizontal * chspeed, rb.velocity.y);
    }


    // Player: 상태 클래스의 중간 부모
    private class Player : CharacterMainState   //StateMachine의 CharacterMainState클래스와 상속 중 확인
    {
        // Character 스크립트 참조
        protected Character owner;  //캐릭터 클래스는

        // transform 속성: Character의 transform 반환
        protected Transform transform
        {
            get { return owner.transform; } //위치 가져오기
        }

        // 이동 속도 속성
        protected float chspeed
        {
            get { return owner.chspeed; }   //속도 가져오기
        }

        // 타겟 Transform 속성
        protected Transform target
        {
            get { return owner.target; }    //대상 타겟 가져오기
        }

        // 시작 위치 속성
        protected Vector2 startPos
        {
            get { return owner.startPos; }  //위치 선정
        }

        // 적 감지 범위 속성
        protected float enemyRadius
        {
            get { return owner.enemyRadius; }   //적 감지범위
        }


        public Rigidbody2D rb
        {
            get { return owner.rb; }          //중력 설정
        }

        public SpriteRenderer srr
        {
            get { return owner.srr; }
        }

        // 생성자: Character 데이터 저장
        public Player(Character owner)
        {
            this.owner = owner;
        }

    }

    //대기 상태
    private class IdleState : Player        //현재 플레이어는 대기 상태로 만들기 등록
    {
        // 생성자: Character 데이터 받기
        public IdleState(Character owner) : base(owner) { }

        public override void Transition()
        {
            //플레이어가 일정거리 안에 들어오면 추적상태로 전환
            if (Vector2.Distance(target.position, transform.position) < enemyRadius)
            {
                ChangeState(State.Trace);
            }
        }
    }


    //추격 상태
    private class TraceState : Player
    {
        // 생성자: Character 데이터 받기
        public TraceState(Character owner) : base(owner) { }
        
        public override void Update()
        {
            
            Vector2 dir = (target.position - transform.position).normalized;
            // y축 제어: 수평만 이동
            Debug.Log("발동중");
            
            
            rb.velocity = new Vector2(dir.x * chspeed, rb.velocity.y);
            
            if(dir.x < 0)
            {
                owner.srr.flipX = true;
            }
            else
            {
                owner.srr.flipX = false;
            }
        }

        public override void Transition()
        {
            // Transition: 타겟이 범위를 벗어나면 Return 상태로 전이
            if (Vector2.Distance(target.position, transform.position) > enemyRadius)
            {
                ChangeState(State.Return);
            }
        }
    }

    //복귀 상태
    private class ReturnState : Player
    {
       
        public ReturnState(Character owner) : base(owner) { }

        public override void Update()
        {
            // Update: 시작 위치로 이동
            Vector2 dir = ((Vector3)startPos - transform.position).normalized;
            transform.Translate(dir * chspeed * Time.deltaTime, Space.World);
        }

        public override void Transition()
        {
            // Transition: 시작 위치 도달 시 Idle 상태로 전이
            if (Vector2.Distance(startPos, transform.position) < 0.1f)
            {
                ChangeState(State.Idle);
            }
        }
    }
}