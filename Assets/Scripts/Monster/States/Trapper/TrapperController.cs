/*
File: TrapperController.cs
Project: Capstone Project
Programmer: Isaiah Bartlett
First Version: 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static EntityInfo;

public class TrapperController : MonsterController
{
    public List<EntityInfo> Targets = new List<EntityInfo>();
    public List<TrapperBulbController> ActiveBulbs = new List<TrapperBulbController>();
    public List<TrapperBulbController> DestroyedBulbs = new List<TrapperBulbController>();

    private void Start()
    {
        GameObject[] bulbObjs = GameObject.FindGameObjectsWithTag("TrapperBulb");

        foreach(GameObject bulbObj in bulbObjs)
        {
            TrapperBulbController bulb = bulbObj.GetComponent<TrapperBulbController>();
            bulb.SetTrapper(this);
            ActiveBulbs.Add(bulb);
        }
    }

    public void TeleportToBulb(TrapperBulbController bulb, Vector3 targetPos)
    {
        bulb.TriggerTrapDestroyed();
        Agent.Warp(bulb.transform.position);
        Agent.destination = targetPos;
        animator.SetTrigger("Teleport");
        SightController.LookDirection = SimpleSightMeshController.GetAngleFromVectorFloat(bulb.transform.position - targetPos) - 90;
    }



    public void OnTrapTriggered(TrapperBulbController bulb, Vector3 TargetPos)
    {
        (state as TrapperBaseState).OnTrapTriggered(bulb, TargetPos);
    }

    public void OnTrapDestroyed(TrapperBulbController bulb)
    {
        ActiveBulbs.Remove(bulb);
        DestroyedBulbs.Add(bulb);

        (state as TrapperBaseState).OnTrapDestroyed(bulb);
    }

    public void OnTrapRestored(TrapperBulbController bulb)
    {
        DestroyedBulbs.Remove(bulb);
        ActiveBulbs.Add(bulb);
    }
}
