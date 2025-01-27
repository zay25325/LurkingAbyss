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
        foreach(var room in levelMap.RoomGrid) {
            Vector2 roompos = room.Value.transform.position;

            // make the walls


        }
    }

    public void GenerateLevel() {
        levelMap.StartRoomGen(mapGenRoomCount);
    }
}

