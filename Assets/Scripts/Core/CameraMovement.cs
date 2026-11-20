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
        if (Keyboard.current.shiftKey.isPressed || Keyboard.current.leftCtrlKey.isPressed)
        {
            speed = basespeed + SprintIncrease;
        }
        else
        {
            speed = basespeed;
        }
        Vector3 movement = Vector3.zero;
        
        //TODO: I am using hardCoded Inputs, later Ill use actions for keybinds
        if (Keyboard.current.wKey.isPressed) movement.y += 1;
        if (Keyboard.current.sKey.isPressed) movement.y -= 1;
        if (Keyboard.current.aKey.isPressed) movement.x -= 1;
        if (Keyboard.current.dKey.isPressed) movement.x += 1;

        targetPosition += movement * speed * Time.deltaTime;
        if (targetPosition.x >= TopRight.x) targetPosition.x = TopRight.x;
        if (targetPosition.y >= TopRight.y) targetPosition.y = TopRight.y;
        if (targetPosition.x <= BottomLeft.x) targetPosition.x = BottomLeft.x;
        if (targetPosition.y <= BottomLeft.y) targetPosition.y = BottomLeft.y;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, SmoothTime);
    }
}
