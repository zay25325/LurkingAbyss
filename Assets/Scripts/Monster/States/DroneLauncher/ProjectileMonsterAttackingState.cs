// FileName:     ProjectileMonsterAttackingState.cs
// Assignment:   Capstone Project
// Author:       Rhys McCash
// Student #:    8825169
// Date:         03/01/2025
// Description:  Charges and fires a homing projectile at the target with a cooldown.

using System.Collections;
using UnityEngine;

public class ProjectileMonsterAttackingState : MonsterState
{
    new protected ProjectileMonsterController controller => base.controller as ProjectileMonsterController;
    [SerializeField] GameObject projectilePrefab;
    
    private bool canAttack = false;
    private float lastAttackTime = 0f;

    private void OnEnable()
    {
        StartCoroutine(ChargeAttack());
    }

    private IEnumerator ChargeAttack()
    {
        yield return new WaitForSeconds(controller.AttackChargeTime);
        canAttack = true;
    }

    private void Update()
    {
        if (controller.Target == null || !controller.Target.CompareTag("Player"))
        {
            controller.Target = null;
            controller.SwitchState<ProjectileMonsterMoveState>();
            return;
        }

        float distanceToTarget = Vector3.Distance(controller.transform.position, controller.Target.transform.position);

        if (distanceToTarget > controller.StalkingDistance * 1.5f)
        {
            controller.SwitchState<ProjectileMonsterMoveState>();
            return;
        }

        if (distanceToTarget < controller.FleeDistance)
        {
            controller.SwitchState<ProjectileMonsterFleeState>();
            return;
        }

        if (canAttack && Time.time >= lastAttackTime + controller.AttackSpeed)
        {
            FireProjectile();
            lastAttackTime = Time.time; 
            canAttack = false;
            StartCoroutine(ChargeAttack()); 
        }
    }

    private void FireProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, controller.transform.position, Quaternion.identity);
        ProjectileController projectileScript = projectile.GetComponent<ProjectileController>();
        projectileScript.Target = controller.Target.transform.position;

        Debug.Log("Firing tracking projectile at " + controller.Target.name);
    }
}
