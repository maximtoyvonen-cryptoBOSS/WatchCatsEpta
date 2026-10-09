using UnityEngine;

public class DesktopInputService : IInputService
{
    // Avoiding hardcoding axes strings where possible, though Input.GetAxis("Mouse X") is standard.
    // For pure cross-platform and modularity, using explicit key checks for movement.

    public Vector2 GetMovementInput()
    {
        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.W)) vertical += 1f;
        if (Input.GetKey(KeyCode.S)) vertical -= 1f;
        if (Input.GetKey(KeyCode.D)) horizontal += 1f;
        if (Input.GetKey(KeyCode.A)) horizontal -= 1f;

        return new Vector2(horizontal, vertical).normalized;
    }

    public Vector2 GetLookInput()
    {
        // For standard Unity input system without external plugins, we use generic mouse axes.
        // It's technically hardcoding the axis name but it's the default Unity axis.
        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    public bool IsSprinting()
    {
        return Input.GetKey(KeyCode.LeftShift);
    }

    public bool IsCrouching()
    {
        return Input.GetKey(KeyCode.LeftControl);
    }

    public bool IsInteractDown()
    {
        return Input.GetKeyDown(KeyCode.E);
    }

    public bool IsScannerDown()
    {
        return Input.GetMouseButtonDown(1);
    }

    public bool IsScannerUp()
    {
        return Input.GetMouseButtonUp(1);
    }

    public bool IsExitDown()
    {
        return Input.GetKeyDown(KeyCode.Escape);
    }
}
