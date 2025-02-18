using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MonsterSpawnerController : MonoBehaviour
{
    [SerializeField] List<GameObject> SpawnThese = new() {};
    [SerializeField] LevelController levelController;

    public void SpawnList() {
        //spawn the list of things in random spots in the level
        foreach(var thing in SpawnThese) {
            int spot = Random.Range(0,levelController.spawners.Count);
            Instantiate(thing, levelController.spawners[spot].transform);
        }
    }

}

[CustomEditor(typeof(MonsterSpawnerController))]
public class MonsterSpawnerControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MonsterSpawnerController myScript = (MonsterSpawnerController)target;
        if(GUILayout.Button("Spawn Things"))
        {
            myScript.SpawnList();
        }
    }
}
