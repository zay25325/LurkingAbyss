/*
File: ProjectileController.cs
Project: Capstone Project
Programmer: Isaiah Bartlett
First Version: 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HiveMotherController : MonsterController
{
    [Header("Scouting")]
    [SerializeField] float callDistance = 25f; // should always be louder than navigationDistance
    [SerializeField] float navigationPointDistance = 20f;

    [Header("Combat")]
    [SerializeField] public int RequiredSwarmlingsForCombat = 4;
    [SerializeField] int collectedSwarmlings = 0;
    [SerializeField] float passiveHealingPerSwarmling = 0.125f; // with 4 swarmlings, heal at 0.5hp/sec

    

    public float CallDistance { get => callDistance; }
    public float NavigationPointDistance { get => navigationPointDistance; }
    public int CollectedSwarmlings { get => collectedSwarmlings; set => collectedSwarmlings = value; }
    public GameObject CombatTarget { get; set; }

    new protected void Update()
    {
        base.Update();
        hp = Mathf.Clamp(hp + collectedSwarmlings * passiveHealingPerSwarmling * Time.deltaTime, 0, maxHP);
    }
}
