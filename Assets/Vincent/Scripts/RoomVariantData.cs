using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomVariantData : MonoBehaviour
{
    // default room size of 9x9
    [SerializeField] Vector2Int roomSize = new(LevelController.STATIC_ROOM_SIZE,LevelController.STATIC_ROOM_SIZE);    
    
    // for item and interactible spawns
    [System.NonSerialized] public List<SpawnController> itemSpawns = new() {};
    [System.NonSerialized] public List<Vector2Int> walls = new() {};

    private bool didInit = false;

    [SerializeField] Tilemap wallsMap;

    private void Start() {
        Init();
    }

    public void Init() {
        if(!didInit) {
            //register our item spawns
            foreach(var i in GetComponentsInChildren<SpawnController>()) {
                if(i.type == SpawnController.SpawnClass.Item) {
                    itemSpawns.Add(i);
                }
            }

            // get list of wall positions
            foreach(var pos in new BoundsInt(-roomSize.x/2,-roomSize.y/2,0,roomSize.x,roomSize.y,1).allPositionsWithin) {
                var t = wallsMap.GetTile(new Vector3Int(pos.x,pos.y,0));
                if(t != null) {
                    walls.Add((Vector2Int)pos);
                }
            }
            didInit = true;
        }
    }

    public TileBase[] getWalls() {
        BoundsInt area = new BoundsInt(-roomSize.x/2,-roomSize.y/2,roomSize.x,roomSize.y,0,1);
        return wallsMap.GetTilesBlock(area);
    }

    private void OnDrawGizmos() {
        Gizmos.color = new(0f,1f,1f,0.1f);
        Gizmos.DrawCube(new Vector3(0.5f,0.5f,0f), new Vector3(roomSize.x,roomSize.y,1));
    }

}
