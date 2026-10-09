using UnityEngine;

public class MobileInputService : IInputService
{
    // Stub implementation for mobile support.
    // In a real project, this would integrate with on-screen joysticks and buttons.

    public Vector2 GetMovementInput()
    {
        return Vector2.zero;
    }

    public Vector2 GetLookInput()
    {
        return Vector2.zero;
    }

    public bool IsSprinting()
    {
        return false;
    }

    public bool IsCrouching()
    {
        return false;
    }

    public bool IsInteractDown()
    {
        return false;
    }

    public bool IsScannerDown()
    {
        return false;
    }

    public bool IsScannerUp()
    {
        return false;
    }

    public bool IsExitDown()
    {
        return false;
    }
}
