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
    private int currentMonsterId;
    // Start is called before the first frame update

    private void Awake()
    {
        
        character = FindObjectOfType<Character>();
    }

    void Start()
    {
        SpawnNewMonster();
        mon = Randoms(0, monsterData.monsterName.Length);
        currentMonsterId = mon;
        monsterName = monsterData.monsterName[mon];
        switch (mon)
        {
            case 1:
                monsterData.color = MonsterData.Colors.Blue; break;
            case 2:
                monsterData.color = MonsterData.Colors.Red; break;
            case 3:
                monsterData.color = MonsterData.Colors.White; break;
        }

        monsterColor = monsterData.color.ToString();


    }

    // Update is called once per frame
    void Update()
    {
        if (character.isInAttackRange)
        {
            for (int i = 0; i < 3; i++)
            {
                if (character.weapon[i])
                {
                    currentHp -= 10; // ← 모든 색상 10 데미지!
                    
                    if (currentHp <= 0)
                    {
                        monster[mon].SetActive(false);
                        monsterCount--;
                    }
                    break;
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

        // 데이터 갱신
        monsterName = monsterData.monsterName[mon];
        monsterData.color = (MonsterData.Colors)mon;
        monsterColor = monsterData.color.ToString();

        // HP 설정
        monsterMaxHp = Randoms(10, 101);
        currentHp = monsterMaxHp;

        // 몬스터 활성화
        monster[mon].transform.position = spawnPoint.position;
        monster[mon].SetActive(true);
        monsterCount =1;

       
    }
}
