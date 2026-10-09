using UnityEngine;

[RequireComponent(typeof(Camera), typeof(AudioListener))]
public class CCTVCameraNode : HackableNode
{
    [Header("CCTV Settings")]
    [SerializeField] private float panSpeed = 50f;
    [SerializeField] private float tiltSpeed = 50f;
    [SerializeField] private float maxPanAngle = 60f;
    [SerializeField] private float maxTiltAngle = 30f;

    private Camera cam;
    private AudioListener audioListener;
    private bool isActiveHack;

    private float currentPan;
    private float currentTilt;
    private Quaternion initialRotation;

    private IInputService inputService;

    private void Awake()
    {
        hackType = HackType.CCTVCamera;
        cam = GetComponent<Camera>();
        audioListener = GetComponent<AudioListener>();

        cam.enabled = false;
        audioListener.enabled = false;

        initialRotation = transform.rotation;

        // Extract current euler angles relative to initial, or start at 0
        currentPan = 0f;
        currentTilt = 0f;
    }

    private void Start()
    {
        if (inputService == null)
        {
            if (YandexGamesManager.Instance != null && YandexGamesManager.Instance.IsMobile())
            {
                var inputProvider = Object.FindObjectOfType<MobileInputProvider>();
                if (inputProvider != null) inputService = inputProvider.GetInputService();
            }

            if (inputService == null)
            {
                inputService = new DesktopInputService();
            }
        }
    }

    public void SetInputService(IInputService newService)
    {
        inputService = newService;
    }

    public Camera GetCamera()
    {
        return cam;
    }

    private void Update()
    {
        if (isActiveHack)
        {
            HandleRotation();
        }
    }

    private void HandleRotation()
    {
        Vector2 lookInput = inputService.GetLookInput();
        float mouseX = lookInput.x;
        float mouseY = lookInput.y;

        currentPan += mouseX * panSpeed * Time.deltaTime;
        currentTilt -= mouseY * tiltSpeed * Time.deltaTime;

        currentPan = Mathf.Clamp(currentPan, -maxPanAngle, maxPanAngle);
        currentTilt = Mathf.Clamp(currentTilt, -maxTiltAngle, maxTiltAngle);

        transform.rotation = initialRotation * Quaternion.Euler(currentTilt, currentPan, 0f);
    }

    public override void ExecuteHack(HackManager manager)
    {
        manager.EnterCameraHack(this);
    }

    public void SetCameraActive(bool state)
    {
        isActiveHack = state;
        cam.enabled = state;
        audioListener.enabled = state;
    }
}
