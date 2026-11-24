using System;
using JetBrains.Annotations;
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
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    void Awake()
    {
        instance = this;
        input = new GameInputAction();

        string savedBindings = PlayerPrefs.GetString("InputBindings", "");

        if (!string.IsNullOrEmpty(savedBindings))
            input.asset.LoadBindingOverridesFromJson(savedBindings);
    }
    public void RebindAction(string actionPath, Action<string> onComplete = null)
    {
        InputAction action = input.asset.FindAction(actionPath);

        if (action == null)
        {
            Debug.LogError("Not found :(");
            return;
        }

        rebindingOperation?.Cancel();
        action.Disable();

        rebindingOperation = action.PerformInteractiveRebinding()
        .WithCancelingThrough("<Keyboard>/escape")
        .OnComplete(operation =>
        {
            action.Enable();

            string binding = action.GetBindingDisplayString();

            input.asset.SaveBindingOverridesAsJson();

            PlayerPrefs.SetString(
                "InputBindings",
                input.asset.SaveBindingOverridesAsJson()
            );

            PlayerPrefs.Save();

            operation.Dispose();
            rebindingOperation = null;

            onComplete?.Invoke(binding);
        })
        .OnCancel(operation =>
        {
            action.Enable();
            operation.Dispose();
            rebindingOperation = null;
        });
        rebindingOperation.Start();
    }
    
    public string GetBinding(string actionPath)
    {
        return input.asset.FindAction(actionPath)?.GetBindingDisplayString();
    }
    public void ResetBinding()
    {
        rebindingOperation?.Cancel();
        input.asset.RemoveAllBindingOverrides();
        PlayerPrefs.DeleteKey("InputBindings");
        PlayerPrefs.Save();
    }

    void OnEnable()
    {
        input.Player.Enable();
        input.Hotbar.Enable();

        input.Player.ToggleExperiment.performed += OnToggleExperiment;
        input.Player.ToggleInventory.performed += OnToggleInventory;
        input.Player.ToggleJournal.performed += OnToggleJournal;
        input.Player.MoveToHotbar.performed += OnMoveToHotbar;
        input.Player.ToggleShop.performed += OnToggleShop;
        input.Player.MoveToInv.performed += OnMoveToInv;
        input.Player.Close.performed += OnClose;

        input.Hotbar.Slot1.performed += OnSlot1;
        input.Hotbar.Slot2.performed += OnSlot2;
        input.Hotbar.Slot3.performed += OnSlot3;
        input.Hotbar.Slot4.performed += OnSlot4;
        input.Hotbar.Slot5.performed += OnSlot5;
        input.Hotbar.Slot6.performed += OnSlot6;
        input.Hotbar.Slot7.performed += OnSlot7;
        input.Hotbar.Slot8.performed += OnSlot8;
        input.Hotbar.Slot9.performed += OnSlot9;
        input.Hotbar.Slot0.performed += OnSlot0;
    }
    void OnDisable()
    {
        input.Hotbar.Slot1.performed -= OnSlot1;
        input.Hotbar.Slot2.performed -= OnSlot2;
        input.Hotbar.Slot3.performed -= OnSlot3;
        input.Hotbar.Slot4.performed -= OnSlot4;
        input.Hotbar.Slot5.performed -= OnSlot5;
        input.Hotbar.Slot6.performed -= OnSlot6;
        input.Hotbar.Slot7.performed -= OnSlot7;
        input.Hotbar.Slot8.performed -= OnSlot8;
        input.Hotbar.Slot9.performed -= OnSlot9;
        input.Hotbar.Slot0.performed -= OnSlot0;

        input.Player.Close.performed -= OnClose;
        input.Player.MoveToInv.performed -= OnMoveToInv;
        input.Player.ToggleShop.performed -= OnToggleShop;
        input.Player.MoveToHotbar.performed -= OnMoveToHotbar;
        input.Player.ToggleJournal.performed -= OnToggleJournal;
        input.Player.ToggleInventory.performed -= OnToggleInventory;
        input.Player.ToggleExperiment.performed -= OnToggleExperiment;


        input.Player.Disable();
        input.Hotbar.Disable();
        rebindingOperation?.Cancel();
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