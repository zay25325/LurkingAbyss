using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmlingWanderState : MonsterState
{
    new private SwarmlingController controller { get => base.controller as SwarmlingController; }

    private int pointsTraveledTo = 0;
    private int pointsBeforeReturning = 1;
    private float wanderDistance = 8f;

    private void OnEnable()
    {
        pointsBeforeReturning = Random.Range(2, 5); // 2 to 4 points
        controller.Agent.speed = controller.BaseSpeed;
        pointsTraveledTo = 0;
        SetRandomTravelPoint();
    }

    private void Update()
    {
        if (controller.Agent.remainingDistance < 0.5f)
        {
            pointsTraveledTo++;
            if (pointsTraveledTo >= pointsBeforeReturning)
            {
                controller.SwitchState<SwarmlingReturnState>();
            }
            else
            {
                SetRandomTravelPoint();
            }

        }
    }


    public override void OnNoiseDetection(Vector2 pos, float volume, List<EntityInfo.EntityTags> tags)
    {
        if (tags.Contains(EntityInfo.EntityTags.HiveMother))
        {
            NoiseDetectionManager.Instance.NoiseEvent.Invoke(transform.position, volume, GetComponent<EntityInfo>().Tags);
            controller.Agent.SetDestination(pos);
            controller.SwitchState<SwarmlingRespondState>();
        }
        else if (tags.Contains(EntityInfo.EntityTags.Swarmling) == false)
        {
            controller.FleeFromSound(pos);
        }
    }

    private void SetRandomTravelPoint()
    {
        Vector3 point = MonsterHelper.RandomNavmeshLocation(wanderDistance, transform.position);
        controller.Agent.destination = point;
    }
}
