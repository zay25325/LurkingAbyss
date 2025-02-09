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

    public static readonly int STATIC_ROOM_SIZE = 9;

    public void BuildLevelFromMap() {

        // Place Level Bounds
        BoundsInt levelBounds = new(0,0,0,0,0,1);
        int xMin = 0, xMax = 0, yMin = 0, yMax = 0;
        foreach(var pair in levelMap.RoomGrid) {
            RoomController room = pair.Value;
            Vector2Int roompos = (Vector2Int)tileManager.grid.WorldToCell(room.transform.position);

            // stretch level bounds to fit every generated room
            xMin = roompos.x < xMin ? roompos.x : xMin;
            xMax = roompos.x > xMax ? roompos.x : xMax;

            yMin = roompos.y < yMin ? roompos.y : yMin;
            yMax = roompos.y > yMax ? roompos.y : yMax;
        }

        levelBounds.SetMinMax(new Vector3Int(xMin, yMin, 0), new Vector3Int(xMax, yMax, 1));

        var boundsTile = tileManager.PickTile(TileMapLayer.LayerClass.Palette,new Vector2Int(2,0));
        tileManager.SetRect(TileMapLayer.LayerClass.Unbreakable, new Vector2Int(levelBounds.xMin,levelBounds.yMin),new Vector2Int(levelBounds.xMax,levelBounds.yMax), boundsTile);

        foreach(var pair in levelMap.RoomGrid) {
            RoomController room = pair.Value;
            Vector2Int roompos = (Vector2Int)tileManager.grid.WorldToCell(room.transform.position);
            Vector2Int roomsize = new(STATIC_ROOM_SIZE, STATIC_ROOM_SIZE);

            // clone room prefab
            tileManager.CloneRect(
                TileMapLayer.LayerClass.Roomx9,
                new Vector2Int(-4,-4),
                new Vector2Int(STATIC_ROOM_SIZE, STATIC_ROOM_SIZE),
                TileMapLayer.LayerClass.Wall,
                roompos-(roomsize/2)
            );

            //carve out level bounds 
            tileManager.ClearRect(
                TileMapLayer.LayerClass.Unbreakable,
                roompos-(roomsize/2),
                new Vector2Int(STATIC_ROOM_SIZE, STATIC_ROOM_SIZE)
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
                        //var doorpos = Random.Range(-(halfwidth-1),halfwidth-1); //this just uses room width. we don't plan on having oblong rooms do we?
                        var doorpos = 0;
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


