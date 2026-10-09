using UnityEngine;
using UnityEngine.EventSystems;

public class TouchField : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private bool isPressed;
    private Vector2 touchDist;
    private Vector2 pointerOld;

    public Vector2 TouchDist => touchDist;

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        pointerOld = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        touchDist = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isPressed)
        {
            touchDist = eventData.position - pointerOld;
            pointerOld = eventData.position;
        }
    }

    private void Update()
    {
        if (!isPressed)
        {
            touchDist = Vector2.zero;
        }
    }
}
