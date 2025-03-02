using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomVariantData : MonoBehaviour
{
    // default room size of 9x9
    [SerializeField] Vector2Int roomSize = new(LevelController.STATIC_ROOM_SIZE,LevelController.STATIC_ROOM_SIZE);    
    
    // for item and interactible spawns
    [HideInInspector] public List<SpawnController> SpawnPoints = new() {};
    [HideInInspector] public List<Vector2Int> walls = new() {};

    private bool isInitialized = false; // bools should be an is___ not a did___

    [SerializeField] Tilemap wallsMap;

    private void Awake() {
        Init();
    }

    public void Init() {
        if(!isInitialized) {
            //register all spawners
            //foreach(var i in GetComponentsInChildren<SpawnController>()) {
            //    //if(i.type == SpawnController.SpawnClass.Item) {
            //        itemSpawns.Add(i);
            //    //}
            //}
            SpawnPoints.AddRange(GetComponentsInChildren<SpawnController>());

            // get list of wall positions
            foreach (var pos in new BoundsInt(-roomSize.x/2,-roomSize.y/2,0,roomSize.x,roomSize.y,1).allPositionsWithin) {
                var t = wallsMap.GetTile(new Vector3Int(pos.x,pos.y,0));
                if(t != null) {
                    walls.Add((Vector2Int)pos);
                }
            }
            isInitialized = true;
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
