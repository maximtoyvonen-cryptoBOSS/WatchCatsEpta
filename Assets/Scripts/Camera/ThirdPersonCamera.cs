using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target & Positioning")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 1.5f, 0f);
    [SerializeField] private float distance = 5f;

    [Header("Rotation Settings")]
    [SerializeField] private float sensitivityX = 2f;
    [SerializeField] private float sensitivityY = 2f;
    [SerializeField] private float minYAngle = -30f;
    [SerializeField] private float maxYAngle = 60f;

    [Header("Smoothing")]
    [SerializeField] private float smoothTime = 0.1f;

    private IInputService inputService;
    private float currentX = 0f;
    private float currentY = 0f;
    private Vector3 currentVelocity;

    private void Awake()
    {
        // Initialize angles based on current rotation if needed, or start at 0
        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;
    }

    private void Start()
    {
        if (inputService == null)
        {
            if (YandexGamesManager.Instance != null && YandexGamesManager.Instance.IsMobile())
            {
                inputService = UnityEngine.Object.FindObjectOfType<MobileInputService>();
            }

            if (inputService == null)
            {
                inputService = new DesktopInputService();
            }
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        HandleInput();
        UpdateCameraPositionAndRotation();
    }

    private void HandleInput()
    {
        Vector2 lookInput = inputService.GetLookInput();
        currentX += lookInput.x * sensitivityX;
        currentY -= lookInput.y * sensitivityY;

        currentY = Mathf.Clamp(currentY, minYAngle, maxYAngle);
    }

    private void UpdateCameraPositionAndRotation()
    {
        // Calculate rotation
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0f);

        // Calculate position based on target + offset, rotated back by 'distance'
        Vector3 targetPosition = target.position + offset;
        Vector3 direction = new Vector3(0, 0, -distance);
        Vector3 desiredPosition = targetPosition + rotation * direction;

        // Smoothly move the camera
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothTime);

        // Look at the target (adjusted by offset)
        transform.LookAt(targetPosition);
    }

    public void SetInputService(IInputService newService)
    {
        inputService = newService;
    }
}
