
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEditor.Toolbars;
public enum Tabs
{
    All,
    Plants,
    Seeds,
    Other,
}
public class Inventory : MonoBehaviour
{
    public int money = 0;
    public TextMeshProUGUI moneyText;
    public GameObject inventoryPanel;
    public TextMeshProUGUI DetailText;
    public Transform contentParent;
    public GameObject slotPrefab;

    public Transform tabParent;
    public Sprite activeTab;
    public Sprite inactiveTab;
    private Tabs currentTab = Tabs.All;
    private List<GameObject> activeSlots = new();

    private Dictionary<PlantData, int> seeds = new();
    private Dictionary<PlantData, int> plants = new();

    public static Inventory instance;
    private void Start()
    {
        foreach (PlantData data in TileUpdater.instance.startPlants)
        {
            AddSeeds(data, 2);
        }
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
        money += amount * data.sellPrice;
        SetPlant(data, 0);
        string formatted = money.ToString("D3");
        moneyText.text = formatted + "g";
    }
    private void BuySeeds(PlantData data)
    {
        if (data.seedCost <= 0)
            return;
        int amount = money / data.seedCost;
        if (amount <= 0)
            return;
        AddSeeds(data, amount);
        money -= amount * data.seedCost;
        string formatted = money.ToString("D3");
        moneyText.text = formatted + "g";
    }
    public void InitialiseInv()
    {
        foreach (GameObject slot in activeSlots)
        {
            Destroy(slot);
        }
        activeSlots.Clear();
        void CreateSlots<T>(Dictionary<T, int> dictionary, Tabs itemCatagory) where T : PlantData
        {
            if (currentTab != Tabs.All && currentTab != itemCatagory)
                return;
            foreach (var kvp in dictionary)
            {
                if (kvp.Value <= 0) continue;
                GameObject slotObj = Instantiate(slotPrefab, contentParent);
                activeSlots.Add(slotObj);
                InventoryItem item = slotObj.GetComponent<InventoryItem>();
                String details = itemCatagory == Tabs.Seeds ? kvp.Key.seedDetails : kvp.Key.plantDetails;
                item.GetComponent<Button>().onClick.AddListener(() => ShowDetails(details));
                item.countText.text = kvp.Value.ToString();
                item.Sprite.sprite = itemCatagory == Tabs.Seeds ? kvp.Key.seedinventorySprite : kvp.Key.inventorySprite;
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
    }
    public void ShowDetails(string details)
    {
        DetailText.text = details;
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
    }
    public void RemovePlant(PlantData data, int amount)
    {
        int current = TryGetPlant(data);
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
        Debug.Log(current + " - " + amount + " = " + (current - amount));
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