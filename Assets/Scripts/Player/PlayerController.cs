using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float sprintSpeed = 6f;
    [SerializeField] private float crouchSpeed = 1.5f;

    [Header("Settings")]
    [SerializeField] private float rotationSmoothTime = 0.1f;
    [SerializeField] private float gravity = -9.81f;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    private CharacterController characterController;
    private IInputService inputService;

    private float currentSpeed;
    private float rotationVelocity;
    private float verticalVelocity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        // Using DesktopInputService by default. In a real scenario, this could be injected via DI.
        inputService = new DesktopInputService();
    }

    private void Update()
    {
        HandleMovement();
        ApplyGravity();
    }

    private void HandleMovement()
    {
        Vector2 input = inputService.GetMovementInput();
        Vector3 direction = new Vector3(input.x, 0f, input.y).normalized;

        // Determine current speed
        currentSpeed = walkSpeed;
        if (inputService.IsSprinting())
        {
            currentSpeed = sprintSpeed;
        }
        else if (inputService.IsCrouching())
        {
            currentSpeed = crouchSpeed;
        }

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            if (cameraTransform != null)
            {
                targetAngle += cameraTransform.eulerAngles.y;
            }

            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            characterController.Move(moveDir.normalized * (currentSpeed * Time.deltaTime));
        }
        else
        {
            // Even if not moving horizontally, we still need to apply gravity next
            characterController.Move(Vector3.zero);
        }
    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // Small constant to ensure grounded state
        }

        verticalVelocity += gravity * Time.deltaTime;
        characterController.Move(new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime);
    }

    public void SetInputService(IInputService newService)
    {
        inputService = newService;
    }
}
