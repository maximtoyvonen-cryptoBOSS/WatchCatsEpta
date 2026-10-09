using UnityEngine;

public interface IInputService
{
    Vector2 GetMovementInput();
    Vector2 GetLookInput();
    bool IsSprinting();
    bool IsCrouching();
    bool IsInteractDown();
    bool IsScannerDown();
    bool IsScannerUp();
    bool IsExitDown();
}
