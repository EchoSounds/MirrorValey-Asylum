using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

namespace ronan.player
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed;
        public float walkSpeed = 7;
        public float sprintSpeed = 10;
        public float slideSpeed = 30f;
        public float slopeIncreaseMultiplier = 1.5f;
        public float speedIncreaseMultiplier = 2.5f;
        public float climbSpeed = 3f;

        public float groundDrag = 5;

        private float desiredMoveSpeed;
        private float lastDesiredMoveSpeed;

        [Header("Jumping")]
        public float jumpForce = 9;
        public float jumpCooldown = 0.25f;
        public float airMultiplier = 0.4f;
        bool readyToJump = true;

        [Header("Crouching")]
        public float crouchSpeed = 3.5f;
        public float crouchYScale = 0.5f;
        private float startYScale;

        [Header("Ground Check")]
        public float playerHeight = 2f;
        public LayerMask whatIsGround;
        public bool grounded;
        public bool onSlide;



        public Transform camOrientation;
        public Transform playerOrientation;
        public bool sliding;
        public bool climbing;

        float horizontalInput;
        float verticalInput;
        Vector2 movementInput;

        Vector3 moveDirection;

        Rigidbody rb;
        public PlayerInputs playerInputs;

        public MovementState state;
        public enum MovementState
        {
            walking,
            sprinting,
            crouching,
            air
        }

        private void Awake()
        {
            playerInputs = new PlayerInputs();
        }
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;

            readyToJump = true;
            moveSpeed = walkSpeed;

            startYScale = transform.localScale.y;
        }

        private void OnEnable()
        {
            playerInputs.Enable();
        }
        private void OnDisable()
        {
            playerInputs.Disable();
        }

        private void Update()
        {
            grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

            MyInput();
            SpeedControl();
            StateHandler();

            if (grounded)
            {
                rb.drag = groundDrag;
            }
            else
            {
                rb.drag = 0;
            }

        }

        private void FixedUpdate()
        {
            MovePlayer();
        }

        private void MyInput()
        {
            Vector2 movIn = playerInputs.Player.Movement.ReadValue<Vector2>();
            movementInput = movIn;
            horizontalInput = movementInput.x;
            verticalInput = movementInput.y;

            if (playerInputs.Player.Jump.WasPressedThisFrame() && readyToJump && grounded)
            {

                readyToJump = false;

                Jump();

                Invoke(nameof(ResetJump), jumpCooldown);
            }

            if (playerInputs.Player.Crouch.WasPressedThisFrame())
            {
                transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
                rb.AddForce(Vector3.down * 10f, ForceMode.Impulse);
            }
            else if (playerInputs.Player.Crouch.WasReleasedThisFrame())
            {
                transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            }

        }

        private void StateHandler()
        {
            if (playerInputs.Player.Crouch.inProgress)
            {
                state = MovementState.crouching;
                desiredMoveSpeed = crouchSpeed;
            }
            else if (grounded && playerInputs.Player.Sprint.inProgress)
            {
                state = MovementState.sprinting;
                desiredMoveSpeed = sprintSpeed;
            }
            else if (grounded)
            {
                state = MovementState.walking;
                desiredMoveSpeed = walkSpeed;
            }
            else
            {
                state = MovementState.air;
            }

            if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f && moveSpeed != 0)
            {
                StopAllCoroutines();
                StartCoroutine("SmoothlyLerpMoveSpeed");
            }
            else
            {
                moveSpeed = desiredMoveSpeed;
            }
            lastDesiredMoveSpeed = desiredMoveSpeed;
        }

        private IEnumerator SmoothlyLerpMoveSpeed()
        {
            // smoothly lerp movementSpeed to desired value
            float time = 0;
            float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
            float startValue = moveSpeed;

            while (time < difference)
            {
                moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);
                time += Time.deltaTime * speedIncreaseMultiplier;
                yield return null;
            }

            moveSpeed = desiredMoveSpeed;
        }


        private void MovePlayer()
        {
            moveDirection = camOrientation.forward * verticalInput + camOrientation.right * horizontalInput;

            if (grounded)
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

            }
            else if (!grounded)
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
            }

        }

        public void SpeedControl()
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }

        private void Jump()
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            Debug.Log("Jump");
        }

        private void ResetJump()
        {
            readyToJump = true;
        }
    }
}