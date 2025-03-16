/*
File: TrapperRestoreState.cs
Project: Capstone Project
Programmer: Isaiah Bartlett
First Version: 3/15/2025
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static EntityInfo;

public class TrapperRestoreState : TrapperBaseState
{
    [SerializeField] float restoreDelay = 1.5f;
    float restoreTimer;

    protected void OnEnable()
    {
        restoreTimer = restoreDelay;
        if (controller.DestroyedBulbs.Count > 0)
        {
            controller.Agent.destination = controller.DestroyedBulbs[0].transform.position;
        }
        else
        {
            controller.SwitchState<TrapperWanderState>(); // just in case
        }
    }

    private void Update()
    {
        if (controller.Targets.Count > 0) // prioritize combat over restoring traps
        {
            controller.SwitchState<TrapperHuntingState>();
        }
        if (controller.Agent.remainingDistance < .5f) // restore trap
        {
            restoreTimer -= Time.deltaTime;
            if (restoreTimer <= 0f)
            {
                controller.DestroyedBulbs[0].TriggerTrapRestored();
                controller.SwitchState<TrapperWanderState>();
            }
        }
    }

    public override void OnSeeingEntityEnter(Collider2D collider)
    {
        base.OnSeeingEntityEnter(collider);
        if (controller.Targets.Count > 0)
        {
            controller.SwitchState<TrapperHuntingState>();
        }
    }

    public override void OnTrapTriggered(TrapperBulbController bulb, Vector3 targetPos)
    {
        controller.TeleportToBulb(bulb, targetPos);
    }
}
