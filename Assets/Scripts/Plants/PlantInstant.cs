using UnityEngine;

[System.Serializable]
public class PlantInstance
{
    public PlantData data;
    public float growthProgress;
    public int currentStage;
    public PlantInstance(PlantData plantData)
    {
        data = plantData;
        growthProgress = 0f;
        currentStage = 0;
    }
    public bool IsFullyGrown()
    {
        return growthProgress >= data.growthTime;
    }
}