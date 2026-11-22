using UnityEngine;
using System;

[Serializable]
public class ShopData
{
    public string itemName;
    public PlantData plantData;
    public bool sellsSeeds = true;
    [TextArea(2, 5)]
    public string customDescripton;
}
