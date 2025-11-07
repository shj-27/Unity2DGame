using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MosterBase : MonoBehaviour
{

    [SerializeField] private MonsterData monsterData;

    [SerializeField] private string monsterName;        //몬스터이름
    [SerializeField] private string monsterColor;       //몬스터이름
    [SerializeField] private int monsterMaxHp;          //몬스터 최대 HP
    [SerializeField] private int currentHp;             //몬스터 현재 체력
    [SerializeField] private List<GameObject> monster;
    [SerializeField] private int monsterCount;
    [SerializeField] private Transform spawnPoint;
    private Character character;
    private int mon;
    [SerializeField] private int currentMonsterId;
    private bool[] monstering = new bool[3];
    // Start is called before the first frame update

    private void Awake()
    {
        
        character = FindObjectOfType<Character>();
    }

    void Start()
    {
        SpawnNewMonster();
       


    }

    // Update is called once per frame
    void Update()
    {
        if (character.isInAttackRange)      //충돌 하고 있는 애가
        {
            if (character.damageTriggered)  //데미지 애니메이션 종료하고
            {
                if (monstering[currentMonsterId]==character.weapon[currentMonsterId])
                {
                    currentHp -= 10; // ← 모든 색상 10 데미지!
                    character.damageTriggered = false;
                    if (currentHp <= 0)
                    {
                        monstering[currentMonsterId] = false;
                        monster[currentMonsterId].SetActive(false);
                        monsterCount--;
                    }

                }
                else
                {
                    character.damageTriggered = false;
                }
            }
            
        }

        if (monsterCount < 1)
        {
            SpawnNewMonster();
        }

        
    }

    public int Randoms(int a, int b)
    {
        int ab = Random.Range(a, b);
        return ab;
    }

    void SpawnNewMonster()
    {
        // 랜덤 몬스터 선택
        mon = Randoms(0, monsterData.monsterName.Length);
        currentMonsterId = mon; // 현재 몬스터 ID 저장
        monstering[currentMonsterId] = true;
        // 데이터 갱신
        monsterName = monsterData.monsterName[currentMonsterId];
        switch (currentMonsterId)
        {
            case 0:
                monsterData.color = MonsterData.Colors.Blue; break;
            case 1:
                monsterData.color = MonsterData.Colors.Red; break;
            case 2:
                monsterData.color = MonsterData.Colors.White; break;
        }
        monsterColor = monsterData.color.ToString();

        // HP 설정
        monsterMaxHp = Randoms(10, 101);
        currentHp = monsterMaxHp;

        // 몬스터 활성화
        monster[currentMonsterId].transform.position = spawnPoint.position;
        monster[currentMonsterId].SetActive(true);
        monsterCount ++;

       
    }
}
