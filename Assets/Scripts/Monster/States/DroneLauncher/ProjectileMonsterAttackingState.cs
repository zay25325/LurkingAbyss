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
        if (controller.Target == null) return;

        Vector3 directionToTarget = (controller.Target.transform.position - controller.transform.position).normalized;
        Vector3 spawnPosition = controller.transform.position + (directionToTarget * 1.5f);
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

        int projectileLayer = LayerMask.NameToLayer("Projectiles");
        projectile.layer = projectileLayer;
        int monsterLayer = controller.gameObject.layer;
        Physics.IgnoreLayerCollision(projectileLayer, monsterLayer, true);

        ProjectileController projectileScript = projectile.GetComponent<ProjectileController>();
        if (projectileScript != null)
        {
            projectileScript.SetTarget(controller.Target.transform.position);
        }

        Debug.Log("Firing projectile at " + controller.Target.name);
    }

}
