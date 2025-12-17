using UnityEngine;
using Unity.Netcode;

public class Move : NetworkBehaviour
{
    private float moveSpeed = 5f;
    private float jumpHeight = 1.5f;
    private float gravity = -9.81f;

    private const float mouseSensitivity = 2f;
    private const float maxLookAngle = 90f;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    private CharacterController characterController;
    private Camera player_camera;

    private float cur_y_speed = 0;
    private float vertical_camera_rotation = 0f;
    private bool is_grounded = false;

    // Input variables
    private Vector3 input_move_coeff = Vector3.zero;
    private Vector2 input_mouse_rotate = Vector2.zero;

    private NetworkVariable<Vector3> net_pos = new NetworkVariable<Vector3>(
        default,
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Owner
    );

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            return;
        }

        characterController = GetComponent<CharacterController>();
        player_camera = GetComponentInChildren<Camera>();

        // Lock cursor to game window, only for local player
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        net_pos.Value = transform.position;
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        ProcessInput();
        HandleGroundCheck();
        HandleMovement();
        ProcessRotation();
        ApplyGravity();

        net_pos.Value = transform.position;
    }

    private void ProcessInput()
    {
        input_move_coeff.x = -Input.GetAxis("Horizontal");
        input_move_coeff.z = Input.GetAxis("Vertical");

        input_mouse_rotate.x = Input.GetAxis("Mouse X") * mouseSensitivity;
        input_mouse_rotate.y = Input.GetAxis("Mouse Y") * mouseSensitivity;
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
        // Calculate movement direction
        Vector3 moveDirection = transform.right * input_move_coeff.z + transform.forward * input_move_coeff.x;

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

        // Vertical camera rotation
        vertical_camera_rotation -= input_mouse_rotate.y;
        vertical_camera_rotation = Mathf.Clamp(vertical_camera_rotation, -maxLookAngle, maxLookAngle);
        Vector3 local_rotation = player_camera.transform.localRotation.eulerAngles;
        player_camera.transform.localRotation = Quaternion.Euler(vertical_camera_rotation, local_rotation.y, local_rotation.z);
    }

    private void ApplyGravity()
    {
        cur_y_speed += gravity * Time.deltaTime;
        characterController.Move(new Vector3(0, cur_y_speed * Time.deltaTime, 0));
    }
}
