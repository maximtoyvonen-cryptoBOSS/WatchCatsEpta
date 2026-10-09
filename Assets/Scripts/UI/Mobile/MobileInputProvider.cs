using UnityEngine;

public class MobileInputProvider : MonoBehaviour
{
    [SerializeField] private VirtualJoystick joystick;
    [SerializeField] private TouchField touchField;
    [SerializeField] private VirtualButton interactBtn;
    [SerializeField] private VirtualButton scannerBtn;
    [SerializeField] private VirtualButton sprintBtn;
    [SerializeField] private VirtualButton crouchBtn;
    [SerializeField] private VirtualButton phoneBtn;
    [SerializeField] private VirtualButton exitBtn;

    private MobileInputService mobileInputService;

    public MobileInputService GetInputService()
    {
        if (mobileInputService == null)
        {
            mobileInputService = new MobileInputService(
                joystick, touchField, interactBtn, scannerBtn,
                sprintBtn, crouchBtn, phoneBtn, exitBtn);
        }
        return mobileInputService;
    }
}
