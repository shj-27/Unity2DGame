using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MosterData", menuName = "SO/MonsterData")]
public class MonsterData : ScriptableObject
{
    //몬스터 색
    public enum Colors
    {
        Red,
        Blue,
        White
    }
    public int monsterMaxHp;
    public string[] monsterName;
    public Colors color;
    public List<GameObject> prefab;
    

    public int Randoms(int a, int b)
    {
        int ab = Random.Range(a, b);
        return ab;
    }
}
