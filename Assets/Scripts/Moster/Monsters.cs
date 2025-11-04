using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;


public class Monsters : MonoBehaviour
{
    // 상태 Enum: 캐릭터의 가능한 상태 정의
    enum State
    {
        Ible,   // 대기 상태
        Attack, // 공격 상태
        Trace,  // 추격 상태
        Hit,    // 데미지를 받음
        Die     // 캐릭터 사망
    }

    

    //이동 속도
    [SerializeField] private float chspeed;
    //적 감지 범위
    [SerializeField] private float enemyRadius;

    //스프라이트(좌우 변경 위해서 필수)
    private SpriteRenderer srr;
    //땅 위에서 다녀야하니 필수
    private Rigidbody2D rb;
   
    // 상태 머신 컴포넌트
    private StateMachine stateMachine;
    // 캐릭터의 시작 위치
    private Vector2 startPos;


    //적을 만나게 되면 공격 애니메이션 발동
    private RaycastHit2D attack;
    [SerializeField] private float attackLength;


    //타겟(플레이어)
    [SerializeField] private Transform target;

   

    private Animator ani;

    private void Awake()
    {
        // 컴포넌트 초기화: SpriteRenderer 가져오기
        srr = GetComponent<SpriteRenderer>();
        // 컴포넌트 초기화: Rigidbody2D 가져오기
        rb = GetComponent<Rigidbody2D>();
        // StateMachine 컴포넌트 동적 추가
        stateMachine = gameObject.AddComponent<StateMachine>();

        ani = GetComponent<Animator>();


        // 상태 등록: Idle 상태, Moster 데이터를 StateMachine한테 전달
        stateMachine.AddState(State.Ible, new IbleState(this));
        stateMachine.AddState(State.Attack, new AttackState(this));
        stateMachine.AddState(State.Trace, new TraceState(this));

        // 초기 상태 설정: Idle 상태로 시작
        stateMachine.InitState(State.Ible);
    }

    void Start()
    {
        // 타겟 초기화: Enemy 태그 오브젝트 찾기, null 체크
        if (target == null)
        {
            target = GameObject.FindWithTag("Player")?.transform;
        }
        // 시작 위치 저장
        startPos = transform.position;
    }

    void Update()
    {
        // 캐릭터가 오른쪽을 보는 경우 (flipX = false)
        Vector2 attackDirection = srr.flipX ? Vector2.left : Vector2.right;
        
        // Raycast 방향을 캐릭터 방향에 따라 변경
        attack = Physics2D.Raycast(transform.position, attackDirection, attackLength);

        // Debug Ray도 동일한 방향으로 표시
        Debug.DrawRay(transform.position, attackDirection * attackLength, Color.red);

     
    }

    private void FixedUpdate()
    {
        
    }


    // Player: 상태 클래스의 중간 부모
    private class Monster : CharacterMainState   //StateMachine의 CharacterMainState클래스와 상속 중 확인
    {
        // Moster 스크립트 참조
        protected Monsters owner;  //캐릭터 클래스는

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

        //공격관련
        public RaycastHit2D attack
        {
            get { return owner.attack; }
        }

        public float attackLength
        {
            get { return owner.attackLength; }
        }

        public Animator ani
        {
            get { return owner.ani; }
        }

        // 생성자: Character 데이터 저장
        public Monster(Monsters owner)
        {
            this.owner = owner;
        }

       

    }

    //대기 상태
    private class IbleState : Monster        //현재 몬스터는 대기 상태로 만들기 등록
    {
        //Monsters 데이터 받기
        public IbleState(Monsters owner) : base(owner) { }

        public override void Update()
        {
            owner.ani.SetBool("Move", false);
        }
        
        public override void Transition()
        {
           

            //플레이어가 살아있다면
            if (Vector2.Distance(target.position, transform.position) < enemyRadius)
            {
                ChangeState(State.Trace);
            }
        }
    }


    //추격 상태
    private class TraceState : Monster
    {
        // 생성자: Monster 데이터 받기
        public TraceState(Monsters owner) : base(owner) { }
        Vector2 attackDirection;
        Vector2 rayStart;
        RaycastHit2D hit;
        Vector2 dir;
        public override void Update()
        {
           
            owner.ani.SetBool("Move", true);
            dir = (target.position - transform.position).normalized;
            rb.velocity = new Vector2(dir.x * chspeed, rb.velocity.y);
            
            
        }
        public override void FixedUpdate()
        {
            srr.flipX = dir.x < 0;
            attackDirection = srr.flipX ? Vector2.left : Vector2.right;
            rayStart = (Vector2)transform.position + attackDirection;
            hit = Physics2D.Raycast(rayStart, attackDirection, owner.attackLength);
        }
        public override void Transition()
        {
           
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                ChangeState(State.Attack);
                return;
            }
        }
    }


    //복귀 상태
    

    private class AttackState : Monster
    {
        private float attackDuration = 0.5f; // 공격 애니메이션 길이
        private float timer;
        

        public AttackState(Monsters owner) : base(owner) { }

        public override void Enter()   //공격 애니메이션
        {
            //공격 애니메이션 공격할 때 그 자리에서 공격하기
            
            rb.velocity = Vector2.zero;
            timer = 0;
            owner.ani.SetBool("Move", false);
            owner.ani.SetTrigger("Attack");
        }

        public override void Update()  //데미지 처리
        {
           
            timer += Time.deltaTime; //공격딜레이 시작
            if (timer > attackDuration)
            {
                owner.srr.flipX = false;    
                
                timer = 0;
                ChangeState(State.Ible);
            }

            
        }

       

        
    }

    private  class DieState : Monster
    {
        public DieState(Monsters owner) : base(owner) { }

        public override void Enter()
        {
            GameObject.Destroy(owner.gameObject);
        }
    }
}