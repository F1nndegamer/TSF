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
    public event Action Close;

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
        input.Player.Close.performed += OnClose;
    }
    void OnDisable()
    {
        input.Player.ToggleExperiment.performed -= OnToggleExperiment;
        input.Player.ToggleInventory.performed -= OnToggleInventory;
        input.Player.ToggleJournal.performed -= OnToggleJournal;
        input.Player.MoveToHotbar.performed -= OnMoveToHotbar;
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
}
