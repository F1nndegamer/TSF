using System;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEditor.Toolbars;
using UnityEngine;
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
    public PlantData[] startPlants;
    public Transform hotBarContainerParent;
    public Transform hotBaritemParent;
    public Sprite activeSprite;
    public Sprite inActiveSprite;
    public List<HotBarSlot> barslots = new();
    private HotBarSlot activeBarSlot;
    void Start()
    {
        SelectToolType(selectedtool);
        DrawHotBar();
        foreach (Transform child in hotBaritemParent)
        {
            HotBarSlot slot = child.GetComponent<HotBarSlot>();
            barslots.Add(slot);
        }
    }
    void Awake()
    {
        instance = this;
    }
    void Update()
    {
        HandleToolSelection();
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
    private void HandleToolSelection()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null)
            return;
        if (kb.digit1Key.wasPressedThisFrame)
            SelectTool(1);
        if (kb.digit2Key.wasPressedThisFrame)
            SelectTool(2);
        if (kb.digit3Key.wasPressedThisFrame)
            SelectTool(3);
        if (kb.digit4Key.wasPressedThisFrame)
            SelectTool(4);
        if (kb.digit5Key.wasPressedThisFrame)
            SelectTool(5);
        if (kb.digit6Key.wasPressedThisFrame)
            SelectTool(6);
        if (kb.digit7Key.wasPressedThisFrame)
            SelectTool(7);
        if (kb.digit8Key.wasPressedThisFrame)
            SelectTool(8);
        if (kb.digit9Key.wasPressedThisFrame)
            SelectTool(9);
        if (kb.digit0Key.wasPressedThisFrame)
            SelectTool(0);
        if (kb.fKey.wasPressedThisFrame)
        {
            Debug.Log("HB F key");
            if (GameManager.IsInventoryOpen)
            {
                Debug.Log("InvOpen");
                return;
            }
            if (activeBarSlot.function == Function.Seeds)
            {
                Debug.Log("IsSeeds");
                Inventory.instance.MoveSeedToInv(activeBarSlot.data);

            }
            if (activeBarSlot.function == Function.Plant)
            {
                Debug.Log("IsPlant");
                Inventory.instance.MovePlantToInv(activeBarSlot.data);
            }
        }
    }
    private void SelectTool(int invIndex)
    {
        invIndex -= 1;
        if (invIndex == -1)
        {
            invIndex = 9;
        }
        foreach (Transform child in hotBarContainerParent)
        {
            child.GetComponent<Image>().sprite = inActiveSprite;
        }
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
            int hotbarIndex = -1;
            hotbarIndex = i - 1;
            if (hotbarIndex == -1)
            {
                hotbarIndex = 9; //pos 10
            }
            Transform child = hotBaritemParent.GetChild(i);
            HotBarSlot barslot = child.GetComponent<HotBarSlot>();

            child.GetComponent<Image>().sprite = barslot.sprite;
            if (barslot.sprite == null)
            {
                child.GetComponent<Image>().color = Color.clear;
            }
            else
            {
                child.GetComponent<Image>().color = Color.white;
            }
            if (barslot.function == Function.Plant)
            {
                barslot.data.HotBarSlot = hotbarIndex;
                barslot.sprite = barslot.data.inventorySprite;
            }
            if (barslot.function == Function.Seeds)
            {
                barslot.data.seeds.HotBarSlot = hotbarIndex;
                barslot.sprite = barslot.data.seeds.seedinventorySprite;
            }
        }
    }
    private void SelectToolType(ToolType tool)
    {
        selectedtool = tool;
    }
}