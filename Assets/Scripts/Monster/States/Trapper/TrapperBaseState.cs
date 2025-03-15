/*
File: TrapperBaseState.cs
Project: Capstone Project
Programmer: Isaiah Bartlett
First Version: 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EntityInfo;

public class TrapperBaseState : MonsterState
{
    new protected TrapperController controller { get => base.controller as TrapperController; }
    public virtual void OnTrapTriggered(TrapperBulbController bulb, Vector3 targetPos) { }
    public virtual void OnTrapDestroyed(TrapperBulbController bulb) { }

    public override void OnSeeingEntityEnter(Collider2D collider)
    {
        EntityInfo info = collider.GetComponent<EntityInfo>();
        if (info != null)
        {
            if (info.Tags.Contains(EntityTags.Creature) && info.Tags.Contains(EntityTags.TrapperBulb) == false)
            {
                if (controller.Targets.Contains(info) == false) // just in case
                {
                    controller.Targets.Add(info);
                }
            }
        }
    }
    public override void OnSeeingEntityExit(Collider2D collider)
    {
        EntityInfo info = collider.GetComponent<EntityInfo>();
        if (info != null && controller.Targets.Contains(info))
        {
            controller.Targets.Remove(info);
        }
    }
}
