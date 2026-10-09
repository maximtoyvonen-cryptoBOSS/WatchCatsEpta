using UnityEngine;
using System;

[RequireComponent(typeof(HackingEnergy))]
public class HackManager : MonoBehaviour
{
    private HackingEnergy energyManager;
    private PlayerController playerController;
    private ThirdPersonCamera playerCamera;

    // Track if we are currently inside a camera
    private CCTVCameraNode activeCameraNode;

    public event Action<HackableNode> OnHackSuccessful;
    public event Action<HackableNode> OnHackFailed;

    private void Awake()
    {
        energyManager = GetComponent<HackingEnergy>();
        playerController = GetComponent<PlayerController>();
        playerCamera = UnityEngine.Object.FindObjectOfType<ThirdPersonCamera>(); // Or pass via inspector
    }

    private IInputService inputService;

    public void SetInputService(IInputService newService)
    {
        inputService = newService;
    }

    private void Start()
    {
        inputService = new DesktopInputService();
    }

    private void Update()
    {
        if (inputService == null) return;

        // Exit camera hack mode
        if (activeCameraNode != null && inputService.IsExitDown())
        {
            ExitCameraHack();
        }
    }

    public Camera GetActiveCamera()
    {
        if (activeCameraNode != null)
        {
            return activeCameraNode.GetCamera();
        }

        if (playerCamera != null)
        {
            return playerCamera.GetComponent<Camera>();
        }

        return Camera.main;
    }

    public void AttemptHack(HackableNode node)
    {
        if (node == null || !node.CanInteract()) return;

        if (energyManager.ConsumeEnergy(node.Cost))
        {
            node.ExecuteHack(this);
            OnHackSuccessful?.Invoke(node);
        }
        else
        {
            OnHackFailed?.Invoke(node);
        }
    }

    public void EnterCameraHack(CCTVCameraNode cctvCamera)
    {
        // If we are already in a camera, deactivate it first (chaining)
        if (activeCameraNode != null)
        {
            activeCameraNode.SetCameraActive(false);
        }
        else
        {
            // First time entering camera mode from player
            if (playerController != null) playerController.enabled = false;
            if (playerCamera != null) playerCamera.gameObject.SetActive(false);
        }

        activeCameraNode = cctvCamera;
        activeCameraNode.SetCameraActive(true);
    }

    public void ExitCameraHack()
    {
        if (activeCameraNode != null)
        {
            activeCameraNode.SetCameraActive(false);
            activeCameraNode = null;
        }

        // Restore player control
        if (playerController != null) playerController.enabled = true;
        if (playerCamera != null) playerCamera.gameObject.SetActive(true);
    }
}
