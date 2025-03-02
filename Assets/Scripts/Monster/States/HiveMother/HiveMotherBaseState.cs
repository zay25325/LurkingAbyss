/*
File: ProjectileController.cs
Project: Capstone Project
Programmer: Isaiah Bartlett
First Version: 2/28/2025
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiveMotherBaseState : MonsterState
{
    new protected HiveMotherController controller { get => base.controller as HiveMotherController; }

    protected void OnEnable()
    {
        controller.OverrideSightDirection = false;
    }

    public override void OnTouchEnter(Collision2D collision)
    {
        EntityInfo info = collision.collider.GetComponent<EntityInfo>();
        if (info != null && info.Tags.Contains(EntityInfo.EntityTags.Swarmling))
        {
            info.gameObject.SetActive(false);
            GameObject.Destroy(info.gameObject);
            controller.CollectedSwarmlings++;
            controller.SwitchState<HiveMotherCallState>();
        }
    }
}
