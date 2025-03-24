using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEventRelay : MonoBehaviour
{
    [SerializeField] MonsterController controller;
    public void OnDeath()
    {
        controller.OnDeath();
    }
}
