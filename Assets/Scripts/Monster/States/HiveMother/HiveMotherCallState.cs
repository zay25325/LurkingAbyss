using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiveMotherCallState : MonsterState
{
    new private HiveMotherController controller { get => base.controller as HiveMotherController; }

    const float waitDuration = 2f; // if the swarmling runs into a way, don't immediately switch to the return state
    float waitTimer = 0;

    private void Update()
    {
        waitTimer += Time.deltaTime;
        if (waitTimer > waitDuration)
        {
            controller.SwitchState<HiveMotherMoveState>();
        }
    }

    private void OnEnable()
    {
        waitTimer = 0f;
        NoiseDetectionManager.Instance.NoiseEvent.Invoke(transform.position, controller.CallDistance, GetComponent<EntityInfo>().Tags);
    }

    public override void OnNoiseDetection(Vector2 pos, float volume, List<EntityInfo.EntityTags> tags)
    {
        if (tags.Contains(EntityInfo.EntityTags.Swarmling))
        {
            controller.Agent.SetDestination(pos);
            controller.SwitchState<HiveMotherCollectState>();
        }
    }
}
