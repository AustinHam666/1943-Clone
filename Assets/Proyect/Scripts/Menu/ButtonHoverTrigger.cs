using UnityEngine;
using UnityEngine.EventSystems; // Necesario para detectar el mouse

public class ButtonHoverTrigger : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    private MenuManager manager;
    private RectTransform myRect;

    void Awake()
    {
        manager = GetComponentInParent<MenuManager>();
        myRect = GetComponent<RectTransform>();
    }

    // Se activa al pasar el mouse
    public void OnPointerEnter(PointerEventData eventData)
    {
        manager.SetCursorAt(myRect);
    }

    // Se activa al usar flechas del teclado o joystick
    public void OnSelect(BaseEventData eventData)
    {
        manager.SetCursorAt(myRect);
    }
}