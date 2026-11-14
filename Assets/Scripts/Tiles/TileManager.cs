using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public static TileManager instance;
    public Tilemap groundTilemap;
    private Dictionary<Vector3Int, TileData> tiles = new();

    //TileAssets
    public TileBase grassTile;
    public TileBase tilledTile;
    public TileBase wateredTile;
    void Awake()
    {
        instance = this;
    }
    public void RegisterTile(Vector3Int position, GroundType type)
    {
        tiles[position] = new TileData(type);
    }
    public TileData GetorCreateTile(Vector3Int position)
    {
        if(!tiles.TryGetValue(position, out TileData data))
        {
            data = new TileData(GroundType.Grass);
            tiles.Add(position, data);
        }
        return data;
    }
    public void SetGroundTile(Vector3Int position, GroundType type)
    {
        TileData data = GetorCreateTile(position);

        if(data == null)
            return;
        data.groundType = type;

        TileBase tile = type switch
        {
            GroundType.Grass => grassTile,
            GroundType.Tilled => tilledTile,
            GroundType.Watered => wateredTile,
            _ => grassTile
        };
        groundTilemap.SetTile(position, tile);
    }
}