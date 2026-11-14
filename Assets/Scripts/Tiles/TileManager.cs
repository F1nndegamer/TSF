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
    public TileData GetorCreateTile(Vector3Int position)
    {
        if(!tiles.TryGetValue(position, out TileData data))
        {
            data = new TileData(GroundType.Grass);
            tiles.Add(position, data);
        }
        return data;
    }
    public void TillTile(Vector3Int position)
    {
        TileData data = GetorCreateTile(position);

        if(data.groundType != GroundType.Grass)
        {
            return;
        }
        data.groundType = GroundType.Tilled;

        groundTilemap.SetTile(position, tilledTile);
    }
    public void WaterTile(Vector3Int position)
    {
        TileData data = GetorCreateTile(position);

        if (data.groundType != GroundType.Tilled)
        {
            return;
        }
        data.groundType = GroundType.Watered;

        groundTilemap.SetTile(position, wateredTile);
    }
}