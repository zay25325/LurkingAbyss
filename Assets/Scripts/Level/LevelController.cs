using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using Unity.AI.Navigation;
using UnityEngine.Events;
using Unity.VisualScripting;

// TODO:
// add logic for player start and exit
// add logic for generating items
// add logic for placing monster spawns
// add logic for placing interactibles

public class LevelController : MonoBehaviour
{
    public bool AutoCreate = true;

    //Tilemap
    [SerializeField] TileController tileManager;
    [SerializeField] MapController levelMap;
    public static readonly int STATIC_ROOM_SIZE = 13;

    //Room Layout
    [SerializeField] int mapGenRoomCount = 30;

    //Room Variants
    [DoNotSerialize] List<RoomVariantData> roomVariants = new() {};

    //NavMesh
    [SerializeField] public List<MonsterNavController> monsterNavs;
    [SerializeField] public static UnityEvent UpdateNavMesh = new UnityEvent();

    //Point of Interest
    [SerializeField] public List<SpawnController> spawners = new() {};

    //Doors
    [SerializeField] GameObject doorPrefab;

    //Player
    [SerializeField] GameObject playerPrefab;

    //SpawnPools
    [SerializeField] SpawnPoolManager spawnPoolManager;
    private List<GameObject> spawnListPrefabs = new List<GameObject>();
    [SerializeField] Transform spawnPointParent;

    public bool BreakTileAt(Vector3 worldpos) {

        var didBreak = false;

        Vector2Int gridpos = (Vector2Int)tileManager.grid.WorldToCell(worldpos);

        // check breakable layers for a tile at that position
        // walls
        if(tileManager.PickTile(TileMapLayer.LayerClass.Wall,gridpos) != null) {
            tileManager.ClearTile(TileMapLayer.LayerClass.Wall,gridpos);
            didBreak = true;
        }

        // only update navmesh if something actually broke
        if(didBreak) this.BuildNavMesh();
        return didBreak;
    }

    private void Start() {
        UpdateNavMesh.AddListener(BuildNavMesh);

        roomVariants.AddRange(GetComponentsInChildren<RoomVariantData>(true));

        foreach(var variant in roomVariants) {
            variant.Init();
        }

        if (AutoCreate)
        {
            StartCoroutine(AutoStartCoroutine());
        }
    }

    private IEnumerator AutoStartCoroutine()
    {
        GenerateLevel();
        yield return null;
        BuildLevelFromMap();
        yield return null;
        BuildNavMesh();
        yield return null;
        GenerateShadows();
        yield return null;
        CreateSpawnList();
        yield return null;
        SpawnList();
    }

    public void GenerateLevel()
    {
        switch(LevelTransitionManager.Instance.LevelNumber) {
            case 0: mapGenRoomCount = 12; break;
            case 1: mapGenRoomCount = 16; break;
            default: mapGenRoomCount = 20; break;
        }
        levelMap.StartRoomGen(mapGenRoomCount);
    }

