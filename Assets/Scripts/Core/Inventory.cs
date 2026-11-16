
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;
public class Inventory : MonoBehaviour
{
    private Dictionary<PlantData, int> seeds = new();
    private Dictionary<PlantData, int> plants = new();
    public int money = 0;
    public static Inventory instance;
    public GameObject inventoryPanel;
    private void Start()
    {
        AddSeeds(TileUpdater.instance.SelectedPlant, 2);
    }
    void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        //TODO: Actual selling mechanics, for now, just one plant, the temp one in TileUpdater
        Keyboard kb = Keyboard.current;
        if (kb == null)
            return;
        if (kb.digit6Key.wasPressedThisFrame)
        {
            PlantData data = TileUpdater.instance.SelectedPlant;
            money = plants[data] * data.harvestYield;
        }
        if (kb.digit7Key.wasPressedThisFrame)
        {
            PlantData data = TileUpdater.instance.SelectedPlant;
            int seedBuyAmount = money / data.seedCost;
            AddSeeds(data, seedBuyAmount);
            money = 0;
        }
        if(kb.eKey.wasPressedThisFrame)
        {
            GameManager.IsInventoryOpen = !GameManager.IsInventoryOpen;
            inventoryPanel.SetActive(GameManager.IsInventoryOpen);
            InitialiseInv();
        }
    }
    public void InitialiseInv()
    {
        
    }
    #region PlantInv
    public void AddPlant(PlantData data, int amount)
    {
        if (!plants.TryGetValue(data, out int plantamount))
        {
            plants.Add(data, amount);
            return;
        }
        plants[data] = plantamount + amount;
    }
    public void RemovePlant(PlantData data, int amount)
    {
        if (!plants.TryGetValue(data, out int plantamount))
        {
            plants.Add(data, amount);
            return;
        }
        plants[data] = plantamount - amount;
    }
    public void SetPlant(PlantData data, int amount)
    {
        if (!plants.TryGetValue(data, out int plantamount))
        {
            plants.Add(data, amount);
            return;
        }
        plants[data] = amount;
    }
    public int TryGetPlant(PlantData data)
    {
        if (!plants.TryGetValue(data, out int plantamount))
        {
            return plantamount;
        }
        return int.MinValue;
    }
    public void AddSeeds(PlantData data, int amount)
    {
        if (!seeds.TryGetValue(data, out int seedamount))
        {
            seeds.Add(data, amount);
            return;
        }
        seeds[data] = seedamount + amount;
    }
    public void RemoveSeeds(PlantData data, int amount)
    {
        if (!seeds.TryGetValue(data, out int seedamount))
        {
            seeds.Add(data, amount);
            return;
        }
        seeds[data] = seedamount - amount;
    }
    public void SetSeeds(PlantData data, int amount)
    {
        if (!seeds.TryGetValue(data, out int seedamount))
        {
            seeds.Add(data, amount);
            return;
        }
        seeds[data] = amount;
    }
    public int TryGetSeeds(PlantData data)
    {
        if (seeds.TryGetValue(data, out int seedamount))
        {
            return seedamount;
        }
        return int.MinValue;
    }
    #endregion

}