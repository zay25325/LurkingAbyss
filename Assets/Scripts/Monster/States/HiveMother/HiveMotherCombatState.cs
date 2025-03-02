/*
File: ProjectileController.cs
Project: Capstone Project
Programmer: Isaiah Bartlett
First Version: 2/28/2025
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HiveMotherCombatState : HiveMotherBaseState
{
    [SerializeField] GameObject SwarmlingProjectile;

    [SerializeField] float fireRateDelay = 1.25f;
    [SerializeField] float fireRateReductionPerSwarmling = .125f;
    float fireRateTimer = 0f;

    [SerializeField] float positionUpdateDelay = .5f;
    float positionUpdateTimer = 0f;
    List<ProjectileController> projectiles = new List<ProjectileController>();


    [SerializeField] float combatDistance = 3f; // the distance which the hive mother will attempt to maintain from the target

    new protected void OnEnable()
    {
        //DONT call base
        controller.OverrideSightDirection = true;
        UpdatePosition(controller.CombatTarget.transform.position);
    }

    private void Update()
    {
        // projectile attack
        if (projectiles.Count < controller.CollectedSwarmlings)
        {
            fireRateTimer += Time.deltaTime;
            if (fireRateTimer > fireRateDelay - fireRateReductionPerSwarmling * controller.CollectedSwarmlings)
            {
                fireRateTimer -= fireRateDelay;
                FireProjectile(controller.CombatTarget.transform.position);
            }
        }

        Vector3 direction = (controller.CombatTarget.transform.position - transform.position).normalized;

        // vision
        controller.SightController.LookDirection = SimpleSightMeshController.GetAngleFromVectorFloat(direction) + 90;

        // navigation
        positionUpdateTimer += Time.deltaTime;
        if (positionUpdateTimer > positionUpdateDelay)
        {
            positionUpdateTimer = 0f;
            UpdatePosition(controller.CombatTarget.transform.position);
        }
    }

    public override void OnSeeingEntityExit(Collider2D collider)
    {
        if (collider.gameObject == controller.CombatTarget)
        {
            controller.CombatTarget = null;
            controller.SwitchState<HiveMotherMoveState>();
        }
    }


    private void FireProjectile(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        GameObject projectileObj = Instantiate(SwarmlingProjectile, transform.position + direction, new Quaternion());
        ProjectileController projectile = projectileObj.GetComponent<ProjectileController>();
        projectile.Target = target;
        ProjectileEventModifier eventMod = projectile.GetComponent<ProjectileEventModifier>();

        projectiles.Add(projectile);
        eventMod.EndOfFlightEvent.AddListener(() =>
        {
            projectiles.Remove(projectile);
        });
    }

    private void UpdatePosition(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        Vector3 rawNavPoint = target - direction * combatDistance;
        if (NavMesh.SamplePosition(rawNavPoint, out NavMeshHit hit, 5, 1))
        {
            controller.Agent.destination = hit.position;
        }
    }
}
