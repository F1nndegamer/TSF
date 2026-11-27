
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
using System.ComponentModel;
using UnityEditor;
using System.Runtime.CompilerServices;
using UnityEngine.Rendering;
using UnityEngine.U2D;
using Unity.Mathematics;
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

    public Image dragCurorImage;

    public Transform tabParent;
    public Sprite activeTab;
    public Sprite inactiveTab;
    public Sprite activeContainer;
    public Sprite inactiveContainer;
    public Button clsoeButton;
    private Tabs currentTab = Tabs.All;
    private List<GameObject> activeSlots = new();

    private Dictionary<PlantData, int> seeds = new();
    private Dictionary<PlantData, int> plants = new();
    public static Inventory instance;

    private GameObject SelectedItem;
    [HideInInspector] public bool SelectedSeeds;
    public PlantData SelectedPlantData;
    private List<(string details, Tabs category, PlantData data)?> slotData = new();
    private List<InventoryItem> slotItems = new();

    private int InvSelected;

    private bool isHoldingItem;
    private PlantData heldPlantData;
    private bool heldIsSeed;
    private Sprite heldSprite;
    private int heldFromInvSlot = -1;
    private int heldFromHotbarSlot = -1;

    private void Start()
    {
        tabParent.GetChild(0).GetComponent<Button>().onClick.AddListener(() => SwitchTabs(Tabs.All));
        tabParent.GetChild(1).GetComponent<Button>().onClick.AddListener(() => SwitchTabs(Tabs.Seeds));
        tabParent.GetChild(2).GetComponent<Button>().onClick.AddListener(() => SwitchTabs(Tabs.Plants));
        tabParent.GetChild(3).GetComponent<Button>().onClick.AddListener(() => SwitchTabs(Tabs.Other));
        for (int i = 0; i < containerParent.childCount; i++)
        {
            int index = i;
            containerParent.GetChild(i).GetComponent<Button>()?.onClick.AddListener(() => OnInventorySlotClicked(index));
        }
        for (int i = 0; i < hotbarpParent.childCount; i++)
        {
            int index = i;
            hotbarpParent.GetChild(i).GetComponent<Button>()?.onClick.AddListener(() => OnHotbarSlotClicked(index));
        }
        dragCurorImage.raycastTarget = false;
        dragCurorImage.gameObject.SetActive(false);
        InputManager.instance.ToggleInventory += SwitchInventory;
        InputManager.instance.MoveToHotbar += MoveToHotbar;
        foreach (PlantData data in plants.Keys)
        {
            data.seeds.location = Location.Inventory;
            data.location = Location.Inventory;
        }
        clsoeButton.onClick.AddListener(() => SetInventory(false));
    }
    void Awake()
    {
        instance = this;
    }
    private void OnDestroy()
    {
        InputManager.instance.ToggleInventory -= SwitchInventory;
        InputManager.instance.MoveToHotbar -= MoveToHotbar;
        clsoeButton.onClick.RemoveAllListeners();
    }
    void Update()
    {
        if (isHoldingItem)
        {
            MoveDragCursor();
        }
    }
    private void MoveDragCursor()
    {
        if (dragCurorImage == null || Pointer.current == null)
            return;

        Vector2 screenPos = Pointer.current.position.ReadValue();
        RectTransform cursorRect = dragCurorImage.rectTransform;
        RectTransform parentRect = cursorRect.parent as RectTransform;
        Canvas canvas = dragCurorImage.canvas;
        if (parentRect == null || canvas == null)
        {
            cursorRect.position = screenPos;
            return;
        }

        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPos, cam, out Vector2 localPoint))
        {
            cursorRect.localPosition = localPoint;
        }
    }
    public Dictionary<PlantData, int> GetAllPlants()
    {
        return plants;
    }
    public void MoveToHotbar()
    {
        if (SelectedPlantData == null)
            return;
        if (SelectedSeeds)
        {
            MoveSeedToHotBar(SelectedPlantData);
        }
        else
        {
            MovePlantToHotBar(SelectedPlantData);
        }
    }
    public void SwitchInventory()
    {
        if (UIManager.instance.TogglePanel(UIPanel.Inventory))
            InitialiseInv();
        else
            CancelItemHeld();
    }
    public void SetInventory(bool open)
    {
        if (open)
        {
            if (UIManager.instance.OpenPanel(UIPanel.Inventory))
                InitialiseInv();
        }
        else
        {
            CancelItemHeld();
            UIManager.instance.ClosePanel(UIPanel.Inventory);
        }

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
        if (data.seeds.buyPrice <= 0)
            return;
        int amount = GameManager.Money / data.seeds.buyPrice;
        if (amount <= 0)
            return;
        AddSeeds(data, amount);
        GameManager.Money -= amount * data.seeds.buyPrice;
        RefreshMoney();
    }
    public void RefreshMoney()
    {
        string formatted = GameManager.Money.ToString("D3");
        moneyText.text = formatted + "g";
    }
    public void InitialiseInv()
    {
        for (int i = activeSlots.Count - 1; i >= 0; i--)
        {
            if (activeSlots[i] != null)
            {
                activeSlots[i].SetActive(false);
                Destroy(activeSlots[i]);
            }
        }
        activeSlots.Clear();
        slotData.Clear();
        slotItems.Clear();

        List<(string details, Tabs category, PlantData data, int count, Sprite sprite)> entries = new();
        void CollectEntries<T>(Dictionary<T, int> dictionary, Tabs itemCatagory) where T : PlantData
        {
            if (currentTab != Tabs.All && currentTab != itemCatagory)
                return;
            foreach (var kvp in dictionary)
            {
                if (kvp.Value <= 0) continue;
                if (itemCatagory == Tabs.Seeds) if (kvp.Key.seeds.location != Location.Inventory) continue;
                if (itemCatagory == Tabs.Plants) if (kvp.Key.location != Location.Inventory) continue;

                string details = itemCatagory == Tabs.Seeds ? kvp.Key.seeds.seedDetails : kvp.Key.plantDetails;
                Sprite sprite = itemCatagory == Tabs.Seeds ? kvp.Key.seeds.seedinventorySprite : kvp.Key.inventorySprite;
                entries.Add((details, itemCatagory, kvp.Key, kvp.Value, sprite));
            }
        }
        if (currentTab == Tabs.All || currentTab == Tabs.Seeds)
        {
            CollectEntries(seeds, Tabs.Seeds);
        }
        if (currentTab == Tabs.All || currentTab == Tabs.Plants)
        {
            CollectEntries(plants, Tabs.Plants);
        }

        for (int i = 0; i < containerParent.childCount; i++)
        {
            Transform slotContainer = containerParent.GetChild(i);
            if (i < entries.Count)
            {
                var entry = entries[i];
                GameObject slotObj = Instantiate(slotPrefab, slotContainer);
                // slotObj.transform.localPosition = Vector3.zero;
                // slotObj.transform.localScale = Vector3.one;

                activeSlots.Add(slotObj);
                slotData.Add((entry.details, entry.category, entry.data));

                InventoryItem item = slotObj.GetComponent<InventoryItem>();
                item.countText.text = entry.count.ToString();
                item.Sprite.sprite = entry.sprite;
                item.Sprite.gameObject.SetActive(true);
                slotItems.Add(item);
            }
            else
            {
                slotData.Add(null);
                slotItems.Add(null);
            }
        }
        TileUpdater.instance.DrawHotBar();
        SelectSlot(Mathf.Clamp(InvSelected, 0, Mathf.Max(0, containerParent.childCount - 1)));
    }

    public void OnHotbarSlotClicked(int index)
    {
        TileUpdater.instance.SelectTool(index + 1);
        if (isHoldingItem)
        {
            PlaceHeldItemInHotbar(index);
            return;
        }

        if (TileUpdater.instance == null || index < 0 || index >= TileUpdater.instance.barslots.Count)
            return;
        var barslots = TileUpdater.instance.barslots[index];
        if (barslots == null || barslots.data == null)
            return;
        bool isSeed = barslots.function == Function.Seeds;
        Sprite sprite = barslots.sprite != null
            ? barslots.sprite
            : (isSeed ? barslots.data.seeds.seedinventorySprite : barslots.data.inventorySprite);
        if (sprite == null)
            return;

        isHoldingItem = true;
        heldPlantData = barslots.data;
        heldIsSeed = isSeed;
        heldSprite = sprite;
        heldFromInvSlot = -1;
        heldFromHotbarSlot = index;
        if (dragCurorImage != null)
        {
            RectTransform cursorRect = dragCurorImage.rectTransform;
            if (hotbarpParent != null && index < hotbarpParent.childCount)
            {
                RectTransform sourceRect = hotbarpParent.GetChild(index) as RectTransform;
                if (sourceRect != null)
                {
                    Vector3[] corners = new Vector3[4];
                    sourceRect.GetWorldCorners(corners);
                    float width = Vector3.Distance(corners[0], corners[3]);
                    float height = Vector3.Distance(corners[0], corners[1]);
                    cursorRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                    cursorRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                }
            }
            dragCurorImage.sprite = heldSprite;
            dragCurorImage.gameObject.SetActive(true);
            MoveDragCursor();
        }
    }
    public void OnInventorySlotClicked(int index)
    {
        SelectSlot(index);
        if (!isHoldingItem)
        {
            if (index < 0 || index >= slotData.Count || !slotData[index].HasValue)
                return;
            var clicked = slotData[index].Value;
            isHoldingItem = true;
            heldPlantData = clicked.data;
            heldIsSeed = clicked.category == Tabs.Seeds;
            heldFromInvSlot = index;

            InventoryItem itemView = slotItems[index];
            heldSprite = itemView != null ? itemView.Sprite.sprite : null;
            if (itemView != null && itemView.Sprite != null)
            {
                itemView.Sprite.gameObject.SetActive(false);
            }
            if (dragCurorImage != null)
            {
                RectTransform sourceRect = itemView.Sprite.rectTransform;
                RectTransform cursorRect = dragCurorImage.rectTransform;

                Vector3[] corners = new Vector3[4];
                sourceRect.GetWorldCorners(corners);

                float width = Vector3.Distance(corners[0], corners[3]);
                float height = Vector3.Distance(corners[0], corners[1]);

                cursorRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                cursorRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                dragCurorImage.sprite = heldSprite;
                dragCurorImage.gameObject.SetActive(heldSprite != null);
                if (Pointer.current != null)
                {
                    MoveDragCursor();
                }
            }
        }
        else
        {
            PlaceHeldItemInInventorySlot(index);
        }
    }
    private void PlaceHeldItemInInventorySlot(int targetindex)
    {
        if (heldPlantData == null)
        {
            CancelItemHeld();
            return;
        }
        if (heldFromInvSlot < 0)
        {
            if (heldIsSeed)
            {
                int oldHotbar = heldFromHotbarSlot >= 0 ? heldFromHotbarSlot : heldPlantData.seeds.HotBarSlot;
                heldPlantData.seeds.location = Location.Inventory;
                heldPlantData.seeds.HotBarSlot = -1;
                if (oldHotbar >= 0 && oldHotbar < TileUpdater.instance.barslots.Count)
                    TileUpdater.instance.barslots[oldHotbar].ClearSlot();
            }
            else
            {
                int oldHotbar = heldFromHotbarSlot >= 0 ? heldFromHotbarSlot : heldPlantData.HotBarSlot;
                heldPlantData.location = Location.Inventory;
                heldPlantData.HotBarSlot = -1;
                if (oldHotbar >= 0 && oldHotbar < TileUpdater.instance.barslots.Count)
                    TileUpdater.instance.barslots[oldHotbar].ClearSlot();
            }
        }
        if (targetindex >= 0 && targetindex < slotData.Count)
        {
            var existingAtTarget = slotData[targetindex];
            slotData[targetindex] = (
                heldIsSeed ? heldPlantData.seeds.seedDetails : heldPlantData.plantDetails,
                heldIsSeed ? Tabs.Seeds : Tabs.Plants,
                heldPlantData
            );

            if (heldFromInvSlot >= 0 && heldFromInvSlot < slotData.Count && heldFromInvSlot != targetindex)
            {
                slotData[heldFromInvSlot] = existingAtTarget;
            }
        }
        ClearHeldState();
        TileUpdater.instance.DrawHotBar();
        InitialiseInv();
    }
    public void PlaceHeldItemInHotbar(int hotbarIndex)
    {
        if (!isHoldingItem || heldPlantData == null)
            return;
        if (TileUpdater.instance == null || hotbarIndex < 0 || hotbarIndex >= TileUpdater.instance.barslots.Count)
            return;

        int previousSlot = heldIsSeed
            ? (heldFromHotbarSlot >= 0 ? heldFromHotbarSlot : (heldPlantData.seeds.location == Location.hotBar ? heldPlantData.seeds.HotBarSlot : -1))
            : (heldFromHotbarSlot >= 0 ? heldFromHotbarSlot : (heldPlantData.location == Location.hotBar ? heldPlantData.HotBarSlot : -1));

        if (previousSlot >= 0 && previousSlot < TileUpdater.instance.barslots.Count && previousSlot != hotbarIndex)
        {
            TileUpdater.instance.barslots[previousSlot].ClearSlot();
        }
        if (TileUpdater.instance.barslots[hotbarIndex].data != heldPlantData)
        {
            ReleaseHotbarSlotItem(hotbarIndex);
        }

        if (heldIsSeed)
        {
            heldPlantData.seeds.location = Location.hotBar;
            heldPlantData.seeds.HotBarSlot = hotbarIndex;
            TileUpdater.instance.barslots[hotbarIndex].function = Function.Seeds;
            TileUpdater.instance.barslots[hotbarIndex].data = heldPlantData;
            TileUpdater.instance.barslots[hotbarIndex].sprite = heldPlantData.seeds.seedinventorySprite;
        }
        else
        {
            heldPlantData.location = Location.hotBar;
            heldPlantData.HotBarSlot = hotbarIndex;
            TileUpdater.instance.barslots[hotbarIndex].function = Function.Plant;
            TileUpdater.instance.barslots[hotbarIndex].data = heldPlantData;
            TileUpdater.instance.barslots[hotbarIndex].sprite = heldPlantData.inventorySprite;
        }

        ClearHeldState();
        TileUpdater.instance.DrawHotBar();
        InitialiseInv();
    }
    private void ReleaseHotbarSlotItem(int hotbarIndex)
    {
        if (TileUpdater.instance == null || hotbarIndex < 0 || hotbarIndex >= TileUpdater.instance.barslots.Count)
            return;
        var targetSlot = TileUpdater.instance.barslots[hotbarIndex];
        if (targetSlot == null || targetSlot.data == null)
            return;

        if (targetSlot.function == Function.Seeds && targetSlot.data.seeds.HotBarSlot == hotbarIndex)
        {
            targetSlot.data.seeds.location = Location.Inventory;
            targetSlot.data.seeds.HotBarSlot = -1;
        }
        else if (targetSlot.function == Function.Plant && targetSlot.data.HotBarSlot == hotbarIndex)
        {
            targetSlot.data.location = Location.Inventory;
            targetSlot.data.HotBarSlot = -1;
        }
        targetSlot.ClearSlot();

    }
    public void CancelItemHeld()
    {
        if (!isHoldingItem)
            return;
        if (heldFromInvSlot >= 0 && heldFromInvSlot < slotItems.Count && slotItems[heldFromInvSlot] != null)
            slotItems[heldFromInvSlot].Sprite.gameObject.SetActive(true);
        ClearHeldState();
    }
    private void ClearHeldState()
    {
        isHoldingItem = false;
        heldPlantData = null;
        heldIsSeed = false;
        heldSprite = null;
        heldFromInvSlot = -1;
        heldFromHotbarSlot = -1;
        if (dragCurorImage != null)
        {
            dragCurorImage.sprite = null;
            dragCurorImage.gameObject.SetActive(false);
        }
    }
    public void SelectSlot(int index)
    {
        if (containerParent == null || containerParent.childCount == 0)
            return;
        foreach (Transform child in containerParent)
        {
            Image img = child.GetComponent<Image>();
            if (img != null)
                child.GetComponent<Image>().sprite = inactiveContainer;
        }
        index = Mathf.Clamp(index, 0, containerParent.childCount - 1);
        InvSelected = index;

        containerParent.GetChild(index).GetComponent<Image>().sprite = activeContainer;

        if (index >= slotData.Count || !slotData[index].HasValue)
        {
            DetailText.text = "";
            SelectedPlantData = null;
            SelectedSeeds = false;
            return;
        }
        var selected = slotData[index].Value;
        SelectedSeeds = selected.category == Tabs.Seeds;
        SelectedPlantData = selected.data;
        DetailText.text = SelectedPlantData.plantDetails;
    }
    public void SwitchTabs(Tabs tab)
    {
        CancelItemHeld();
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
        if (UIManager.instance.IsOpen(UIPanel.Inventory))
            InitialiseInv();
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