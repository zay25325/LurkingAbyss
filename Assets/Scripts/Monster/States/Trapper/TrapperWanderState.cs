/*
File: TrapperWanderState.cs
Project: Capstone Project
Programmer: Isaiah Bartlett
First Version: 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static EntityInfo;

public class TrapperWanderState : TrapperBaseState
{
    [SerializeField] float navigationPointDistance = 20f;

    List<Vector3> navigationPoints = new List<Vector3>();
    Vector3? currentNavPoint = null;

    protected void OnEnable()
    {
        SetNextPoint();
    }

    private void Update()
    {
        if (controller.Targets.Count > 0)
        {
            controller.SwitchState<TrapperHuntingState>();
        }
        if (controller.Agent.remainingDistance < .5f)
        {
            navigationPoints.Remove(currentNavPoint.Value); 
            SetNextPoint();
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

    private void SetNextPoint()
    {
        if (navigationPoints.Count == 0)
        {
            navigationPoints = MonsterController.GenerateNavigationPoints(navigationPointDistance);
        }

        currentNavPoint = navigationPoints[Random.Range(0, navigationPoints.Count)];
        controller.Agent.SetDestination(currentNavPoint.Value);
    }

    public override void OnTrapTriggered(TrapperBulbController bulb, Vector3 targetPos)
    {
        controller.TeleportToBulb(bulb, targetPos);
    }
}
