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

    private float lastNoiseTime;
    private float noiseInterval = 0.5f;

    public bool IsCrouching { get; private set; }
    public bool IsSprinting { get; private set; }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        // Late init of input to allow mobile setup if needed, handled by external injector or basic detection here
        if (inputService == null)
        {
            if (YandexGamesManager.Instance != null && YandexGamesManager.Instance.IsMobile())
            {
                // Rely on a global DI or find the active mobile input service
                var inputProvider = UnityEngine.Object.FindAnyObjectByType<MobileInputProvider>();
                if (inputProvider != null) inputService = inputProvider.GetInputService();
            }

            // Fallback
            if (inputService == null)
            {
                inputService = new DesktopInputService();
            }
        }
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
        IsSprinting = inputService.IsSprinting();
        IsCrouching = inputService.IsCrouching();

        if (IsSprinting)
        {
            currentSpeed = sprintSpeed;
        }
        else if (IsCrouching)
        {
            currentSpeed = crouchSpeed;
        }

        // Broadcast noise if sprinting (rate limited to avoid spamming path recalculations)
        if (IsSprinting && direction.magnitude >= 0.1f)
        {
            if (Time.time - lastNoiseTime >= noiseInterval)
            {
                NoiseManager.MakeNoise(transform.position, 10f); // 10m noise radius
                lastNoiseTime = Time.time;
            }
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
