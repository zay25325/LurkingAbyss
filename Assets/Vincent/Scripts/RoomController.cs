using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [SerializeField] public int width = 0;
    [SerializeField] public int height = 0;
    //TODO: turn this into it's own object. this is ridiculous
    [SerializeField] public List<int> connections = new() {0,0,0,0};

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetConnectionByDir(Vector2 dir, int set) {
        this.connections[Caridnals.cardToInt[dir]] = set;
    }

    public int GetConnectionByDir(Vector2 dir) {
        return this.connections[Caridnals.cardToInt[dir]];
    }

    private void OnDrawGizmos() {
        Gizmos.color = new Color(0.5f,0.5f,0.2f,0.5f);
        Gizmos.DrawCube(transform.position, new Vector3(width-0.5f,height-0.5f,0));

        for(int i = 0; i < connections.Count; i++) {
            if(connections[i] == 1) {
                Vector2 dir = Caridnals.intToCard[i];
                Vector2 pos = new Vector2(transform.position.x,transform.position.y);
                Gizmos.color = new Color(0.2f,1f,0.2f,0.5f);
                Gizmos.DrawRay(new Ray( pos+(dir*(width/2-0.5f)), dir) );
            }
        }
    }
}
