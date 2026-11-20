
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;
public enum Tabs
{
    All,
    Plants,
    Seeds,
    Other,
}
public class Inventory : MonoBehaviour
{
    public TextMeshProUGUI moneyText;
    public GameObject inventoryPanel;
    public TextMeshProUGUI DetailText;
    public Transform itemParent;
    public Transform containerParent;
    public GameObject slotPrefab;
    public Transform hotbarpParent;

    public Transform tabParent;
    public Sprite activeTab;
    public Sprite inactiveTab;
    public Sprite activeContainer;
    public Sprite inactiveContainer;
    private Tabs currentTab = Tabs.All;
    private List<GameObject> activeSlots = new();

    private Dictionary<PlantData, int> seeds = new();
    private Dictionary<PlantData, int> plants = new();
    private List<Transform> itemSlots = new();
    private List<Transform> itemParents = new();
    public static Inventory instance;
    private GameObject SelectedItem;
    private bool SelectedSeeds;
    public PlantData SelectedPlantData;
    private void Start()
    {
        tabParent.GetChild(0).GetComponent<Button>().onClick.AddListener(() => SwitchTabs(Tabs.All));
        tabParent.GetChild(1).GetComponent<Button>().onClick.AddListener(() => SwitchTabs(Tabs.Seeds));
        tabParent.GetChild(2).GetComponent<Button>().onClick.AddListener(() => SwitchTabs(Tabs.Plants));
        tabParent.GetChild(3).GetComponent<Button>().onClick.AddListener(() => SwitchTabs(Tabs.Other));
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
            SellPlants(TileUpdater.instance.selectedPlant);
        }
        if (kb.digit7Key.wasPressedThisFrame)
        {
            BuySeeds(TileUpdater.instance.selectedPlant);
        }
        if (kb.eKey.wasPressedThisFrame)
        {
            switchInventory();
        }
        if (kb.rKey.wasPressedThisFrame)
        {

            //Either add seeds or plant to Hotbar, we gotta figure out which one we have selected first, than either call movePlants or moveSeeds to hotbar
            if (SelectedSeeds)
            {
                MoveSeedToHotBar(SelectedPlantData);
            }
            else
            {
                MovePlantToHotBar(SelectedPlantData);
            }
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

    private void SellPlants(PlantData data)
    {
        int amount = TryGetPlant(data);
        if (amount <= 0)
            return;
        GameManager.Money += amount * data.sellPrice;
        SetPlant(data, 0);
        RefreshMoney();
    }
    private void BuySeeds(PlantData data)
    {
        if (data.seeds.seedCost <= 0)
            return;
        int amount = GameManager.Money / data.seeds.seedCost;
        if (amount <= 0)
            return;
        AddSeeds(data, amount);
        GameManager.Money -= amount * data.seeds.seedCost;
        RefreshMoney();
    }
    public void RefreshMoney()
    {
        string formatted = GameManager.Money.ToString("D3");
        moneyText.text = formatted + "g";
    }
    public void InitialiseInv()
    {
        for(int i = itemParent.childCount - 1; i >= 0; i--)
        {
            Destroy(itemParent.GetChild(i).gameObject);
        }

        activeSlots.Clear();
        void CreateSlots<T>(Dictionary<T, int> dictionary, Tabs itemCatagory) where T : PlantData
        {
            if (currentTab != Tabs.All && currentTab != itemCatagory)
                return;
            foreach (var kvp in dictionary)
            {
                if (kvp.Value <= 0) continue;
                if (itemCatagory == Tabs.Seeds) if (kvp.Key.seeds.location != Location.Inventory) continue;
                if (itemCatagory == Tabs.Plants) if (kvp.Key.location != Location.Inventory) continue;
                Debug.Log("AAAAh" + itemParent.childCount);
                GameObject slotObj = Instantiate(slotPrefab, itemParent);
                Debug.Log("Added");
                activeSlots.Add(slotObj);
                InventoryItem item = slotObj.GetComponent<InventoryItem>();
                String details = itemCatagory == Tabs.Seeds ? kvp.Key.seeds.seedDetails : kvp.Key.plantDetails;

                int slotNumber = -1;
                for (int i = 0; i < itemParent.childCount; i++)
                {
                    Debug.Log(itemParent.childCount);
                    if (itemParent.GetChild(i).gameObject == slotObj)
                    {
                        slotNumber = i;
                    }
                }

                item.GetComponent<Button>().onClick.AddListener(() => ShowDetails(details, slotNumber, itemCatagory.ToString(), kvp.Key));
                item.countText.text = kvp.Value.ToString();
                item.Sprite.sprite = itemCatagory == Tabs.Seeds ? kvp.Key.seeds.seedinventorySprite : kvp.Key.inventorySprite;
            }
        }
        if (currentTab == Tabs.All || currentTab == Tabs.Seeds)
        {
            CreateSlots(seeds, Tabs.Seeds);
        }
        if (currentTab == Tabs.All || currentTab == Tabs.Plants)
        {
            CreateSlots(plants, Tabs.Plants);
        }
        TileUpdater.instance.DrawHotBar();
    }

    public void ShowDetails(string details, int slotNumber, string plantOrSeed, PlantData data)
    {
        foreach (Transform child in containerParent)
        {
            child.GetComponent<Image>().sprite = inactiveContainer;
        }
        SelectedItem = containerParent.GetChild(slotNumber).gameObject;
        containerParent.GetChild(slotNumber).GetComponent<Image>().sprite = activeContainer;
        DetailText.text = details;

        SelectedSeeds = plantOrSeed == "Seeds";
        SelectedPlantData = data;
        Debug.Log(plantOrSeed);
    }
    public void SwitchTabs(Tabs tab)
    {
        currentTab = tab;
        List<GameObject> tabObject = new();
        foreach (Transform child in tabParent)
        {
            tabObject.Add(child.gameObject);
            child.GetComponent<Image>().sprite = inactiveTab;
        }
        switch (tab)
        {
            case Tabs.All:
                tabParent.transform.GetChild(0).GetComponent<Image>().sprite = activeTab;
                break;
            case Tabs.Seeds:
                tabParent.transform.GetChild(1).GetComponent<Image>().sprite = activeTab;
                break;
            case Tabs.Plants:
                tabParent.transform.GetChild(2).GetComponent<Image>().sprite = activeTab;
                break;
            case Tabs.Other:
                tabParent.transform.GetChild(3).GetComponent<Image>().sprite = activeTab;
                break;
        }
        if (GameManager.IsInventoryOpen)
        {
            InitialiseInv();
        }
    }
    #region PlantInv
    public void SetActiveSeed(PlantData data)
    {
        TileUpdater.instance.selectedPlant = data;
    }
    public void AddPlant(PlantData data, int amount)
    {
        plants.TryGetValue(data, out int current);
        plants[data] = current + amount;
        Journal.instance.DiscoverCrop(data);
        TileUpdater.instance.DrawHotBar();
    }
    public void RemovePlant(PlantData data, int amount)
    {
        int current = TryGetPlant(data);
        plants[data] = current - amount;
        TileUpdater.instance.DrawHotBar();
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
        TileUpdater.instance.DrawHotBar();
    }
    public void RemoveSeeds(PlantData data, int amount)
    {
        int current = TryGetSeeds(data);
        Debug.Log(current + " - " + amount + " = " + (current - amount));
        seeds[data] = current - amount;
        TileUpdater.instance.DrawHotBar();
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
    public int GetNextHotBarSlot()
    {
        HashSet<int> occupied = new();
        foreach (PlantData data in plants.Keys)
        {
            if (data.location == Location.hotBar)
                occupied.Add(data.HotBarSlot);
        }
        foreach (PlantData data in seeds.Keys)
        {
            if (data.seeds.location == Location.hotBar)
                occupied.Add(data.seeds.HotBarSlot);
        }
        List<int> empties = TileUpdater.instance.GetEmptySlots();
        foreach (int noTool in empties)
        {
            if (!occupied.Contains(noTool))
            {
                return noTool;
            }
        }
        return -1;
    }
    public void MoveSeedToHotBar(PlantData data)
    {
        if (data.seeds.location == Location.hotBar)
            return;
        int position = GetNextHotBarSlot();
        Debug.Log(position);
        if (position == -1)
            return;
        data.seeds.location = Location.hotBar;
        data.seeds.HotBarSlot = position;
        TileUpdater.instance.barslots[position].function = Function.Seeds;
        TileUpdater.instance.barslots[position].data = data;
        TileUpdater.instance.barslots[position].sprite = data.seeds.seedinventorySprite;
        InitialiseInv();
    }
    public void MovePlantToHotBar(PlantData data)
    {
        if (data.location == Location.hotBar)
            return;
        int position = GetNextHotBarSlot();
        if (position == -1)
            return;
        data.location = Location.hotBar;
        data.HotBarSlot = position;
        TileUpdater.instance.barslots[position].function = Function.Plant;
        TileUpdater.instance.barslots[position].data = data;
        TileUpdater.instance.barslots[position].sprite = data.inventorySprite;
        InitialiseInv();
    }
    public void MoveSeedToInv(PlantData data)
    {
        int slotIndex = data.seeds.HotBarSlot;
        data.seeds.location = Location.Inventory;
        data.seeds.HotBarSlot = -1;
        if (slotIndex >= 0 && slotIndex < TileUpdater.instance.barslots.Count)
            TileUpdater.instance.barslots[slotIndex].ClearSlot();
        TileUpdater.instance.DrawHotBar();
        InitialiseInv();
    }

    public void MovePlantToInv(PlantData data)
    {
        int slotIndex = data.HotBarSlot;
        data.location = Location.Inventory;
        data.HotBarSlot = -1;

        if (slotIndex >= 0 && slotIndex < TileUpdater.instance.barslots.Count)
            TileUpdater.instance.barslots[slotIndex].ClearSlot();

        TileUpdater.instance.DrawHotBar();
        InitialiseInv();
    }
}