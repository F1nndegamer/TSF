using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class MouseToGrid : MonoBehaviour
{
    private Camera cam;
    private GridLayout grid;
    public Tilemap mouseDisplayMap; // overlay map
    public Tile OverlayTile;
    private Vector3Int lastTile;
    public bool ShowMouse;
    public static MouseToGrid Instance;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        cam = Camera.main;
        grid = FindAnyObjectByType<Grid>();
    }

    void Update()
    {
        //Get current cell

        Vector2 point = new Vector3();
        Vector2 mousePos = Mouse.current.position.ReadValue();

        point = cam.ScreenToWorldPoint(new Vector2(mousePos.x, mousePos.y));
        Vector3Int MouseCell = grid.WorldToCell(point);

        if (EventSystem.current.IsPointerOverGameObject())
        {
            MouseCell = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
        }
        if (MouseCell != lastTile)
        {
            mouseDisplayMap.SetTile(lastTile, null);
            lastTile = MouseCell;
            if (MouseCell.x != int.MinValue)
            {
                mouseDisplayMap.SetTile(MouseCell, OverlayTile);
                GameManager.MousePosition = MouseCell;
            }
        }
    }
    private void OnValidate()
    {
        Cursor.visible = ShowMouse;
    }
    public void ChangeCursor(bool force, bool on = true)
    {
        if (force)
            Cursor.visible = on;
        else
            Cursor.visible = ShowMouse;
    }
}
