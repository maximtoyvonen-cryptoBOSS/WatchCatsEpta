using UnityEngine;

public class MobileInputService : IInputService
{
    private VirtualJoystick joystick;
    private TouchField touchField;
    private VirtualButton interactButton;
    private VirtualButton scannerButton;
    private VirtualButton sprintButton;
    private VirtualButton crouchButton;
    private VirtualButton phoneButton;
    private VirtualButton exitButton;

    public MobileInputService(VirtualJoystick joystick, TouchField touchField,
                              VirtualButton interactBtn, VirtualButton scannerBtn,
                              VirtualButton sprintBtn, VirtualButton crouchBtn,
                              VirtualButton phoneBtn, VirtualButton exitBtn)
    {
        this.joystick = joystick;
        this.touchField = touchField;
        this.interactButton = interactBtn;
        this.scannerButton = scannerBtn;
        this.sprintButton = sprintBtn;
        this.crouchButton = crouchBtn;
        this.phoneButton = phoneBtn;
        this.exitButton = exitBtn;
    }

    public Vector2 GetMovementInput()
    {
        return joystick != null ? joystick.InputVector : Vector2.zero;
    }

    public Vector2 GetLookInput()
    {
        return touchField != null ? touchField.TouchDist : Vector2.zero;
    }

    public bool IsSprinting()
    {
        return sprintButton != null && sprintButton.IsPressed;
    }

    public bool IsCrouching()
    {
        return crouchButton != null && crouchButton.IsPressed;
    }

    public bool IsInteractDown()
    {
        return interactButton != null && interactButton.GetButtonDown();
    }

    public bool IsInteractHeld()
    {
        return interactButton != null && interactButton.IsPressed;
    }

    public bool IsInteractUp()
    {
        return interactButton != null && interactButton.GetButtonUp();
    }

    public bool IsScannerDown()
    {
        return scannerButton != null && scannerButton.GetButtonDown();
    }

    public bool IsScannerUp()
    {
        return scannerButton != null && scannerButton.GetButtonUp();
    }

    public bool IsExitDown()
    {
        return exitButton != null && exitButton.GetButtonDown();
    }

    public bool IsPhoneToggleDown()
    {
        return phoneButton != null && phoneButton.GetButtonDown();
    }
}
