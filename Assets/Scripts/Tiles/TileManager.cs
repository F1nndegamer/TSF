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
    public void PlantTile(Vector3Int position, PlantData plantData)
    {
        TileData data = GetorCreateTile(position);
        if (data.groundType == GroundType.Grass)
        {
            return;
        }
        if(data.hasCrop)
            return;
        if(PlantManager.instance.SpawnPlant(position, plantData))
        {
            data.hasCrop = true;
        }
    }
    public void HarvestTile(Vector3Int position)
    {
        TileData data = GetorCreateTile(position);
        if (!data.hasCrop)
            return;
        PlantManager.instance.HarvestPlant(position);
    }
    public void UntillTile(Vector3Int position)
    {
        TileData data = GetorCreateTile(position);
        if (data.hasCrop)
            return;
        if(data.groundType != GroundType.Tilled)
            return;
        data.groundType = GroundType.Grass;
        groundTilemap.SetTile(position, grassTile);
    }
    public void UnWaterTile(Vector3Int position)
    {
        TileData data = GetorCreateTile(position);

        if (data.groundType != GroundType.Watered)
            return;
        data.groundType = GroundType.Tilled;
        groundTilemap.SetTile(position, tilledTile);
    }
}