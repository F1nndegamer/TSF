using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantManager : MonoBehaviour
{
    public static PlantManager instance;
    public Tilemap plantTileMap;
    private Dictionary<Vector3Int, PlantInstance> plants = new();
    void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        UpdateGrowth();
    }
    public void HarvestPlant(Vector3Int position)
    {
        if (!plants.TryGetValue(position, out PlantInstance plant))
            return;
        if (!plant.IsFullyGrown())
            return;

        //Add Plants to inventor
        Debug.Log(Inventory.instance == null);
        Debug.Log(plant.data.plantName + plant.data.harvestYield);
        Inventory.instance.AddPlant(plant.data, plant.data.harvestYield);

        TileManager.instance.GetorCreateTile(position).hasCrop = false;
        plants.Remove(position);
        plantTileMap.SetTile(position, null);
    }
    public bool SpawnPlant(Vector3Int position, PlantData data)
    {
        //So I have position, PlantData and TileData. tileData is important for the tile being watered etc, and that will go into the Plant.cs script.

        if (plants.ContainsKey(position))
            return false;
        if (data == null || data.growStages.Length == 0)
            return false;
        if (Inventory.instance.TryGetSeeds(data) > 0)
        {
            Inventory.instance.RemoveSeeds(data, 1);
        }
        else
        {
            return false;
        }
        PlantInstance plant = new PlantInstance(data);
        plants.Add(position, plant);
        plantTileMap.SetTile(position, data.growStages[0]);
        return true;
    }
    private void UpdateGrowth()
    {
        foreach (var pair in plants)
        {
            Vector3Int position = pair.Key;
            PlantInstance plant = pair.Value;
            if (plant.IsFullyGrown())
            {
                TileManager.instance.UnWaterTile(position);
                continue;
            }
            TileData tileData = TileManager.instance.GetorCreateTile(position);
            if (tileData.groundType != GroundType.Watered)
                continue;
            plant.growthProgress += Time.deltaTime;
            int newstage = Mathf.Clamp(Mathf.FloorToInt(plant.growthProgress / plant.data.growthTime * plant.data.growStages.Length), 0, plant.data.growStages.Length - 1);
            if (newstage != plant.currentStage)
            {
                plant.currentStage = newstage;
                plantTileMap.SetTile(position, plant.data.growStages[newstage]);
            }
        }
    }
    public PlantInstance GetPlant(Vector3Int position)
    {
        plants.TryGetValue(position, out PlantInstance plant);
        return plant;
    }
}
