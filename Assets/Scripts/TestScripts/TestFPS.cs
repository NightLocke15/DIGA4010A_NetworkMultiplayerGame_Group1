using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System.Collections;
[RequireComponent(typeof(CharacterController))]
public class FPSPlayer : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float lookSpeed = 2f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;
    public float boostedSpeed = 100f;
    private float currentSpeed;
    [SyncVar] // Show current speed to all players
    public bool isSpeedBoosted = false;
    [Header("References")]
    public Transform playerCamera;
    [Header("Private Stuff")]
    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float verticalVelocity;
    private float cameraPitch = 0f;
    [Header("Shooting Stuff")]
    public Transform lazerTransform;
    public TrailRenderer lazerBeam;
    void Start()
    {
        currentSpeed = moveSpeed;
        controller = GetComponent<CharacterController>();
        if (!isLocalPlayer)
        {
            playerCamera.gameObject.SetActive(false);
        }

        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {
        if (!isLocalPlayer) return;
        Debug.Log(currentSpeed);
        HandleMovement();
        HandleLook();
    }
    private void HandleMovement()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward *
        moveInput.y;
        move *= moveSpeed;
        // Apply gravity
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }
        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;
        controller.Move(move * Time.deltaTime);
    }
    private void HandleLook()
    {
        float mouseX = lookInput.x * lookSpeed * Time.deltaTime;
        float mouseY = lookInput.y * lookSpeed * Time.deltaTime;
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f);
        playerCamera.localRotation = Quaternion.Euler(cameraPitch, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
    }
    public void HandleShoot()
    {
        if (!isLocalPlayer) return;
        Ray ray = new Ray(lazerTransform.position, lazerTransform.forward);
        // Instantiate the visual beam
        TrailRenderer beam = Instantiate(lazerBeam, lazerTransform.position,
        Quaternion.identity);
        beam.AddPosition(lazerTransform.position);
        if (Physics.Raycast(ray, out RaycastHit hit, 50f))
        {
            beam.transform.position = hit.point;
            // Try to damage a player if hit
            var playerStats = hit.collider.gameObject.GetComponent<PlayerStats>();
            if (playerStats)
            {
                playerStats.Damage(20); // Example damage amount
            }
        }
        else
        {
            beam.transform.position = lazerTransform.position +
            lazerTransform.forward * 50f;
        }
    }
    [Server]
    public void ApplySpeedBoost()
    {
        StartCoroutine(SpeedBoostRoutine());
    }
    IEnumerator SpeedBoostRoutine()
    {
        isSpeedBoosted = true;
        currentSpeed = boostedSpeed;
        yield return new WaitForSeconds(3f);
        currentSpeed = moveSpeed;
        isSpeedBoosted = false;
    }
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }
    public void OnAttack(InputValue value)
    {
        if (value.isPressed)
        {
            HandleShoot();
        }
    }
}
