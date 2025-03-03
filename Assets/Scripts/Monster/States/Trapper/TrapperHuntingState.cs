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
    [Header("Traps")]
    [SerializeField] int maxTrapCount = 7;
    [SerializeField] float trapCooldown = 2.5f;
    [SerializeField] GameObject trapPrefab; // maybe make a list later
    private float trapTimer;

    [Header("Navigation")]
    [SerializeField] float followDistance = 6;
    [SerializeField] float maxSamplePosDistance = 3;

    [Header("Interest")] // return to wander if losing the target
    [SerializeField] float interestDuration = 5f; // 5 seconds
    float interestTimer;



    private void OnEnable()
    {
        //Require tags and last point being set from wander state
        trapTimer = trapCooldown;
        interestTimer = interestDuration;
    }

    protected void Update()
    {
        trapTimer -= Time.deltaTime;
        if (trapTimer <= 0)
        {
            Instantiate(trapPrefab, transform.position, new Quaternion());
            trapTimer = trapCooldown;
        }

        interestTimer -= Time.deltaTime;
        if (interestTimer <= 0)
        {
            controller.SwitchState<TrapperWanderState>();
        }
    }


    public override void OnNoiseDetection(Vector2 pos, float volume, List<EntityInfo.EntityTags> tags)
    {
        if (Vector2.Distance(pos, controller.LastTargetLocation) > controller.MaxDistanceFromPrevious)
        {
            Vector2 direction = (controller.LastTargetLocation - pos).normalized;
            Vector2 navPos = pos - (direction * followDistance); // follow the target from a distance;
            if (NavMesh.SamplePosition(navPos, out NavMeshHit hit, maxSamplePosDistance, 2))
            {
                controller.Agent.destination = hit.position;
            }

            controller.LastTargetLocation = pos;
            interestTimer = interestDuration;
        } 
    }
}