    public void BuildLevelFromMap() {

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
                new Vector2Int(-(STATIC_ROOM_SIZE/2),-(STATIC_ROOM_SIZE/2)),
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

            // place doorways
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
                        //Instantiate(doorPrefab, roompos+dir*(halfwidth)+perp*doorpos+(Vector2.one*0.5f), Quaternion.identity);
                        break;
                    case 2: // open
                        for(int j = (int)(-halfwidth)+1; j < halfwidth; j++) {
                            var cellpos = j;
                            tileManager.ClearTile(TileMapLayer.LayerClass.Wall, (Vector2Int)tileManager.grid.WorldToCell(roompos+dir*(halfwidth)+perp*cellpos));
                        }
                        break;
                        
                }
            }

            

            // Get a random room variant
            RoomVariantData roomVariant = this.roomVariants[Random.Range(0,roomVariants.Count)];

            // rotate the room variant at random
            int randir = Random.Range(0,4);
            // place internal walls
            var wallTile = tileManager.PickTile(TileMapLayer.LayerClass.Palette, new Vector2Int(1,0));
            foreach(var pos in roomVariant.walls)
            {
                tileManager.SetTile(TileMapLayer.LayerClass.Wall, roompos+Directions.Rotate2DInt(pos, randir), wallTile);
            }

            // Rotate Spawnpoints (translate to account for 0.5 offset)
            foreach(var obj in roomVariant.SpawnPoints)
            {
                // We need to intentionally duplicate spawn points as we are never actually instantiating the room prefab
                var instance = Instantiate(obj, room.transform.position+obj.transform.localPosition, Quaternion.identity, spawnPointParent);
                instance.transform.RotateAround(new Vector3(room.transform.position.x,room.transform.position.y,0), new Vector3(0,0,1), 90*randir);
                spawners.Add(instance.GetComponent<SpawnController>());
            }

            // place floor
            var floorTile = tileManager.PickTile(TileMapLayer.LayerClass.Palette,new Vector2Int(0,0));
            tileManager.SetRect(TileMapLayer.LayerClass.Floor, roompos-(roomsize/2), roompos+(roomsize/2), floorTile);

        }
    }

    public void BuildNavMesh()
    {
        foreach (var nav in monsterNavs)
        {
            nav.UpdateMesh();
        }
    }

    public void GenerateShadows()
    {
        tileManager.GenerateShadows();
    }

    public IEnumerable WaitBuildNavMesh()
    {
        foreach (var nav in monsterNavs)
        {
            AsyncOperation asyncOp = nav.UpdateMesh();
            while (asyncOp.isDone == false)
            {
                yield return null;
            }
        }
    }

    public void CreateSpawnList()
    {
        spawnListPrefabs = spawnPoolManager.GenerateSpawnList(
            itemCount: 5 + Mathf.Clamp((LevelTransitionManager.Instance.LevelNumber), 0, 5),
            primaryMonsterCount: 2 + (LevelTransitionManager.Instance.LevelNumber / 3),
            secondaryMonsterCount: 1 + (LevelTransitionManager.Instance.LevelNumber / 3), 
            environmentCount: 2 + Mathf.Clamp((LevelTransitionManager.Instance.LevelNumber / 2), 0, 3), 
            teleShardCount: 1
            );
    }

    public void SpawnList()
    {

        //spawn the list of things in random spots in the level
        foreach (GameObject prefab in spawnListPrefabs)
        {
            SpawnItem(prefab);
        }

        //Spawn Player
        GameObject playerObj;
        if (PlayerController.Instance != null)
        {
            playerObj = SpawnExistingPlayer(PlayerController.Instance.gameObject);
        }
        else
        {
            playerObj = SpawnItem(playerPrefab);
        }

        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        cameraController.FollowTransform = playerObj.transform;
    }

    private GameObject SpawnItem(GameObject prefab)
    {
        int index = Random.Range(0, spawners.Count);
        GameObject obj = Instantiate(prefab, spawners[index].transform.position + new Vector3(0.5f, 0.5f, 0), new Quaternion());
        obj.transform.parent = null;

        spawners.RemoveAt(index);
        return obj;
    }

    private GameObject SpawnExistingPlayer(GameObject player)
    {
        int index = Random.Range(0, spawners.Count);
        Vector3 spawnPoint = spawners[index].transform.position + new Vector3(0.5f, 0.5f, 0);
        player.transform.position = spawnPoint;

        spawners.RemoveAt(index);
        return player;
    }

    public void DestroySpawners()
    {
        int count = 0;
        while(spawners.Count > 0)
        {
            //destroy the spawner
            Destroy(spawners[0].gameObject);
            //remove the list
            spawners.RemoveAt(0);
            count ++;
        }
        Debug.Log($"Destroyed {count} spawners");
    }

    public void DestroyLevel()
    {
        levelMap.ClearRoomGrid();
        tileManager.Nuke();
        DestroySpawners();
        BuildNavMesh();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(LevelController))]
public class LevelControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelController myScript = (LevelController)target;
        if(GUILayout.Button("Build Layout"))
        {
            myScript.GenerateLevel();
        }

        if(GUILayout.Button("Build Map"))
        {
            myScript.BuildLevelFromMap();
        }

         if(GUILayout.Button("Make Navmesh"))
        {
            myScript.BuildNavMesh();
        }

        if (GUILayout.Button("Generate Shadows"))
        {
            myScript.GenerateShadows();
        }

        if (GUILayout.Button("Spawn List"))
        {
            myScript.CreateSpawnList();
            myScript.SpawnList();
        }

        if (GUILayout.Button("Destroy Map"))
        {
            myScript.DestroyLevel();
        }

    }
}
#endif


