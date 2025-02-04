using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

// TODO:
// add logic for player start and exit
// add logic for generating items
// add logic for placing monster spawns
// add logic for placing interactibles

public class LevelController : MonoBehaviour
{
    [SerializeField] TileController tileManager;
    [SerializeField] MapController levelMap;

    [SerializeField] int mapGenRoomCount;

    const int STATIC_ROOM_SIZE = 9;

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
                TileMapLayer.LayerClass.Palette,
                new Vector2Int(-4,-4),
                new Vector2Int(STATIC_ROOM_SIZE, STATIC_ROOM_SIZE),
                TileMapLayer.LayerClass.Wall,
                roompos-(roomsize/2)
            );

            // place doors
            for(int i = 0; i < room.connections.Length; i++) {
                
                var dir = Directions.IntToVec(i);
                var perp = Vector2.Perpendicular(dir);
                var halfwidth = Mathf.Floor(STATIC_ROOM_SIZE/2);

                switch(room.GetConnectionByIndex(i)) {
                    case 0: // closed
                        break;
                    case 1: // door
                        //selects a random spot for the opening to appear
                        var doorpos = Random.Range(-(halfwidth-1),halfwidth-1); //this just uses room width. we don't plan on having oblong rooms do we?

                        // this is a really confusing string of math, but basically:
                        // roompos points to the center of the room
                        // then pos points to a wall
                        // then perp points to a spot on that wall
                        tileManager.ClearTile(TileMapLayer.LayerClass.Wall, (Vector2Int)tileManager.grid.WorldToCell(roompos+dir*(halfwidth)+perp*doorpos));

                        break;
                    case 2: // open
                        break;
                        
                }
            }

            // place floor
            //tileManager.CloneRect(
            //    "Prefabs",
            //    new Vector2Int(9,-7),
            //    new Vector2Int(room.width, room.height),
            //    "Floor",
            //    roompos-(roomsize/2)
            //);
        }
    }

    public void GenerateLevel() {
        levelMap.StartRoomGen(mapGenRoomCount);
    }

    public void DestroyLevel() {
        levelMap.ClearRoomGrid();
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

        if(GUILayout.Button("Destroy Map"))
        {
            myScript.DestroyLevel();
        }
    }
}


