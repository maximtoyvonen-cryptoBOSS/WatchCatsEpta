using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool isPressed;
    private bool wasPressedThisFrame;
    private bool wasReleasedThisFrame;

    public bool IsPressed => isPressed;

    private void Update()
    {
        // Reset single-frame flags at the end of the frame
        wasPressedThisFrame = false;
        wasReleasedThisFrame = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        wasPressedThisFrame = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        wasReleasedThisFrame = true;
    }

    public bool GetButtonDown()
    {
        return wasPressedThisFrame;
    }

    public bool GetButtonUp()
    {
        return wasReleasedThisFrame;
    }
}
