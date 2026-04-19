using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Lane Settings")]
    int currentLane = 0;     // -1, 0, 1
    [SerializeField] float laneDistance = 2f;
    [SerializeField] private float laneSwitchSpeed = 10f;
    Queue<int> laneQueue = new Queue<int>();
    bool isSwitchingLane = false;


    [Header("Jump Function")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravityForce = -9.81f;
    bool isGrounded;
    private Vector3 velocity;

    InputAction moveAction;
    InputAction jumpAction;
    InputAction slideAction;
    CharacterController characterController;
    bool isSliding;
    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        jumpAction = InputSystem.actions.FindAction("Jump");
        moveAction = InputSystem.actions.FindAction("Move");
        slideAction = InputSystem.actions.FindAction("Slide");
    }

    private void Update()
    {
        HandleJump();
        HandleMovement();
        HandleLane();
        HandleSlide();
    }

    private void HandleMovement()
    {

        Vector2 input = moveAction.ReadValue<Vector2>();
        if (moveAction.WasPressedThisFrame())
        {
            if (input.x > 0)
            {
                laneQueue.Enqueue(1);
            }
            else if (input.x < 0)
            {
                laneQueue.Enqueue(-1);
            }
        }
    }
    private void HandleLane()
    {

        if (isSwitchingLane) return;
        if (laneQueue.Count == 0) return;

        int dir = laneQueue.Dequeue();

        if ((dir == 1 && currentLane < 1) || (dir == -1 && currentLane > -1))
        {
            currentLane += dir;
            StartCoroutine(SmoothLaneSwitch());
            PlayerEvents.OnLaneChanged?.Invoke(dir);
        }
    }
    IEnumerator SmoothLaneSwitch()
    {
        isSwitchingLane = true;

        Vector3 startPos = transform.position;
        Vector3 targetPos = new Vector3(currentLane * laneDistance, startPos.y, startPos.z);

        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * laneSwitchSpeed;

            Vector3 newPos = Vector3.Lerp(startPos, targetPos, t);
            Vector3 move = newPos - transform.position;

            characterController.Move(move);

            yield return null;
        }

        isSwitchingLane = false;
    }

    private void HandleJump()
    {
        isGrounded = characterController.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            PlayerEvents.onGroundChanged?.Invoke(true);

        }
        if (jumpAction.WasPressedThisFrame() && isGrounded && !isSliding)
        {
            velocity.y = jumpForce;
            PlayerEvents.onJump?.Invoke();
            PlayerEvents.onGroundChanged?.Invoke(false);
            transform.rotation = quaternion.identity;
        }
        velocity.y += gravityForce * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
    private void HandleSlide()
    {
        if (slideAction.WasPressedThisFrame() && isGrounded)
        {
            PlayerEvents.OnSlideStart?.Invoke();
        }
        else if (slideAction.WasReleasedThisFrame())
        {
            PlayerEvents.OnSlideEnd?.Invoke();
        }
    }
}