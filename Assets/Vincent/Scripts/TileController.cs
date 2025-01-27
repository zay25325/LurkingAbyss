using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileController : MonoBehaviour
{
    [SerializeField] Dictionary<string, Tilemap> layers = new();

    [SerializeField] public Grid grid;

    // reference the tilemaps in our layers dict
    void Start()
    {
        this.grid = GetComponent<Grid>();
        Tilemap[] getTilemaps = GetComponentsInChildren<Tilemap>();
        foreach(var item in getTilemaps) {
            this.layers.Add(item.gameObject.name, item);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTile(string layer, Vector2Int pos, TileBase tile) {
        var tileMap = layers[layer];
        tileMap.SetTile(new Vector3Int(pos.x,pos.y,0), tile);
    }

    public TileBase PickTile(string layer, Vector2Int pos) {
        var tileMap = layers[layer];
        return tileMap.GetTile(new Vector3Int(pos.x,pos.y,0));
    }

    public void SetRect(string layer, Vector2Int pos, Vector2Int end, TileBase tile) {
        var tileMap = layers[layer];
        tileMap.BoxFill(new Vector3Int(pos.x,pos.y,0), tile, pos.x, pos.y, end.x, end.y);
    }
    
    public void ClearTile(string layer, Vector2Int pos) {
        var tileMap = layers[layer];
        tileMap.SetTile(new Vector3Int(pos.x,pos.y,0), null);
    }

    public void ClearRect(string layer, Vector2Int pos, Vector2Int size) {
        var tileMap = layers[layer];

        BoundsInt area = new BoundsInt(pos.x,pos.y,0,size.x,size.y,1);

        foreach(var position in area.allPositionsWithin) {
            ClearTile(layer, ((Vector2Int)position));
        }
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
    public void CloneRect(string srcLayer, Vector2Int srcPos, Vector2Int srcSize, string destLayer, Vector2Int destPos) {    

        var source = layers[srcLayer].GetComponent<Tilemap>();
        var destination = layers[destLayer].GetComponent<Tilemap>();

        BoundsInt sourceArea = new BoundsInt(srcPos.x,srcPos.y,0,srcSize.x,srcSize.y,1);
        BoundsInt destinationArea = new BoundsInt(destPos.x,destPos.y,0,srcSize.x,srcSize.y,1);

        TileBase[] sourceBlock = source.GetTilesBlock(sourceArea);
        destination.SetTilesBlock(destinationArea, sourceBlock);
    }
}
