using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class LevelController : MonoBehaviour
{
    [SerializeField] TileController tileManager;
    [SerializeField] MapController levelMap;

    [SerializeField] int mapGenRoomCount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuildLevelFromMap() {
        foreach(var pair in levelMap.RoomGrid) {
            RoomController room = pair.Value;
            Vector2Int roompos = (Vector2Int)tileManager.grid.WorldToCell(room.transform.position);
            Vector2Int roomsize = new(room.width, room.height);

            // clone room prefab
            tileManager.CloneRect(
                "Prefabs",
                new Vector2Int(2,-7),
                new Vector2Int(room.width, room.height),
                "Walls",
                roompos-(roomsize/2));
        }
    }

    public void GenerateLevel() {
        levelMap.StartRoomGen(mapGenRoomCount);
    }
}


[CustomEditor(typeof(LevelController))]
public class LevelControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelController myScript = (LevelController)target;
        if(GUILayout.Button("Start Level Gen"))
        {
            myScript.GenerateLevel();
        }

        if(GUILayout.Button("Place Tiles"))
        {
            myScript.BuildLevelFromMap();
        }
    }
}


