using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmlingRespondState : MonsterState
{
    new private SwarmlingController controller { get => base.controller as SwarmlingController; }

    private void OnEnable()
    {
        controller.Agent.speed = controller.BaseSpeed / 1.5f;
    }

    private void Update()
    {
        if (controller.Agent.remainingDistance < 0.5f)
        {
            controller.SwitchState<SwarmlingReturnState>();
        }
    }

    public override void OnNoiseDetection(Vector2 pos, float volume, List<EntityInfo.EntityTags> tags)
    {
        if (tags.Contains(EntityInfo.EntityTags.HiveMother))
        {
            NoiseDetectionManager.Instance.NoiseEvent.Invoke(transform.position, volume, GetComponent<EntityInfo>().Tags);
            controller.Agent.SetDestination(pos);
        }
    }
}
