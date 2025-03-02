using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiveMotherCallState : HiveMotherBaseState
{
    [SerializeField] float sightScanSpeed = 180f;
    [SerializeField] float waitDuration = 2f; // if the swarmling runs into a way, don't immediately switch to the return state
    float waitTimer = 0;

    private void Update()
    {
        if (controller.Agent.remainingDistance < .5f)
        {
            if (controller.Agent.destination != transform.position)
            {
                controller.OverrideSightDirection = true;
                controller.Agent.destination = transform.position;
            }

            controller.SightController.LookDirection += sightScanSpeed * Time.deltaTime;
        }

        waitTimer += Time.deltaTime;
        if (waitTimer > waitDuration)
        {
            controller.SwitchState<HiveMotherMoveState>();
        }
    }

    new protected void OnEnable()
    {
        base.OnEnable();

        // Use destination from previous state
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
