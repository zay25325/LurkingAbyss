/*
File: TrapperHuntingState.cs
Project: Capstone Project
Programmer: Isaiah Bartlett
First Version: 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TrapperHuntingState : TrapperBaseState
{
    [Header("Interest")] // return to wander if losing the target
    [SerializeField] float interestDuration = 3f; // 3 seconds
    float interestTimer;


    [SerializeField] GameObject SwarmlingProjectile;

    [SerializeField] float fireRateDelay = 1.25f;
    float fireRateTimer = 0f;

    [SerializeField] float positionUpdateDelay = .5f;
    float positionUpdateTimer = 0f;
    List<ProjectileController> projectiles = new List<ProjectileController>();


    [SerializeField] float combatDistance = 3f; // the distance which the hive mother will attempt to maintain from the target


    private void OnEnable()
    {
        interestTimer = interestDuration;
    }

    protected void Update()
    {
        if (controller.Targets.Count == 0)
        {
            interestTimer -= Time.deltaTime;
            if (interestTimer <= 0)
            {
                controller.SwitchState<TrapperWanderState>();
            }
        }
        else
        {
            interestTimer = interestDuration;

            // projectile attack
            fireRateTimer += Time.deltaTime;
            if (fireRateTimer > fireRateDelay)
            {
                fireRateTimer -= fireRateDelay;
                FireProjectile(controller.Targets[0].transform.position);
            }

            Vector3 direction = (controller.Targets[0].transform.position - transform.position).normalized;

            // vision
            controller.SightController.LookDirection = SimpleSightMeshController.GetAngleFromVectorFloat(direction) + 90;

            // navigation
            positionUpdateTimer += Time.deltaTime;
            if (positionUpdateTimer > positionUpdateDelay)
            {
                positionUpdateTimer = 0f;
                UpdatePosition(controller.Targets[0].transform.position);
            }
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
