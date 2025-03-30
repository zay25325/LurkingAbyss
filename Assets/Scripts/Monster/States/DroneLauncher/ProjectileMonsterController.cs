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
    [SerializeField] private float attackChargeTime = 2f;  
    [SerializeField] private float fleeDistance = 5f;     
    [SerializeField] private float stalkingDistance = 10f;
    [SerializeField] private float maxChaseDistance = 20f; 
    [SerializeField] private float attackSpeed = 1.5f;

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

    protected override void OnHarmed(float damage)
    {
        base.OnHarmed(damage);

        if (HP > 0)
        {
            Debug.Log("Projectile Monster is fleeing after taking damage!");
            SwitchState<ProjectileMonsterFleeState>();
        }
    }
}
