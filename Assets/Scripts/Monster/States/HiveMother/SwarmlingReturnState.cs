using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SwarmlingReturnState : MonsterState
{
    new private SwarmlingController controller { get => base.controller as SwarmlingController; } // lets hope this dosen't kill everything

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

    public override void OnNoiseDetection(Vector2 pos, float volume)
    {
        controller.FleeFromSound(pos);
    }
}
