using UnityEngine;
using System;

[Serializable]
public class ShopData
{
    public string itemName;
    public PlantData plantData;
    public bool sellsSeeds = true;
    public int buyPrice = 10;
    public int sellPrice = 5;
    [TextArea(2, 5)]
    public string customDescripton;
}
