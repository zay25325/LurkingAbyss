using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileController : MonoBehaviour
{
    [SerializeField] Dictionary<string, Tilemap> layers = new();



    // reference the tilemaps in our layers dict
    void Start()
    {
        Tilemap[] getTilemaps = GetComponentsInChildren<Tilemap>();
        foreach(var item in getTilemaps) {
            this.layers.Add(item.gameObject.name, item);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setTile(string layer, Vector2Int pos, Tile tile) {
        var tileMap = layers[layer];
        tileMap.SetTile(new Vector3Int(pos.x,pos.y,0), tile);
    }

    public void ClearLayer(string layer) {
        var gameObject = layers[layer];
        var tileMap = gameObject.GetComponent<Tilemap>();
        tileMap.ClearAllTiles();
    }

    public void CloneTile(string srcLayer, Vector2Int srcPos, string destLayer, Vector2Int destPos) {
    
        var source = layers[srcLayer].GetComponent<Tilemap>();
        var destination = layers[destLayer].GetComponent<Tilemap>();
        
        var t = source.GetTile(new Vector3Int(srcPos.x,srcPos.y,0));
        destination.SetTile(new Vector3Int(destPos.x,destPos.y,0), t);
    }

    //
    // Clones an area of tiles from one layer to another
    //
    public void CloneRect(string srcLayer, Vector2Int srcpos, Vector2Int srcSize, string destLayer, Vector2Int destpos) {
    
        //todo: use get tile block

        var source = layers[srcLayer].GetComponent<Tilemap>();
        var destination = layers[destLayer].GetComponent<Tilemap>();
    }
}
