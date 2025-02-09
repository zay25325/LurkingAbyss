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

    [SerializeField] List<RoomVariantData> roomVariants;

    public static readonly int STATIC_ROOM_SIZE = 9;

    private void Start() {
        foreach(var variant in roomVariants) {
            variant.Init();
        }
    }

    public void BuildLevelFromMap() {

        foreach(var variant in roomVariants) {
            variant.Init();
        }

        // Place Level Bounds
        var boundsTile = tileManager.PickTile(TileMapLayer.LayerClass.Palette,new Vector2Int(2,0));
        foreach(var pair in levelMap.RoomGrid) {
            RoomController room = pair.Value;
            Vector2Int roompos = (Vector2Int)tileManager.grid.WorldToCell(room.transform.position);
            Vector2Int roomsize = new(STATIC_ROOM_SIZE, STATIC_ROOM_SIZE);

            tileManager.SetRect(TileMapLayer.LayerClass.Unbreakable, roompos-roomsize, roompos+roomsize, boundsTile);
        }        

        foreach(var pair in levelMap.RoomGrid) {
            RoomController room = pair.Value;
            Vector2Int roompos = (Vector2Int)tileManager.grid.WorldToCell(room.transform.position);
            Vector2Int roomsize = new(STATIC_ROOM_SIZE, STATIC_ROOM_SIZE);

            // clone base room
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

            // Get a random room variant
            var roomVariant = this.roomVariants[Random.Range(0,roomVariants.Count)];

            // place internal walls
            var wallTile = tileManager.PickTile(TileMapLayer.LayerClass.Palette, new Vector2Int(1,0));
            foreach(var pos in roomVariant.walls) {
                tileManager.SetTile(TileMapLayer.LayerClass.Wall, roompos+pos, wallTile);
            }
            // place item spawns relative to room origin
            foreach(var obj in roomVariant.itemSpawns) {
                Instantiate(obj, room.transform.position+obj.transform.localPosition, Quaternion.identity);
            }
            // TODO place interactible spawns relative to room origin
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


