using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HiveMotherController : MonsterController
{
    [Header("Scouting")]
    [SerializeField] float callDistance = 25f; // should always be louder than navigationDistance
    [SerializeField] float navigationPointDistance = 20f;

    private int collectedSwarmlings = 0;

    public const int RequiredSwarmlingsForCombat = 4;

    public float CallDistance { get => callDistance; }
    public float NavigationPointDistance { get => navigationPointDistance; }
    public int CollectedSwarmlings { get => collectedSwarmlings; set => collectedSwarmlings = value; }
}
