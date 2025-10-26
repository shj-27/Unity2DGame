using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Character : MonoBehaviour
{
    enum State
    {
        Idle,   //대기
        Walk,   //이동
        Attack, //공격
        Trace,  //추격
        Return
    }

    [SerializeField] private Transform target; //적 찾기
    
    
    [SerializeField] private float chspeed;     //이동속도
    [SerializeField] private float horizontal;  //x값이동

    [SerializeField] private float enemyRadius; //범위



    private SpriteRenderer srr;
    private Rigidbody2D rb;
    [SerializeField] private Transform gizmo;

    private StateMachine stateMachine;
    private Vector2 startPos;

    private void Awake()
    {
        srr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        stateMachine.AddState(State.Idle, new IdleState(this)); //대기
        stateMachine.AddState(State.Idle, new TraceState(this)); //대기
        stateMachine.AddState(State.Idle, new IdleState(this)); //대기
        
        


    }
    // Start is called before the first frame update
    void Start()
    {
    
    }

    
    // Update is called once per frame
    void Update()
    {
    
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal * chspeed, rb.velocity.y);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(gizmo.position, enemyRadius);
    }

    private class Player : CharacterMainState
    {
        protected Character owner;

        protected Transform transform
        {
            get { return transform; }
        }

        protected float chspeed
        {
            get { return owner.chspeed; }
        }

       
        protected Transform target
        {
            get { return owner.target; }
        }

        protected Vector2 startPos
        {
            get { return owner.startPos; }
        }

        protected float enemyRadius
        {
            get { return owner.enemyRadius; }
        }

        public Player(Character owner)
        {
            this.owner = owner;
        }


    }

    private class IdleState : Player
    {
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
    private class TraceState : Player
    {
        public TraceState(Character owner) : base(owner) { }

        public override void Update()
        {
            //방향계산
            Vector2 dir = (target.position - transform.position).normalized;
            //이동
            transform.Translate(dir * chspeed * Time.deltaTime, Space.World);
        }
        public override void Transition()
        {
            //탐지 범위를 벗어나면 복귀상태로 전환
            if (Vector2.Distance(target.position, transform.position) > enemyRadius)
            {
                ChangeState(State.Return);
            }
        }
    }
    private class ReturnState : Player
    {
        public ReturnState(Character owner) : base(owner) { }

        public override void Update()
        {
            Vector2 dir = ((Vector3)startPos - transform.position).normalized;

            transform.Translate(dir * chspeed * Time.deltaTime, Space.World);
        }
        public override void Transition()
        {
            if (Vector2.Distance(startPos, transform.position) < 0.1f)
            {
                ChangeState(State.Idle);
            }
        }
    }
}
