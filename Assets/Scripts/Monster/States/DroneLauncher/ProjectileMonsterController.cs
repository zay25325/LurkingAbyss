
// FileName:     ProjectileMonsterController.cs
// Assignment:   Capstone Project
// Author:       Rhys McCash
// Student #:    8825169
// Date:         03/01/2025
// Description:  Controls the ProjectileMonster, handling state transitions

using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ProjectileMonsterController : MonsterController
{
    [Header("Combat")]
    [SerializeField] float attackChargeTime = 2f;  
    [SerializeField] float fleeDistance = 5f;     
    [SerializeField] float stalkingDistance = 10f;

    public float AttackChargeTime => attackChargeTime;
    public float FleeDistance => fleeDistance;
    public float StalkingDistance => stalkingDistance;
    
    public GameObject Target { get; set; }

    new protected void Update()
    {
        base.Update();

        if (Target == null)
        {
            SwitchState<ProjectileMonsterMoveState>();
        }
    }
}
