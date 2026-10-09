using UnityEngine;

public interface IInputService
{
    Vector2 GetMovementInput();
    Vector2 GetLookInput();
    bool IsSprinting();
    bool IsCrouching();
    bool IsInteractDown();
    bool IsInteractHeld();
    bool IsInteractUp();
    bool IsScannerDown();
    bool IsScannerUp();
    bool IsExitDown();
    bool IsPhoneToggleDown();
}
