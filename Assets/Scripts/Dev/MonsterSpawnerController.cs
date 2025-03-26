using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MonsterSpawnerController : MonoBehaviour
{
    [SerializeField] List<GameObject> SpawnThese = new() {};
    [SerializeField] LevelController levelController;

    

}

#if UNITY_EDITOR
[CustomEditor(typeof(MonsterSpawnerController))]
public class MonsterSpawnerControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MonsterSpawnerController myScript = (MonsterSpawnerController)target;
        if(GUILayout.Button("Spawn Things"))
        {
            //myScript.SpawnList();
        }
    }
}
#endif
