using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSwitchMonsterState : MonsterState
{
    float timeBeforeSwitchingBack = 0;
    const float SWITCH_DELAY = 5f;

    void Start()
    {
        // nothing to do on Start
    }

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

    public override void OnNoiseDetection(Vector2 pos, float volume)
    {

    }

    public override void OnSeeingEntityEnter(Collider2D collider)
    {

    }

    public override void OnSeeingEntityExit(Collider2D collider)
    {

    }

    public override void OnTouchEnter(Collision2D collision)
    {

    }

    public override void OnTouchExit(Collision2D collision)
    {

    }
}
