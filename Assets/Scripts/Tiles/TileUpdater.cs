using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Categorization;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;
public enum ToolType
{
    Nothing,
    Hoe,
    Can,
    Seeds,
    Sythe,
    UnTill,
    Buy,
    Sell,
}
public class TileUpdater : MonoBehaviour
{
    public static TileUpdater instance;
    public ToolType selectedtool = ToolType.Hoe;
    private Vector3Int lastTilePosition = new Vector3Int(int.MinValue, int.MinValue);
    public PlantData selectedPlant;
    public Transform hotBarContainerParent;
    public Transform hotBaritemParent;
    public Sprite activeSprite;
    public Sprite inActiveSprite;
    public List<HotBarSlot> barslots = new();
    private HotBarSlot activeBarSlot;
    void Start()
    {
        InputManager.instance.SelectHotbarSlot += SelectTool;
        InputManager.instance.MoveToInv += MoveToInv;

        SelectToolType(selectedtool);
        DrawHotBar();
        foreach (Transform child in hotBaritemParent)
        {
            HotBarSlot slot = child.GetComponent<HotBarSlot>();
            barslots.Add(slot);
        }

    }
    public void OnDestroy()
    {
        InputManager.instance.MoveToInv -= MoveToInv;
        InputManager.instance.SelectHotbarSlot -= SelectTool;
    }

    void Awake()
    {
        instance = this;
    }
    void Update()
    {
        Mouse mouse = Mouse.current;

        if (mouse == null || !mouse.leftButton.isPressed)
        {
            lastTilePosition = new Vector3Int(int.MinValue, int.MinValue);
            return;
        }

        Vector3Int currentPosition = GameManager.MousePosition;

        if (currentPosition == lastTilePosition)
            return;

        lastTilePosition = currentPosition;

        switch (selectedtool)
        {
            case ToolType.Hoe:
                TileManager.instance.TillTile(currentPosition);
                break;
            case ToolType.Can:
                TileManager.instance.WaterTile(currentPosition);
                break;
            case ToolType.Seeds:
                TileManager.instance.PlantTile(currentPosition, selectedPlant);
                break;
            case ToolType.Sythe:
                TileManager.instance.HarvestTile(currentPosition);
                break;
            case ToolType.UnTill:
                TileManager.instance.UntillTile(currentPosition);
                break;
        }
    }
    public void MoveToInv()
    {
        if (activeBarSlot.function == Function.Seeds)
        {
            Inventory.instance.MoveSeedToInv(activeBarSlot.data);

        }
        if (activeBarSlot.function == Function.Plant)
        {
            Inventory.instance.MovePlantToInv(activeBarSlot.data);
        }
    }
    private void SelectTool(int invIndex)
    {
        invIndex = invIndex == 0 ? 9 : invIndex - 1;
        if (invIndex < 0 || invIndex >= hotBarContainerParent.childCount || invIndex >= hotBaritemParent.childCount)
            return;

        foreach (Transform child in hotBarContainerParent)
            child.GetComponent<Image>().sprite = inActiveSprite;

        hotBarContainerParent.GetChild(invIndex).GetComponent<Image>().sprite = activeSprite;
        //Lets find the tool
        Transform item = hotBaritemParent.GetChild(invIndex);
        activeBarSlot = item.GetComponent<HotBarSlot>();
        switch (activeBarSlot.function)
        {
            case Function.Hoe:
                SelectToolType(ToolType.Hoe);
                break;
            case Function.Can:
                SelectToolType(ToolType.Can);
                break;
            case Function.Sythe:
                SelectToolType(ToolType.Sythe);
                break;
            case Function.Untill:
                SelectToolType(ToolType.UnTill);
                break;
            case Function.Buy:
                Shop.instance.ToggleShop();
                break;
            case Function.Sell:
                Shop.instance.ToggleShop();
                break;
            case Function.Seeds:
                SelectToolType(ToolType.Seeds);
                selectedPlant = activeBarSlot.data;
                break;
            case Function.Nothing:
                SelectToolType(ToolType.Nothing);
                break;
        }
    }

    public List<int> GetEmptySlots()
    {
        List<int> empties = new();
        for (int i = 0; i < hotBaritemParent.childCount; i++)
        {
            Transform item = hotBaritemParent.GetChild(i);
            HotBarSlot barSlot = item.GetComponent<HotBarSlot>();
            if (barSlot == null)
                Debug.Log(" No Component");
            if (barSlot.function == Function.Nothing)
            {

                empties.Add(i);
            }
            else
            {
                Debug.Log(barSlot.function);
            }
        }

        return empties;
    }
    public void DrawHotBar()
    {
        for (int i = 0; i < hotBaritemParent.childCount; i++)
        {
            Transform child = hotBaritemParent.GetChild(i);
            HotBarSlot barslot = child.GetComponent<HotBarSlot>();
            Image slotImage = child.GetComponent<Image>();

            int count = 0;
            bool showCount = false;

            if (barslot.function == Function.Seeds)
            {
                if (barslot.data == null || Inventory.instance == null)
                {
                    barslot.ClearSlot();
                }
                else
                {
                    count = Inventory.instance.TryGetSeeds(barslot.data);
                    if (count <= 0)
                    {
                        barslot.data.seeds.location = Location.Inventory;
                        barslot.data.seeds.HotBarSlot = -1;
                        barslot.ClearSlot();
                        if (selectedtool == ToolType.Seeds && selectedPlant == barslot.data)
                            SelectToolType(ToolType.Nothing);

                    }
                    else
                    {
                        barslot.data.seeds.HotBarSlot = i;
                        barslot.sprite = barslot.data.seeds.seedinventorySprite;
                        showCount = true;
                    }
                }
            }
            if (barslot.function == Function.Plant)
            {
                if (barslot.data == null || Inventory.instance == null)
                {
                    barslot.ClearSlot();
                }
                else
                {
                    count = Inventory.instance.TryGetPlant(barslot.data);
                    if (count <= 0)
                    {
                        barslot.data.location = Location.Inventory;
                        barslot.data.HotBarSlot = -1;
                        barslot.ClearSlot();
                    }
                    else
                    {
                        barslot.data.HotBarSlot = i;
                        barslot.sprite = barslot.data.inventorySprite;
                        showCount = true;
                    }
                }
            }

            slotImage.sprite = barslot.sprite;
            if (barslot.sprite == null)
            {
                child.GetComponent<Image>().color = Color.clear;
            }
            else
            {
                child.GetComponent<Image>().color = Color.white;
            }

            if (barslot.count != null)
            {
                barslot.count.enabled = showCount;
                if (showCount)
                    barslot.count.text = count.ToString();
            }
        }
    }
    private void SelectToolType(ToolType tool)
    {
        selectedtool = tool;
    }
}