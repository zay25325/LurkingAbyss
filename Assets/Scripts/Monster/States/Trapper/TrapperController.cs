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
    private float maxDistanceFromPrevious = 5f; // ignore matching sounds if they are not in the correct location. when tracking one swarmling, ignore other swarmlings
    private Vector2 lastTargetLocation = Vector2.zero;
    private List<EntityTags> targetTags = new List<EntityTags>();

    public float MaxDistanceFromPrevious { get => maxDistanceFromPrevious; }
    public Vector2 LastTargetLocation { get => lastTargetLocation; set => lastTargetLocation = value; }
    public List<EntityTags> TargetTags { get => targetTags; set => targetTags = value; }
}
