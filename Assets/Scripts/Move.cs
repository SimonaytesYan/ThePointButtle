using UnityEngine;
using Unity.Netcode;

public class Move : NetworkBehaviour
{
    private float moveSpeed = 7.5f;
    private float jumpHeight = 1.5f;
    private float gravity = -9.81f;

    private const float mouseSensitivity = 0.8f;
    private const float maxLookAngle = 90f;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    [SerializeField] private Transform cameraPitchPivot;

    [SerializeField] private Animator animator;

    private CharacterController characterController;

    private float cur_y_speed = 0f;
    private float vertical_camera_rotation = 0f;
    private bool is_grounded = false;

    // Input variables
    private Vector3 input_move_coeff = Vector3.zero;
    private Vector2 input_mouse_rotate = Vector2.zero;

    public override void OnNetworkSpawn()
    {
        Camera playerCamera = GetComponentInChildren<Camera>();
        AudioListener listener = GetComponentInChildren<AudioListener>();

        if (playerCamera != null)
        {
            playerCamera.enabled = IsOwner;
        }
        if (listener != null)
        {
            listener.enabled = IsOwner;
        }

        if (IsOwner)
        {
            // Lock cursor to game window
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Start()
    {
        if (!IsOwner)
        {
            return;
        }

        characterController = GetComponent<CharacterController>();

        if (cameraPitchPivot == null)
        {
            Transform found = transform.Find("CameraPivot");
            if (found != null) cameraPitchPivot = found;
        }

        if (animator == null) { 
            animator = GetComponentInChildren<Animator>();
        }

    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        ProcessInput();
        UpdateAnimatorParams();
        HandleGroundCheck();
        HandleMovement();
        ProcessRotation();
        ApplyGravity();
    }

    private void ProcessInput()
    {
        input_move_coeff.x = Input.GetAxis("Horizontal"); 
        input_move_coeff.z = Input.GetAxis("Vertical");  

        input_mouse_rotate.x = Input.GetAxis("Mouse X") * mouseSensitivity;
        input_mouse_rotate.y = Input.GetAxis("Mouse Y") * mouseSensitivity;
    }

    private void UpdateAnimatorParams()
    {
        if (animator == null) { 
            return;
        }

        animator.SetFloat("MoveX", input_move_coeff.x);
        animator.SetFloat("MoveY", input_move_coeff.z);
    }


    private void HandleGroundCheck()
    {
        is_grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Reset cur_y_speed when grounded
        if (is_grounded && cur_y_speed < 0)
        {
            cur_y_speed = -2f;
        }
    }

    private void HandleMovement()
    {
        Vector3 moveDirection =
            transform.forward * input_move_coeff.z +
            transform.right   * input_move_coeff.x;

        // Normalize to prevent faster diagonal movement
        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }

        // Apply movement
        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);

        // Handle jumping
        if (Input.GetButtonDown("Jump") && is_grounded)
        {
            cur_y_speed = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void ProcessRotation()
    {
       
        transform.Rotate(Vector3.up * input_mouse_rotate.x);

        vertical_camera_rotation -= input_mouse_rotate.y;
        vertical_camera_rotation = Mathf.Clamp(vertical_camera_rotation, -maxLookAngle, maxLookAngle);

        if (cameraPitchPivot != null)
        {
            cameraPitchPivot.localRotation = Quaternion.Euler(vertical_camera_rotation, 0f, 0f);
        }
    }

    private void ApplyGravity()
    {
        cur_y_speed += gravity * Time.deltaTime;
        characterController.Move(new Vector3(0, cur_y_speed * Time.deltaTime, 0));
    }
}
