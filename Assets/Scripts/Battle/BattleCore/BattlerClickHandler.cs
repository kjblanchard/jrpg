using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This is a click handler that should be attached to all battlers.  It requires a box collider which is the area that can be clicked.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class BattlerClickHandler : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public BattleButtonBroadcaster _battleButtonBroadcaster;

    void Awake()
    {
        _battleButtonBroadcaster = new BattleButtonBroadcaster();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _battleButtonBroadcaster.OnButtonPressed();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _battleButtonBroadcaster.OnButtonHovered();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _battleButtonBroadcaster.OnButtonHoverLeave();
    }
}
