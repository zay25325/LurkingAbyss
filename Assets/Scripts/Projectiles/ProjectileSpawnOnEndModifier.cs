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
    public override void OnEndOfFlight()
    {
        GameObject spawnedObj = GameObject.Instantiate(prefab);
        spawnedObj.transform.position = transform.position;
    }
}
