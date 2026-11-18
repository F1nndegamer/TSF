using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Plant", menuName = "Farming/Plant", order = 1)]
public class PlantData : ScriptableObject
{
    public string plantName;

    public Sprite inventorySprite;
    public Location location = Location.Inventory;
    [TextArea(5, 15)]
    public String plantDetails;
    public int harvestYield = 2;
    public SeedData seeds;
    public int sellPrice;
}