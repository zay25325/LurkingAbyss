using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmlingFleeState : MonsterState
{
    new private SwarmlingController controller { get => base.controller as SwarmlingController; } // lets hope this dosen't kill everything

    const float hideDuration = 3f; // if the swarmling runs into a way, don't immediately switch to the return state
    float hideTimer = 0;

    private void Update()
    {
        if (controller.Agent.remainingDistance < 0.5f)
        {
            hideTimer += Time.deltaTime;
            if (hideTimer > hideDuration)
            {
                controller.SwitchState<SwarmlingReturnState>();
            }
        }
    }

    private void OnEnable()
    {
        controller.Agent.speed = controller.BaseSpeed;
        hideTimer = 0f;
    }

    public override void OnNoiseDetection(Vector2 pos, float volume)
    {
        controller.FleeFromSound(pos, false); // don't reinvoke this state
        hideTimer = 0f;
    }

}
