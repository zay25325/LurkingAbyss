using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchingBagState : MonsterState
{
    public override void OnHarmed(float damage)
    {
        Debug.Log($"{gameObject.name} was damaged for: {damage}");
    }
}
