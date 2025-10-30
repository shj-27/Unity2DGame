using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MosterData", menuName = "SO/MonsterData")]
public class MonsterData : ScriptableObject
{
    public enum Colors
    {
        Red,
        Blue, 
        Green
    }

    public string monsterName;
    public string colors;
    public int maxHp;
    public float moveSpeed;
    public GameObject prefab;
    void Start()
    {
        
    }

   
    void Update()
    {
        
    }
}
