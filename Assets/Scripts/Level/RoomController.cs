using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RoomController : MonoBehaviour
{
    [SerializeField] public int width = 0;
    [SerializeField] public int height = 0;
   
    // this must not be serialized, or it fails to initialize properly
    [HideInInspector] public int[] connections = new int[4] {0,0,0,0};

    public void SetConnectionByDir(Vector2 dir, int set) {
        this.connections[Directions.VecToInt(dir)] = set;
    }

    public int GetConnectionByDir(Vector2 dir) {
        return this.connections[Directions.VecToInt(dir)];
    }
    public int GetConnectionByIndex(int i) {
        return this.connections[i];
    }



    private void OnDrawGizmos() {
        Gizmos.color = new Color(0.5f,0.5f,0.2f,0.5f);
        Gizmos.DrawCube(transform.position, new Vector3(width-1f,height-1f,0));

        for(int i = 0; i < connections.Length; i++) {
            if(connections[i] == 1) {
                Vector2 dir = Directions.IntToVec(i);
                Vector2 pos = new Vector2(transform.position.x,transform.position.y);
                Gizmos.color = new Color(0.2f,1f,0.2f,0.5f);
                Gizmos.DrawRay(new Ray( pos+(dir*(width/2-0.5f)), dir) );
            }

            if(connections[i] == 2) {
                Vector2 dir = Directions.IntToVec(i);
                Vector2 pos = new Vector2(transform.position.x,transform.position.y);
                Gizmos.color = new Color(1f,0.2f,0.2f,0.5f);
                Gizmos.DrawRay(new Ray( pos+(dir*(width/2-0.5f)), dir) );
            }
        }
    }
}
