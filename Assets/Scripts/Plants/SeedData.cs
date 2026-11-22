using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Seed", menuName = "Farming/Seed", order = 1)]

public class SeedData : ScriptableObject
{
    public string SeedName;
    public Location location = Location.Inventory;
    public int HotBarSlot = - 1;
    public float growthTime = 24f;
    // 0 is initial
    public TileBase[] growStages;
    public float waterCost;
    public Sprite seedinventorySprite;
    [TextArea(5, 15)]
    public String seedDetails;
    public int harvestYield = 2;
    public int buyPrice = 1;
    public int sellPrice;
}

public enum Location
{
    Inventory,
    hotBar,
}