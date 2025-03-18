/*
File: ProjectileController.cs
Project: Capstone Project
Programmer: Isaiah Bartlett
First Version: 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawnOnEndModifier : ProjectileModifier
{
    [SerializeField] GameObject prefab;
    private EntityInfo attackingEntityInfo;

    public void SetAttackingEntityInfo(EntityInfo entityInfo)
    {
        attackingEntityInfo = entityInfo;
    }
    public override void OnEndOfFlight()
    {
        GameObject spawnedObj = GameObject.Instantiate(prefab);
        spawnedObj.transform.position = transform.position;

        if (attackingEntityInfo != null)
        {
            ProjectileDamageModifier damageModifier = spawnedObj.GetComponent<ProjectileDamageModifier>();
            if (damageModifier != null)
            {
                damageModifier.attackingEntityInfo = attackingEntityInfo;
            }
            else
            {
                Debug.LogError("Spawned object is missing a ProjectileDamageModifier!");
            }
        }
    }
}
