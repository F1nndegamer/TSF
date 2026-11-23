using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    private GameInputAction input;

    public event Action ToggleExperiment;
    public event Action ToggleInventory;
    public event Action ToggleJournal;
    public event Action ToggleShop;
    public event Action MoveToHotbar;
    public event Action MoveToInv;
    public event Action Close;
    public event Action<int> SelectHotbarSlot;
    public Vector2 Movemenent => input.Player.Move.ReadValue<Vector2>();
    public bool IsSprinting => input.Player.Sprint.IsPressed();

    void Awake()
    {
        instance = this;
        input = new GameInputAction();
    }
    void OnEnable()
    {
        input.Player.Enable();

        input.Player.ToggleExperiment.performed += OnToggleExperiment;
        input.Player.ToggleInventory.performed += OnToggleInventory;
        input.Player.ToggleJournal.performed += OnToggleJournal;
        input.Player.ToggleShop.performed += OnToggleShop;
        input.Player.MoveToHotbar.performed += OnMoveToHotbar;
        input.Player.MoveToInv.performed += OnMoveToInv;
        input.Player.Close.performed += OnClose;
    }

    void OnDisable()
    {
        input.Player.ToggleExperiment.performed -= OnToggleExperiment;
        input.Player.ToggleInventory.performed -= OnToggleInventory;
        input.Player.ToggleJournal.performed -= OnToggleJournal;
        input.Player.ToggleShop.performed -= OnToggleShop;
        input.Player.MoveToHotbar.performed -= OnMoveToHotbar;
        input.Player.MoveToInv.performed -= OnMoveToInv;
        input.Player.Close.performed -= OnClose;

        input.Player.Disable();
    }

    void OnDestroy()
    {
        input.Dispose();
    }

    private void OnToggleExperiment(InputAction.CallbackContext ctx) => ToggleExperiment?.Invoke();

    private void OnToggleInventory(InputAction.CallbackContext ctx) => ToggleInventory?.Invoke();
    private void OnToggleJournal(InputAction.CallbackContext ctx) => ToggleJournal?.Invoke();
    private void OnMoveToHotbar(InputAction.CallbackContext ctx) => MoveToHotbar?.Invoke();
    private void OnClose(InputAction.CallbackContext ctx) => Close?.Invoke();

    private void OnToggleShop(InputAction.CallbackContext ctx) => ToggleShop?.Invoke();

    private void OnSlot1(InputAction.CallbackContext ctx) => SelectHotbarSlot?.Invoke(1);
    private void OnSlot2(InputAction.CallbackContext ctx) => SelectHotbarSlot?.Invoke(2);
    private void OnSlot3(InputAction.CallbackContext ctx) => SelectHotbarSlot?.Invoke(3);
    private void OnSlot4(InputAction.CallbackContext ctx) => SelectHotbarSlot?.Invoke(4);
    private void OnSlot5(InputAction.CallbackContext ctx) => SelectHotbarSlot?.Invoke(5);
    private void OnSlot6(InputAction.CallbackContext ctx) => SelectHotbarSlot?.Invoke(6);
    private void OnSlot7(InputAction.CallbackContext ctx) => SelectHotbarSlot?.Invoke(7);
    private void OnSlot8(InputAction.CallbackContext ctx) => SelectHotbarSlot?.Invoke(8);
    private void OnSlot9(InputAction.CallbackContext ctx) => SelectHotbarSlot?.Invoke(9);
    private void OnSlot0(InputAction.CallbackContext ctx) => SelectHotbarSlot?.Invoke(0);
    private void OnMoveToInv(InputAction.CallbackContext ctx) => MoveToInv?.Invoke();
}
