using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSwitchMonsterState : MonsterState
{
    float timeBeforeSwitchingBack = 0;
    const float SWITCH_DELAY = 5f;

    void Update()
    {
        // update timer
        if (timeBeforeSwitchingBack > 0)
        {
            timeBeforeSwitchingBack -= Time.deltaTime;
            if (timeBeforeSwitchingBack <= 0)
            {
                timeBeforeSwitchingBack = 0;
                controller.SwitchState<DebugMonsterState>();
            }
        }
    }

    private void OnEnable()
    {
        Debug.Log("DebugSwitchMonsterState enabled");
        timeBeforeSwitchingBack = SWITCH_DELAY;
    }

    private void OnDisable()
    {
        Debug.Log("DebugSwitchMonsterState disabled");
    }
}
