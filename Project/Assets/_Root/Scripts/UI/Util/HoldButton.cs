using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("UI/HoldButton")]
public class HoldButton : Button
{
    public readonly UnityEvent onDown = new UnityEvent();
    public readonly UnityEvent onUp= new UnityEvent();

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (IsActive() && IsInteractable())
        {
            base.OnPointerDown(eventData);
            onDown.Invoke();
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (IsActive() && IsInteractable())
        {
            base.OnPointerUp(eventData);
            onUp.Invoke();
        }
    }
}