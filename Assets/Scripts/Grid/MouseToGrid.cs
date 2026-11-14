using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

public class MouseToGrid : MonoBehaviour
{
    private Camera cam;
    private GridLayout grid;
    public Tilemap mouseDisplayMap; // overlay map
    public Tile OverlayTile;
    private Vector3Int lastTile;
    void Start()
    {
        Cursor.visible = false;
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
        if(MouseCell != lastTile)
        {
            mouseDisplayMap.SetTile(lastTile, null);
            lastTile = MouseCell;
            mouseDisplayMap.SetTile(MouseCell, OverlayTile);
        }
    }
}
