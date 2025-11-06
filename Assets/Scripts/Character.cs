using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Character : MonoBehaviour
{
    // 상태 Enum: 캐릭터의 가능한 상태 정의
    enum State
    {
        Ible,   // 대기 상태
        Attack, // 공격 상태
        Trace,  // 추격 상태
        Return, // 복귀 상태
        Hit,    // 데미지를 받음
        Die     // 캐릭터 사망
    }
    // 상태 머신 컴포넌트
    private StateMachine stateMachine;


    //이동 속도
    [SerializeField] private float chspeed;

    //스프라이트(좌우 변경 위해서 필수)
    private SpriteRenderer srr;
    //땅 위에서 다녀야하니 필수
    private Rigidbody2D rb;

    // 캐릭터의 시작 위치
    private Vector2 startPos;


    
    [SerializeField] private DetectionZone detectionZone;

    //적은 다수 가까운 적을 먼저 때리게 만들기 위해 만들어진 놈
    [SerializeField] private List<Monsters> monsterInRange => detectionZone.monsterInRange;
    private Monsters nearest = null;
    [SerializeField] private int enemyCount;
    //적을 만나게 되면 공격 애니메이션 발동
    private Vector2 attackDir;                      //이동
    [SerializeField] private float attackLength;    //범위
    private RaycastHit2D attack;                    //공격
    public bool isInAttackRange = false;            //공격확인
    public LayerMask layerMask;                     //공격대상
    private Transform target;                       //가까운 적대상

    //애니
    private Animator ani;

    private void Awake()
    {
        // SpriteRenderer 가져오기
        srr = GetComponent<SpriteRenderer>();
        // Rigidbody2D 가져오기
        rb = GetComponent<Rigidbody2D>();
        // Animator 가져오기
        ani = GetComponent<Animator>();

        // StateMachine 컴포넌트 동적 추가
        stateMachine = gameObject.AddComponent<StateMachine>();


        // 상태 등록: 상태, Character 데이터를 StateMachine한테 전달
        stateMachine.AddState(State.Ible, new IbleState(this));
        stateMachine.AddState(State.Attack, new AttackState(this));
        stateMachine.AddState(State.Trace, new TraceState(this));
        stateMachine.AddState(State.Return, new ReturnState(this));

        // 초기 상태 설정: Idle 상태로 시작
        stateMachine.InitState(State.Ible);

        detectionZone = GetComponentInChildren<DetectionZone>();

    }
  
    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
       
    }

    private void FixedUpdate()
    {

        //가장 가까운 몬스터 찾기 시작
        float nearestDist = Mathf.Infinity; // 지금까지 최소 거리 (초기값: 무한대)
        nearest = null;         //대상 리셋
        enemyCount = 0;         //몬스터 수 리셋
        //사거리 안 모든 몬스터 하나씩 확인
        foreach (var monster in monsterInRange)
        {
            enemyCount++;
            // 죽어서 사라진 몬스터는 건너뜀
            if (monster == null) continue;

            // 캐릭터와 몬스터 사이 거리 계산
            float dist = Vector2.Distance(transform.position, monster.transform.position);

            // 더 가까운 몬스터 발견 → 갱신
            if (dist < nearestDist)
            {
                
                nearestDist = dist;    // 최소 거리 갱신
                nearest = monster;     // 가장 가까운 몬스터 갱신
                
            }
           

        }

        attackDir = srr.flipX ? Vector2.right : Vector2.left;
        attack = Physics2D.Raycast(transform.position, attackDir, attackLength, LayerMask.GetMask("Enemy"));
        

        if (attack.collider != null)
        {
            
            isInAttackRange = true;
        }
        if (attack.collider == null )
        {
            isInAttackRange = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Debug.DrawRay(transform.position, attackDir * attackLength, Color.red);
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

        // 시작 위치 속성
        protected Vector2 startPos
        {
            get { return owner.startPos; }  //위치 선정
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
        public int enemyCount
        {
            get { return owner.enemyCount; }
        }
        public bool isInAttackRange
        {
            get { return owner.isInAttackRange; }
        }

        public float attackLength
        {
            get { return owner.attackLength; }
        }

        public Animator ani
        {
            get { return owner.ani; }
        }

        public Transform target
        { get { return owner.target; } }
        public Monsters nearest
        {
            get { return owner.nearest; }
        }

        // 생성자: Character 데이터 저장
        public Player(Character owner)
        {
            this.owner = owner;
        }

        


    }

    //대기 상태
    private class IbleState : Player        //현재 플레이어는 대기 상태로 만들기 등록
    {
        //Character 데이터 받기
        public IbleState(Character owner) : base(owner) { }

        public override void Enter()
        {
            
            ani.SetBool("Move", false);
            ani.ResetTrigger("Attack");
            ani.Play("Idle", -1, 0f);
            rb.velocity = Vector2.zero;
        }

        public override void Transition()
        {
            //추적 시작
            if (nearest != null)
            {
               
                owner.target = nearest.transform;  // 타겟 설정
                ChangeState(State.Trace);          // 추적 상태로 전이
                
            }
           
        }
    }


    //추격 상태
    private class TraceState : Player
    {
        // 생성자: Character 데이터 받기
        public TraceState(Character owner) : base(owner) { }
        


        public override void Enter()
        {
           
            owner.ani.SetBool("Move", true);
        }
        public override void FixedUpdate()
        {
            Vector2 dir = (target.position - transform.position).normalized;
            rb.velocity = new Vector2(dir.x * chspeed, rb.velocity.y);
            srr.flipX = dir.x > 0;

            if (isInAttackRange)
            {
                ChangeState(State.Attack);
                
            }
           

           
        }
       



    }


    //복귀 상태
    private class ReturnState : Player
    {

        public ReturnState(Character owner) : base(owner) { }

        public override void Enter()
        {
            owner.ani.SetBool("Move", true);
            
        }

        public override void FixedUpdate()
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
                ChangeState(State.Ible);
                return;
            }
        }


    }

    private class AttackState : Player
    {
        private float attackDuration = 0.5f; // 공격 애니메이션 길이
        private float timer;


        public AttackState(Character owner) : base(owner) { }

        public override void Enter()   //공격 애니메이션
        {
            //공격 애니메이션 공격할 때 그 자리에서 공격하기

            rb.velocity = Vector2.zero;
            timer = 0;
            owner.ani.SetBool("Move", false);
            
        }

        public override void Update()  //데미지 처리
        {
            

            timer += Time.deltaTime; //공격딜레이 시작
            if (timer > attackDuration)
            {
                
                timer = 0;
                owner.ani.SetTrigger("Attack");
            }

            

        }

        public override void Transition()
        {
            if (enemyCount <= 0)
            {
                ChangeState(State.Ible);
               
            }

           

        }


    }

    private class DieState : Player
    {
        public DieState(Character owner) : base(owner) { }

        public override void Enter()
        {
            GameObject.Destroy(owner.gameObject);
        }
    }
}