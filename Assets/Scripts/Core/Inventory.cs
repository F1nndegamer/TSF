
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
            SellPlants();
        }
        if (kb.digit7Key.wasPressedThisFrame)
        {
            BuySeeds();
        }
        if (kb.eKey.wasPressedThisFrame)
        {
            switchInventory();
        }
    }
    public void switchInventory()
    {
        GameManager.IsInventoryOpen = !GameManager.IsInventoryOpen; //Set Gamemanager to correct state
        bool isOpen = GameManager.IsInventoryOpen; //IsOpen will be set to correct state
        inventoryPanel.SetActive(isOpen);
        MouseToGrid.Instance.ChangeCursor(isOpen); //if isOpen force the cursor to be visible
        if (isOpen)
            InitialiseInv();

    }

    private void SellPlants()
    {
        PlantData data = TileUpdater.instance.SelectedPlant;
        int amount = TryGetPlant(data);
        if (amount <= 0)
            return;
        money += amount * data.sellPrice;
        SetPlant(data, 0);
    }
    private void BuySeeds()
    {
        PlantData data = TileUpdater.instance.SelectedPlant;
        if (data.seedCost <= 0)
            return;
        int amount = money / data.seedCost;
        if (amount <= 0)
            return;
        AddSeeds(data, amount);
        money -= amount * data.seedCost;
    }
    public void InitialiseInv()
    {

    }
    #region PlantInv
    public void AddPlant(PlantData data, int amount)
    {
        plants.TryGetValue(data, out int current);
        plants[data] = current + amount;
    }
    public void RemovePlant(PlantData data, int amount)
    {
        int current = TryGetPlant(data);
        if (current - amount <= 0)
        {
            Debug.Log("Negative plants");
            return;
        }
        plants[data] = current - amount;
    }
    public void SetPlant(PlantData data, int amount)
    {
        plants[data] = Mathf.Max(0, amount);
    }
    public int TryGetPlant(PlantData data)
    {
        if (!plants.TryGetValue(data, out int amount))
        {
            plants.Add(data, 0);
            amount = 0;
        }
        return amount;
    }
    public void AddSeeds(PlantData data, int amount)
    {
        seeds.TryGetValue(data, out int current);
        seeds[data] = current + amount;
    }
    public void RemoveSeeds(PlantData data, int amount)
    {
        int current = TryGetSeeds(data);
        if (current - amount <= 0)
        {
            Debug.Log("Negative Seeds");
            return;
        }
        seeds[data] = current - amount;
    }
    public void SetSeed(PlantData data, int amount)
    {
        seeds[data] = Mathf.Max(0, amount);
    }
    public int TryGetSeeds(PlantData data)
    {
        if (!seeds.TryGetValue(data, out int amount))
        {
            seeds.Add(data, 0);
            amount = 0;
        }
        return amount;
    }
    #endregion

}