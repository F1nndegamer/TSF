using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Plant", menuName = "Farming/Plant", order = 1)]
public class PlantData : ScriptableObject
{
    public string plantName;

    public float growthTime = 24f;
    // 0 is initial
    public TileBase[] growStages;

    public int harvestYield = 2;
    public int seedCost = 1;
}