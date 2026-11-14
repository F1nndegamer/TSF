using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
public enum Actions
{
    Nothing,
    Till,
    Water,
    IDK,
}
public class TileUpdater : MonoBehaviour
{
    public Actions action = Actions.Till;
    void Update()
    {
        Mouse mouse = Mouse.current;
        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            if (action == Actions.Till)
            {
                //SetGroundTile also initalises the tile so lets do that
                TileManager.instance.SetGroundTile(GameManager.MousePosition, GroundType.Tilled);
            }
        }
    }
}
