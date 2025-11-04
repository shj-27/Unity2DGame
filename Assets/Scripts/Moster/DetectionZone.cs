using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionZone : MonoBehaviour
{
    public List<Monsters> monsterInRange = new List<Monsters>();

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent<Monsters>(out var m))
        {
            if (!monsterInRange.Contains(m))
                monsterInRange.Add(m);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.TryGetComponent<Monsters>(out var m))
            monsterInRange.Remove(m);
    }
}
