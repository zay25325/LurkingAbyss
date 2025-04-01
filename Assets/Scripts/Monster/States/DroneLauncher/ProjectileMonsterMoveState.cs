// FileName:     ProjectileMonsterMoveState.cs
// Assignment:   Capstone Project
// Author:       Rhys McCash
// Student #:    8825169
// Date:         03/01/2025
// Description:  Handles the wandering behavior of the ProjectileMonster.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class ProjectileMonsterMoveState : MonsterState
{
    new protected ProjectileMonsterController controller { get => base.controller as ProjectileMonsterController; }
    private Vector3? currentNavPoint = null;
    private List<Vector3> navigationPoints = new List<Vector3>();

    new protected void OnEnable()
    {
        if (controller == null)
        {
            Debug.LogError("ProjectileMonsterMoveState: Controller is STILL null! Something went wrong.");
            return;
        }

        if (!currentNavPoint.HasValue)
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
        if (controller == null || controller.Agent == null)
        {
            Debug.LogError("ProjectileMonsterMoveState: Controller or NavMeshAgent is null!");
            return;
        }

        if (controller.Agent.remainingDistance < 0.5f)
        {
            navigationPoints.Remove(currentNavPoint.Value);
            if (navigationPoints.Count == 0)
                GenerateNavigationPoints();

            currentNavPoint = navigationPoints[Random.Range(0, navigationPoints.Count)];
            controller.Agent.SetDestination(currentNavPoint.Value);
        }

        foreach (GameObject obj in controller.ObjectsInView)
        {
            if (obj.CompareTag("Player"))
            {
                controller.Target = obj;
                controller.SwitchState<ProjectileMonsterStalkingState>();
                return;
            }
        }
    }

    private void GenerateNavigationPoints()
    {
        NavMeshPlus.Components.NavMeshSurface navMeshSurface = GameObject.FindFirstObjectByType<NavMeshPlus.Components.NavMeshSurface>();
        if (navMeshSurface == null)
        {
            Debug.LogError("No NavMeshSurface found in the scene.");
            return;
        }

        Bounds bounds = navMeshSurface.navMeshData.sourceBounds;
        int xPoints = Mathf.CeilToInt(bounds.size.x / controller.StalkingDistance);
        int yPoints = Mathf.CeilToInt(bounds.size.z / controller.StalkingDistance);
        float xStep = bounds.size.x / (xPoints + 1);
        float yStep = bounds.size.z / (yPoints + 1);

        for (int x = 0; x <= xPoints; x++)
        {
            for (int y = 0; y <= yPoints; y++)
            {
                Vector3 rawNavPoint = new Vector3(bounds.min.x + xStep * x, bounds.min.z + yStep * y, 0);
                if (NavMesh.SamplePosition(rawNavPoint, out NavMeshHit hit, controller.StalkingDistance, NavMesh.AllAreas))
                {
                    navigationPoints.Add(hit.position);
                }
            }
        }
    }
}
