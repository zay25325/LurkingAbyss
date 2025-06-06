using NavMeshPlus.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterNavController : MonoBehaviour
{
    [SerializeField] public NavMeshSurface monsterNav;

    private void Start() {
        monsterNav.BuildNavMesh();
    }

    public AsyncOperation UpdateMesh() {
        return monsterNav.UpdateNavMesh(monsterNav.navMeshData);
    }
}
