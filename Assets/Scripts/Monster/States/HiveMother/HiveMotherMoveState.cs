/*
File: ProjectileController.cs
Project: Capstone Project
Programmer: Isaiah Bartlett
First Version: 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HiveMotherMoveState : HiveMotherBaseState
{
    NavMeshPlus.Components.NavMeshSurface navSurface;
    List<Vector3> navigationPoints = new List<Vector3>();
    Vector3? currentNavPoint = null;

    private void Awake()
    {
        navSurface = GameObject.FindFirstObjectByType<NavMeshPlus.Components.NavMeshSurface>();
    }

    new protected void OnEnable()
    {
        base.OnEnable();
        if (currentNavPoint.HasValue == false)
        {
            if (navigationPoints.Count == 0)
            {
                GenerateNavigationPoints();
            }

            currentNavPoint = navigationPoints[Random.Range(0, navigationPoints.Count)];
        }

        controller.Agent.SetDestination(currentNavPoint.Value);
    }

    private void Update()
    {
        if (controller.Agent.remainingDistance < .5f)
        {
            navigationPoints.Remove(currentNavPoint.Value);
            currentNavPoint = null;
            controller.SwitchState<HiveMotherCallState>();
        }
    }

    public override void OnSeeingEntityEnter(Collider2D collider)
    {
        EntityInfo info = collider.GetComponent<EntityInfo>();
        if (info != null)
        {
            if ((info.Tags.Contains(EntityInfo.EntityTags.Hunter) || info.Tags.Contains(EntityInfo.EntityTags.Territorial))
            && controller.CollectedSwarmlings >= controller.RequiredSwarmlingsForCombat)
            {
                controller.CombatTarget = info.gameObject;
                controller.SwitchState<HiveMotherCombatState>();
            }
            else if (info.Tags.Contains(EntityInfo.EntityTags.Swarmling))
            {
                controller.SwitchState<HiveMotherCallState>();
            }
        }
    }


    private void GenerateNavigationPoints()
    {
        // Make points spread out evenly across the entire navmesh 
        Bounds bounds = navSurface.navMeshData.sourceBounds;
        int xPoints = Mathf.CeilToInt(bounds.size.x / controller.NavigationPointDistance);
        int yPoints = Mathf.CeilToInt(bounds.size.z / controller.NavigationPointDistance); // navmesh needs to be rotated, so this needs to be z, not y

        float xDifference = bounds.size.x / (xPoints + 1);
        float yDifference = bounds.size.z / (yPoints + 1);

        for (int x = 0; x <= xPoints; x++)
        {
            for (int y = 0; y <= yPoints; y++)
            {
                Vector3 rawNavPoint = new Vector3(bounds.min.x, bounds.min.z, 0) + new Vector3(xDifference / 2 + xDifference * x, yDifference / 2 + yDifference * y, 0);
                if (NavMesh.SamplePosition(rawNavPoint, out NavMeshHit hit, controller.NavigationPointDistance, 1))
                {
                    navigationPoints.Add(hit.position);
                }
            }
        }
    }
}
