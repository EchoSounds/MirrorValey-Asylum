using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Performance.ProfileAnalyzer;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    //Component references
    private Rigidbody rb;
    private PlayerInputActions playerInputActions;

    //GameObject references
    private GameObject playerCamera; 

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float airMultiplier = 0.4f;
    float movementMultiplier = 10f;

    [Header("Sprinting")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float sprintSpeed = 6f;

    [SerializeField] private float acceleration = 10f;

    [Header("Stamina")]
    [SerializeField] private GameObject staminaBar;
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaRegen = 2f;
    [SerializeField] private float sprintStamDecrease = 5f;

    private Slider staminaSlider;
    private Animator staminaAnimator;
    float desiredMoveSpeed;
    float currStamina;
    private bool isRunning;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 5f;

    [Header("Drag")]
    [SerializeField] private float groundDrag = 6f;
    [SerializeField] private float airDrag = 2f;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    float groundDistance = 0.1f;
    bool isGrounded;

    private void Awake()
    {
        //Hide Cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Get Components
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        //Get GameObjects
        playerCamera = FindObjectOfType<Camera>().gameObject;

        //Get Input actions
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        //Subscribe to actions
        playerInputActions.Player.Jump.performed += OnJump;

        //Set Values
        desiredMoveSpeed = walkSpeed;

        //Stamina Bar
        currStamina = maxStamina;
        staminaSlider = staminaBar.GetComponent<Slider>();
        staminaAnimator = staminaBar.GetComponent<Animator>();
        staminaSlider.maxValue = maxStamina;
    }

    private void FixedUpdate()
    {
        MovePlayer(playerInputActions.Player.Movement.ReadValue<Vector2>());
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        ControlDrag();
        SpeedController();
        ManageStamina();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        { 
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.y);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }


    private void MovePlayer(Vector2 moveInput)
    {
        Quaternion targetRotation = playerCamera.transform.rotation;

        // Set the current object's y rotation to match the target object's y rotation
        Vector3 eulerAngles = transform.rotation.eulerAngles;
        eulerAngles.y = targetRotation.eulerAngles.y;
        transform.rotation = Quaternion.Euler(eulerAngles);

        Vector3 moveDir = transform.forward * moveInput.y + transform.right * moveInput.x;

        if (isGrounded)
        {
            rb.AddForce(moveDir.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        } else if(!isGrounded)
        {
            rb.AddForce(moveDir.normalized * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
        }
    }

    void ControlDrag()
    {
        if (isGrounded) { rb.drag = groundDrag; }
        else { rb.drag = airDrag; }
    }

    void SpeedController()
    {
        if (playerInputActions.Player.Sprint.WasPerformedThisFrame()) { desiredMoveSpeed = sprintSpeed; isRunning = true; staminaAnimator.SetBool("isRunning", true); }
        else if (playerInputActions.Player.Sprint.WasReleasedThisFrame()) { desiredMoveSpeed = walkSpeed; isRunning = false; }

        if(currStamina <= 0) { desiredMoveSpeed = walkSpeed; isRunning = false; }

        moveSpeed = Mathf.Lerp(moveSpeed, desiredMoveSpeed, acceleration * Time.deltaTime);
    }

    void ManageStamina()
    {
        staminaSlider.value = currStamina;

        if(isRunning)
        {
            currStamina -= sprintStamDecrease * Time.deltaTime;
        } else if(currStamina < maxStamina)
        {
            currStamina += staminaRegen * Time.deltaTime;
        }
        if(currStamina > maxStamina) { staminaAnimator.SetBool("isRunning", false); }
    }
}
