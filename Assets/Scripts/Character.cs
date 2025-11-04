using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEngine.GraphicsBuffer;


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

    private Transform target;

    //적을 만나게 되면 공격 애니메이션 발동
    private RaycastHit2D attack;
    [SerializeField] private float attackLength;

    //적은 다수 가까운 적을 먼저 때리게 만들기 위해 만들어진 놈
    private List<Monsters> monsterInRange = new List<Monsters>();
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


        // 상태 등록: Idle 상태, Character 데이터를 StateMachine한테 전달
        stateMachine.AddState(State.Ible, new IbleState(this));
        stateMachine.AddState(State.Attack, new AttackState(this));
        stateMachine.AddState(State.Trace, new TraceState(this));
        stateMachine.AddState(State.Return, new ReturnState(this));

        // 초기 상태 설정: Idle 상태로 시작
        stateMachine.InitState(State.Ible);
    }

    void Start()
    {
        // 타겟 초기화: Enemy 태그 오브젝트 찾기, null 체크
        //Monsters target = ;
        //if (target == null) return; //타겟이 없다면 아무것도 하지말기
        //monsterInRange;
        // 시작 위치 저장
        startPos = transform.position;
    }

    void Update()
    {
        // 캐릭터가 오른쪽을 보는 경우 (flipX = false)
        Vector2 attackDirection = srr.flipX ? Vector2.right : Vector2.left;
        
        // Raycast 방향을 캐릭터 방향에 따라 변경
        attack = Physics2D.Raycast(transform.position, attackDirection, attackLength);

        // Debug Ray도 동일한 방향으로 표시
        Debug.DrawRay(transform.position, attackDirection * attackLength, Color.red);

     
    }

    private void FixedUpdate()
    {
        
    }

    //적 감지
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent<Monsters>(out var monster) && !monsterInRange.Contains(monster))
            monsterInRange.Add(monster); // 사거리 안 → 리스트 추가
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.TryGetComponent<Monsters>(out var monster))
            monsterInRange.Remove(monster); // 사거리 밖 → 리스트 제거
    }

    private Monsters GetNearMoneter()  //애는 가까운 몬스터를 찾는 역할!
    {
        if (monsterInRange.Count == 0) return null; // 몬스터 없으면 null 반환

        Monsters nearMonster = null;             // 가장 가까운 몬스터 저장용
        float nearDist = Mathf.Infinity;            // 지금까지의 최소 거리 (무한대로 초기화)
        Vector2 towerPos = transform.position;      // 타워 위치 (성능을 위해 매번 계산 방지)

        // 사거리 안 모든 몬스터를 하나씩 확인
        foreach (var monster in monsterInRange)
        {
            if (monster == null) continue;          // 몬스터가 죽어서 파괴된 경우 스킵

            // 현재 몬스터와 타워 사이 거리 계산
            float dist = Vector2.Distance(towerPos, monster.transform.position);

            // 더 가까운 몬스터 발견 시 갱신
            if (dist < nearDist)
            {
                nearDist = dist;        // 최소 거리 갱신
                nearMonster = monster;  // 가장 가까운 몬스터 갱신
            }
        }
        return nearMonster; // 최종적으로 가장 가까운 몬스터 반환
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

        protected Transform target
        {
            get { return owner.target; }  //위치 선정
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

        public List<Monsters> monsterInRange
        {
            get { return owner.monsterInRange; }
        }

        // 생성자: Character 데이터 저장
        public Player(Character owner)
        {
            this.owner = owner;
        }

       

    }
   

    //대기 상태
    private class IbleState : Player        //현재 플레이어는 대기 상태로 만들기 등록 and GetNearMoneter()에서 가까운 몬스터를 추격하도록만들기
    {
        //Character 데이터 받기
        public IbleState(Character owner) : base(owner) { }

        public override void Update()
        {
            owner.ani.SetBool("Move", false);
        }
        
        public override void Transition()
        {
            // monsterInRange의 리스트에 적이 있다면 감지 추격하기 시작
            if (monsterInRange.Count > 0)
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

            
            owner.ani.SetBool("Move", true);
            // 가까운 적 찾기!
            Monsters nearest = owner.GetNearMoneter();
            if (nearest == null)
            {
                ChangeState(State.Return);
                return;
            }
            Vector2 dir = (transform.position).normalized;
            rb.velocity = new Vector2(dir.x * chspeed, rb.velocity.y);
            Debug.Log(dir.x);
            if (dir.x > 0)
            {
                owner.srr.flipX = true;
               
            }
            else
            {
                owner.srr.flipX = false;
            }

            Vector2 attackDirection = srr.flipX ? Vector2.right : Vector2.left;
            Vector2 rayStart = (Vector2)transform.position + attackDirection;
            RaycastHit2D hit = Physics2D.Raycast(rayStart, attackDirection, owner.attackLength);
            //나는 지금 적을 만나버렸어요 공격하겠습니다
           
            if(hit.collider != null && hit.collider.CompareTag("Enemy"))
            {
               
                ChangeState(State.Attack);
                return;
            }
           
        }   
    }


    //복귀 상태
    private class ReturnState : Player
    {
       
        public ReturnState(Character owner) : base(owner) { }

        public override void Update()
        {
            if (monsterInRange.Count == 0) return;  // 몬스터 없으면 null 반환

            Monsters nearest = null;             // 가장 가까운 몬스터 저장용
            float minDist = Mathf.Infinity;         // 지금까지의 최소 거리 (무한대로 초기화)
            Vector2 myPos = transform.position;   // 유닛 위치 (성능을 위해 매번 계산 방지)
            foreach (var m in monsterInRange)
            {
                if (m == null) continue;
                float dist = Vector2.Distance(myPos, m.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = m;
                }
            }

            // === 추적 ===
            if (nearest != null)
            {
                owner.target = nearest.transform;
                Vector2 dir = (owner.target.position - transform.position).normalized;
                rb.velocity = new Vector2(dir.x * chspeed, rb.velocity.y);
                srr.flipX = dir.x < 0;
                ani.SetBool("Move", true);

                // === 공격 감지 ===
                Vector2 attackDir = srr.flipX ? Vector2.left : Vector2.right;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, attackDir, attackLength);
                if (hit.collider != null && hit.collider.CompareTag("Enemy"))
                {
                    ChangeState(State.Attack);
                    return;
                }
            }
        }

        public override void Transition()
        {
            // Transition: 시작 위치 도달 시 Idle 상태로 전이
            if (Vector2.Distance(startPos, transform.position) < 0.1f)
            {
                ChangeState(State.Ible);
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

    private  class DieState : Player
    {
        public DieState(Character owner) : base(owner) { }

        public override void Enter()
        {
            GameObject.Destroy(owner.gameObject);
        }
    }
}