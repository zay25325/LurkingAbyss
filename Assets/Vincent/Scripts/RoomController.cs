using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [SerializeField] public int width = 0;
    [SerializeField] public int height = 0;
    [SerializeField] public int[] connections = {0,0,0,0};

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
}
