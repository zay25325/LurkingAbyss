using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomVariantData : MonoBehaviour
{
    // default room size of 9x9
    [SerializeField] Vector2Int roomSize = new(LevelController.STATIC_ROOM_SIZE,LevelController.STATIC_ROOM_SIZE);    
    
    // for item and interactible spawns
    [SerializeField] List<GameObject> pointsOfInterest = new() {};

    [SerializeField] Tilemap walls;

    public TileBase[] getWalls() {
        BoundsInt area = new BoundsInt(0,0,roomSize.x,roomSize.y,0,1);
        return walls.GetTilesBlock(area);
    }


}
