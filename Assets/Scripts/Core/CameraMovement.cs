using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CameraMovement : MonoBehaviour
{
    public float basespeed = 5f;
    public float SprintIncrease = 2f;
    private float speed;
    public float SmoothTime = 0.01f;
    public Vector2 TopRight;
    public Vector2 BottomLeft;
    private Vector3 velocity = Vector3.zero;
    private Vector3 targetPosition;
    private GameInputAction input;
    void Awake()
    {
        input = new GameInputAction();
    }
    void OnEnable()
    {
        input.Player.Enable();
    }
    void OnDisable()
    {
        input.Player.Disable();
    }
    void OnDestroy()
    {
        input.Dispose();
    }
    void Start()
    {
        targetPosition = transform.position;
    }

    private void Update()
    {
        speed = input.Player.Sprint.IsPressed() ? basespeed + SprintIncrease : basespeed;

        Vector2 movement = input.Player.Move.ReadValue<Vector2>();

        targetPosition += (Vector3)(movement * speed * Time.deltaTime);

        targetPosition.x = Mathf.Clamp(targetPosition.x, BottomLeft.x, TopRight.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, BottomLeft.y, TopRight.y);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, SmoothTime);
    }
}
