
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
    [SerializeField] float maxChaseDistance = 20f; 
    [SerializeField] float attackSpeed = 1.5f;


    public float AttackSpeed => attackSpeed;
    public float MaxChaseDistance => maxChaseDistance;
    public float AttackChargeTime => attackChargeTime;
    public float FleeDistance => fleeDistance;
    public float StalkingDistance => stalkingDistance;
    
    public GameObject Target { get; set; }

    new protected void Update()
    {
        base.Update();

        if (state.GetType() != typeof(ProjectileMonsterMoveState) && Target == null)
        {
            SwitchState<ProjectileMonsterMoveState>();
        }
    }
}
