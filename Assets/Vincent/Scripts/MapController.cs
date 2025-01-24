using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When generating rooms, places them in the world at-scale
/// this will allow easy building of the tilemap
/// </summary>

public class MapController : MonoBehaviour {

    private Queue<Vector2> genQueue;
    private Dictionary<Vector2, RoomController> roomGrid = new();

    //width and height of the rooms in tiles
    private int defaultRoomSize = 7;

    private bool isGenerating = false;
    private int genPoints = 0;

    [SerializeField] GameObject roomPrefab;

    // Start is called before the first frame update
    void Start() {
        startRoomGen(1, Vector2.zero);
    }

    // Update is called once per frame
    void Update() {

        // perform one gen cycle
        if(genPoints > 0 && isGenerating == true && genQueue.Count > 0) {
            var currentroom = genQueue.Dequeue();
            
            // select random directions to connect to
            int r = Random.Range(1,16);
            int[] connections = {
                (r >> 0) & 1,
                (r >> 1) & 1,
                (r >> 2) & 1,
                (r >> 3) & 1
            };

            foreach(int c in connections) {
                if(c == 1 && currentroom.connections[c] == 0) { //make sure there isn't already a connection there
                    var result = TryCreateFromRoom(currentroom)
                }
            }


        }
    }

    public void startRoomGen(int roomcount, Vector2 origin) {

        this.genPoints = roomcount;
        var startroom = Instantiate(roomPrefab, origin, Quaternion.identity);
        var startroomcontroller = startroom.GetComponent<RoomController>();

        genQueue.Enqueue(startroomcontroller);
        isGenerating = true;
        genPoints --;
    } 

    public bool TryCreateFromRoom(Vector2 src, Vector2 dir, out Vector2 createdroomindex) {
        
        if(!roomGrid.TryGetValue(src, out _)) {
            Debug.Log("Source Room Doesn't Exist");
            createdroomindex = Vector2.zero;
            return false;
        }

        //roomgrid location of new room
        var newroomindex = src+dir;

        // check if there's already a room
        if(roomGrid.TryGetValue(newroomindex, out var existingroomcontroller)) {
            // either connect to returned room or abort
            createdroomindex = Vector2.zero;
            return false;
        } else {
            //multiply position by the room scale to place it in the world properly;
            var newroomcontroller = PlaceRoom(newroomindex, newroomindex*defaultRoomSize, defaultRoomSize, defaultRoomSize);

            //add the new room to our dictionary
            

            //open connections on the rooms
            roomGrid[src].SetConnectionByDir(dir,1); //current direction in the old room
            newroomcontroller.SetConnectionByDir(-dir, 1); //opposite direction in the new room
            roomGrid[newroomindex] = newroomcontroller;
            createdroomindex = newroomindex;
            return true;
        }

    }

    public RoomController PlaceRoom(Vector2 index, Vector2 position, int width, int height) {
        var go = Instantiate(roomPrefab, position, Quaternion.identity);
        var rc = go.GetComponent<RoomController>();

        rc.width = width;
        rc.height = height;

        return rc;
    }

    public void ClearRoomGrid() {
        //todo, implement logic to wipe the room grid
    }
}
