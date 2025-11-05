using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRange : MonoBehaviour
{
    //스프라이트(좌우 변경 위해서 필수)

    private Character character;
    private SpriteRenderer srr => character.srr;


    //적을 만나게 되면 공격 애니메이션 발동
    private Vector2 dir;
    private Vector2 attackDir;
    [SerializeField] private float attackLength;
    private RaycastHit2D attack;
    // Start is called before the first frame update
    void Start()
    {

        character = GetComponent<Character>();
    }

    // Update is called once per frame
    void Update()
    {
       
        attackDir = srr.flipX ? Vector2.right : Vector2.left;

        attack = Physics2D.Raycast(transform.position, attackDir, attackLength);
        Debug.DrawRay(transform.position, attackDir * attackLength, Color.red);
    }
    private void OnDrawGizmos()
    {
        if (attack.collider != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(attack.point, 0.2f);
        }
    }
}
