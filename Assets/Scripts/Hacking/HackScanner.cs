using UnityEngine;
using System;

public class HackScanner : MonoBehaviour
{
    [Header("Scanner Settings")]
    [SerializeField] private float scanRadius = 15f;
    [SerializeField] private float slowMotionScale = 0.4f;
    [SerializeField] private LayerMask hackableLayer;

    // Use a reference to the active camera to cast from the center of the screen
    private Camera activeCamera;
    private bool isScanning;
    private HackManager hackManager;
    private IInputService inputService;

    public event Action<HackableNode> OnTargetNodeChanged;
    private HackableNode currentTargetNode;

    private void Awake()
    {
        hackManager = GetComponent<HackManager>();
    }

    private void Start()
    {
        if (inputService == null)
        {
            if (YandexGamesManager.Instance != null && YandexGamesManager.Instance.IsMobile())
            {
                var inputProvider = UnityEngine.Object.FindObjectOfType<MobileInputProvider>();
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

    private void Update()
    {
        // Get the active camera from HackManager
        activeCamera = hackManager.GetActiveCamera();

        // Input check for scanner
        if (inputService.IsScannerDown())
        {
            StartScanning();
        }
        else if (inputService.IsScannerUp())
        {
            StopScanning();
        }

        if (isScanning)
        {
            PerformScan();

            // If player presses interact/hack while scanning
            if (inputService.IsInteractDown() && currentTargetNode != null)
            {
                hackManager.AttemptHack(currentTargetNode);
                StopScanning(); // Assuming hacking exits scanner mode
            }
        }
    }

    private void StartScanning()
    {
        isScanning = true;
        Time.timeScale = slowMotionScale;
    }

    private void StopScanning()
    {
        isScanning = false;
        Time.timeScale = 1f;
        UpdateTargetNode(null);
    }

    private void PerformScan()
    {
        if (activeCamera == null) return;

        Ray ray = activeCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, scanRadius, hackableLayer))
        {
            HackableNode node = hit.collider.GetComponent<HackableNode>();
            UpdateTargetNode(node);
        }
        else
        {
            UpdateTargetNode(null);
        }
    }

    private void UpdateTargetNode(HackableNode newNode)
    {
        if (currentTargetNode != newNode)
        {
            currentTargetNode = newNode;
            OnTargetNodeChanged?.Invoke(currentTargetNode);

            // Visual highlight logic could go here or be handled by subscribers to the event
        }
    }
}
