using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// When generating rooms, places them in the world at-scale
/// this will allow easy building of the tilemap
/// </summary>

public class MapController : MonoBehaviour {

    private Queue<Vector2> genQueue = new();
    private Dictionary<Vector2, RoomController> roomGrid = new();
    public Dictionary<Vector2, RoomController> RoomGrid {get => roomGrid;}

    //width and height of the rooms in tiles
    private Vector2Int defaultRoomSize = new Vector2Int(9,9);

    public bool IsGenerating {get; private set;} = false;
    private int genPoints = 0;

    [SerializeField] GameObject roomPrefab;

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {

        // perform one gen cycle
        if(genPoints > 0 && IsGenerating == true && genQueue.Count > 0) {
            var index = genQueue.Dequeue();
            roomGrid.TryGetValue(index, out var currentroom);
            

            // push all directions onto a list
            var connectionsToTry = new List<int>() {0,1,2,3};
            var connectionCount = Random.Range(0,3); // choose a minimum number of connections
            var requiredRoom = false; // we try to generate a new room if possible, so we always reach the room cap

            while(connectionsToTry.Count > 0) {
                if(genPoints <= 0) break; // stop trying if we are out of points
                if(connectionCount <= 0 && requiredRoom) break;

                // pop a random direction from the list
                var tryIndex = Random.Range(0,connectionsToTry.Count);
                var tryDir = connectionsToTry[tryIndex];
                connectionsToTry.RemoveAt(tryIndex);

                // try to build a new room there
                if(currentroom.GetConnectionByIndex(tryDir) == 0) {
                    bool roomCreated = TryCreateFromRoom(index, Directions.IntToVec(tryDir), out var newIndex, (connectionCount > 0));
                    
                    // if we were able to make a room, subtract points and add it to the generation queue
                    if(roomCreated) {
                        genPoints --;
                        genQueue.Enqueue(newIndex);
                        requiredRoom = true;
                    }
                    // since there was no connection here before, reduce required connections by 1
                    connectionCount--;
                }

            }

            // make sure to filp the state when we are done
            if(genPoints <= 0) {
                IsGenerating = false;
                genQueue.Clear();
                Debug.Log("Finished Generating Rooms");
            }
        }
    }

    public void StartRoomGen(int roomcount) {

        this.genPoints = roomcount;
        var startroom = PlaceRoom(Vector2.zero, Vector2.zero, defaultRoomSize.x, defaultRoomSize.y);
        genQueue.Enqueue(Vector2.zero);
        IsGenerating = true;
        genPoints --;
    } 

    //
    // Takes a start index, and checks the index in specified direction for a room
    // returns whether a room could be created
    // do not reference the createdroom index if it returns false.
    //
    public bool TryCreateFromRoom(Vector2 src, Vector2 dir, out Vector2 createdroomindex, bool connectExisting) {
        
        if(!roomGrid.ContainsKey(src)) {
            Debug.Log("Source Room Doesn't Exist");
            createdroomindex = Vector2.zero;
            return false;
        }

        //roomgrid location of new room
        var newroomindex = src+dir;

        int connectiontype = Random.Range((int)1,3); //choose either door or open
        //int connectiontype = 1;

        // check if there's already a room
        if(roomGrid.ContainsKey(newroomindex)) {
            if(connectExisting) {
                // connect to the existing room without making a new room
                roomGrid[src].SetConnectionByDir(dir,connectiontype); //current direction in the old room
                roomGrid[newroomindex].SetConnectionByDir(-dir, connectiontype); //opposite direction in the new room
            }
            createdroomindex = Vector2.zero;
            return false;
        } else {
            // multiply position by the room scale;
            var newroomcontroller = PlaceRoom(newroomindex, new Vector2(newroomindex.x*(defaultRoomSize.x-1),newroomindex.y*(defaultRoomSize.y-1)), defaultRoomSize.x, defaultRoomSize.y);

            // sync the connections on the rooms
            roomGrid[src].SetConnectionByDir(dir,connectiontype); //current direction in the old room
            newroomcontroller.SetConnectionByDir(-dir, connectiontype); //opposite direction in the new room
            roomGrid[newroomindex] = newroomcontroller;
            createdroomindex = newroomindex;
            return true;
        }

    }


    //
    // Makes the room, doesn't do any checks
    //
    public RoomController PlaceRoom(Vector2 index, Vector2 position, int width, int height) {
        
        // Game Object
        var go = Instantiate(roomPrefab, gameObject.transform, false); //subtract half a unit to align to tile grid
        go.transform.Translate(position.x,position.y,0);
        var rc = go.GetComponent<RoomController>();
        var box = go.GetComponent<BoxCollider2D>();

        box.size = new Vector2(width,height);


        // Set Members
        rc.width = width;
        rc.height = height;


        // Add to data structure
        roomGrid.Add(index, rc);

        return rc;
    }

    public void ClearRoomGrid() {
        //simply destroy the entire map
        foreach(var pair in RoomGrid) {
            Destroy(pair.Value.gameObject);
        }
        RoomGrid.Clear();
        Debug.Log("Map Destroyed!");
    }



    private void OnDrawGizmosSelected() {
        Gizmos.color = new Color(1,0,1,0.2f);
        Gizmos.DrawCube(gameObject.transform.position, new Vector3(defaultRoomSize.x,defaultRoomSize.y,0));
    }
}