using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Lane Settings")]
    int currentLane = 0;     // -1, 0, 1
    int previousLane = 0;
    [SerializeField] float laneDistance = 2f;
    [SerializeField] private float laneSwitchSpeed = 10f;
    Queue<int> laneQueue = new Queue<int>();
    bool isSwitchingLane = false;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravityForce = -9.81f;
    bool isGrounded;
    private Vector3 velocity;

    [Header("Hit")]
    [SerializeField] private float hitAnimationDuration = 1.2f;

    InputAction moveAction;
    InputAction jumpAction;
    InputAction slideAction;

    CharacterController characterController;

    bool isSliding;
    bool isDead = false;

    public bool IsSwitchingLane => isSwitchingLane;

    private void OnEnable()
    {
        PlayerEvents.OnPlayerHit += HandleHit;
    }

    private void OnDisable()
    {
        PlayerEvents.OnPlayerHit -= HandleHit;
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();

        jumpAction = InputSystem.actions.FindAction("Jump");
        moveAction = InputSystem.actions.FindAction("Move");
        slideAction = InputSystem.actions.FindAction("Slide");
    }

    private void Update()
    {
        if (isDead) return;

        HandleJump();
        HandleMovement();
        HandleLane();
        HandleSlide();
    }

    private void HandleHit()
    {
        if (isDead) return;

        isDead = true;

        velocity = Vector3.zero;
        laneQueue.Clear();
        isSliding = false;

        bool wasSwitchingFromCenter = isSwitchingLane && previousLane == 0;

        StopAllCoroutines();
        isSwitchingLane = false;

        if (wasSwitchingFromCenter)
        {
            StartCoroutine(SnapToMidAndDie());
        }
        else
        {
            StartCoroutine(DieInSameLane());
        }
    }
    private IEnumerator DieInSameLane()
    {
        PlayerEvents.OnPlayerHit?.Invoke();

        yield return new WaitForSecondsRealtime(hitAnimationDuration);

        PlayerEvents.OnPlayerDead?.Invoke();
    }

    private void HandleMovement()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

        if (moveAction.WasPressedThisFrame())
        {
            if (input.x > 0)
                laneQueue.Enqueue(1);
            else if (input.x < 0)
                laneQueue.Enqueue(-1);
        }
    }

    private void HandleLane()
    {
        if (isSwitchingLane || isSliding || laneQueue.Count == 0) return;

        int dir = laneQueue.Dequeue();

        if ((dir == 1 && currentLane < 1) || (dir == -1 && currentLane > -1))
        {
            previousLane = currentLane;

            currentLane += dir;

            StartCoroutine(SmoothLaneSwitch());
            PlayerEvents.OnLaneChanged?.Invoke(dir);
        }
    }

    IEnumerator SmoothLaneSwitch()
    {
        isSwitchingLane = true;

        Vector3 startPos = transform.position;
        float baseX = 0f; // center lane
        Vector3 targetPos = new Vector3(baseX + currentLane * laneDistance, startPos.y, startPos.z);

        float t = 0;

        while (t < 1)
        {
            if (isDead) yield break;

            t += Time.deltaTime * laneSwitchSpeed;
            Vector3 newPos = Vector3.Lerp(startPos, targetPos, t);
            characterController.Move(newPos - transform.position);
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
            isSliding = true;
            PlayerEvents.OnSlideStart?.Invoke();
        }
        else if (slideAction.WasReleasedThisFrame())
        {
            isSliding = false;
            PlayerEvents.OnSlideEnd?.Invoke();
        }
    }

    private IEnumerator SnapToMidAndDie()
    {
        currentLane = 0;

        Vector3 startPos = transform.position;
        Vector3 target = new Vector3(0f, transform.position.y, transform.position.z);

        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * laneSwitchSpeed;
            Vector3 newPos = Vector3.Lerp(startPos, target, t);
            characterController.Move(newPos - transform.position);
            yield return null;
        }

        // AFTER snapping → play animation
        PlayerEvents.OnPlayerHit?.Invoke();

        yield return new WaitForSecondsRealtime(hitAnimationDuration);

        PlayerEvents.OnPlayerDead?.Invoke();
    }
}