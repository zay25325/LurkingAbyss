using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SwarmlingReturnState : MonsterState
{
    new private SwarmlingController controller { get => base.controller as SwarmlingController; }

    Vector3 returnPoint = Vector3.zero;


    void Awake()
    {
        returnPoint = transform.position;
    }

    private void OnEnable()
    {
        controller.Agent.speed = controller.BaseSpeed / 1.5f;
        controller.Agent.SetDestination(returnPoint);
    }

    private void Update()
    {
        if (controller.Agent.remainingDistance < 0.5f)
        {
            controller.SwitchState<SwarmlingWanderState>();
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
}
