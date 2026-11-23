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
    
    void Start()
    {
        targetPosition = transform.position;
    }

    private void Update()
    {
        speed = InputManager.instance.IsSprinting ? basespeed + SprintIncrease : basespeed;

        Vector2 movement = InputManager.instance.Movemenent;

        targetPosition += (Vector3)(movement * speed * Time.deltaTime);

        targetPosition.x = Mathf.Clamp(targetPosition.x, BottomLeft.x, TopRight.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, BottomLeft.y, TopRight.y);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, SmoothTime);
    }
}