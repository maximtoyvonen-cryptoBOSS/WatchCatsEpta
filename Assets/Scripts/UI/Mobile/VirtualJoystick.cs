using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform joystickBackground;
    [SerializeField] private RectTransform joystickHandle;

    private Vector2 inputVector;
    public Vector2 InputVector => inputVector;

    private void Start()
    {
        if (joystickBackground == null)
        {
            joystickBackground = GetComponent<RectTransform>();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBackground, eventData.position, eventData.pressEventCamera, out position))
        {
            position.x = (position.x / joystickBackground.sizeDelta.x);
            position.y = (position.y / joystickBackground.sizeDelta.y);

            inputVector = new Vector2(position.x * 2 - 1, position.y * 2 - 1);
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

            if (joystickHandle != null)
            {
                joystickHandle.anchoredPosition = new Vector2(
                    inputVector.x * (joystickBackground.sizeDelta.x / 2),
                    inputVector.y * (joystickBackground.sizeDelta.y / 2));
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        if (joystickHandle != null)
        {
            joystickHandle.anchoredPosition = Vector2.zero;
        }
    }
}
