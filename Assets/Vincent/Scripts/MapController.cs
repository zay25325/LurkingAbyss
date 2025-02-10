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
            
            // select random directions to connect to
            // we always choose at least one direction
            // TODO make it only include unconnected walls so we don't get too many deadend rooms
            int r = Random.Range(1,16);
            int[] connections = {
                (r >> 0) & 1,
                (r >> 1) & 1,
                (r >> 2) & 1,
                (r >> 3) & 1
            };

            // For each wall, check if we generate a connection there
            for(int c = 0; c < connections.Length; c++) {
                if(genPoints <= 0) break;

                // only try to make a room if there is no connection on that wall
                if(connections[c] != 0 && currentroom.GetConnectionByIndex(c) == 0) {
                    bool roomCreated = TryCreateFromRoom(index, Directions.IntToVec(c), out var newIndex);
                    
                    // if we were able to make a room, subtract points and add it to the generation queue
                    if(roomCreated) {
                        genPoints --;
                        genQueue.Enqueue(newIndex);
                    }
                }
            }

            // make sure to filp the state when we are done
            if(genPoints <= 0) {
                IsGenerating = false;
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
    public bool TryCreateFromRoom(Vector2 src, Vector2 dir, out Vector2 createdroomindex) {
        
        if(!roomGrid.ContainsKey(src)) {
            Debug.Log("Source Room Doesn't Exist");
            createdroomindex = Vector2.zero;
            return false;
        }

        //roomgrid location of new room
        var newroomindex = src+dir;

        //int connectiontype = Random.Range((int)2,3); //choose either door or open
        int connectiontype = 1;

        // check if there's already a room
        if(roomGrid.ContainsKey(newroomindex)) {
            // connect to the existing room without making a new room
            roomGrid[src].SetConnectionByDir(dir,connectiontype); //current direction in the old room
            roomGrid[newroomindex].SetConnectionByDir(-dir, connectiontype); //opposite direction in the new room
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