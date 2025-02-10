using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoObserveState : MonsterState
{
    public override void OnNoiseDetection(Vector2 pos, float volume, List<EntityInfo.EntityTags> tags)
    {
        controller.Agent.destination = pos;
    }
}
