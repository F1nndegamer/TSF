using System;
using UnityEditor.Tilemaps;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
public enum ToolType
{
    Nothing,
    Hoe,
    Can,
    Seeds,
    Sythe,
    UnTill,
}
public class TileUpdater : MonoBehaviour
{
    public static TileUpdater instance;
    public ToolType selectedtool = ToolType.Hoe;
    private Vector3Int lastTilePosition = new Vector3Int(int.MinValue, int.MinValue);
    public PlantData SelectedPlant; //TODO: This is just 1 plant for now
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
                TileManager.instance.PlantTile(currentPosition, SelectedPlant);
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
            SelectTool(ToolType.Hoe);
        if (kb.digit2Key.wasPressedThisFrame)
            SelectTool(ToolType.Can);
        if (kb.digit3Key.wasPressedThisFrame)
            SelectTool(ToolType.Seeds);
        if (kb.digit4Key.wasPressedThisFrame)
            SelectTool(ToolType.Sythe);
        if(kb.digit5Key.wasPressedThisFrame)
            SelectTool(ToolType.UnTill);
        if(kb.digit8Key.wasPressedThisFrame)
            SelectTool(ToolType.Nothing);

    }
    private void SelectTool(ToolType tool)
    {
        selectedtool = tool;
    }
}
