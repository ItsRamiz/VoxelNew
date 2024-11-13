using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController characterController;
    public Transform cameraTransform;

    public float speed = 10.0f;
    public float gravity = -15.81f;
    public float jumpHeight = 3.0f;
    public float highJumpMultiplier = 1.5f; // Multiplier for high jump

    public float mouseSensitivity = 300.0f;

    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float xRotation = 0f;

    private Transform playerTransform;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerTransform = transform;
    }

    void Update()
    {
        PlayerMove();
        HandleCameraRotation();
    }

    public Vector3 getPlayerPosition()
    {
        return playerTransform.position;
    }

    void PlayerMove()
    {
        groundedPlayer = characterController.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
        move.y = 0;

        characterController.Move(move * Time.deltaTime * speed);

        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            // Check if the player is holding the space bar for a high jump
            float jumpPower = jumpHeight;
            if (Input.GetKey(KeyCode.Space))
            {
                jumpPower *= highJumpMultiplier;
            }

            playerVelocity.y += Mathf.Sqrt(jumpPower * -2.0f * gravity);
        }

        playerVelocity.y += gravity * Time.deltaTime;
        characterController.Move(playerVelocity * Time.deltaTime);
    }

    void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        playerTransform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
