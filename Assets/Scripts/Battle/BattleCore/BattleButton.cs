using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// These are the buttons that will be in the menu screens.  Relies on the battle button broadcaster which holds all the events that things should subscribe to.
/// This button overrides unity's button and has all the events we will be firing when pointers exit and enter
/// </summary>
public class BattleButton : Button
{

    public bool IsSelected;

    /// <summary>
    /// You have to hit debug to be able to edit this.
    /// </summary>
    [SerializeField] public DOTweenAnimation IsSelectedAnimation;

    public bool isPlayingSelectedAnimation = false;
    public BattleButtonBroadcaster BattleButtonBroadcaster = new BattleButtonBroadcaster();


    public override void OnPointerClick(PointerEventData eventData)
    {
        BattleButtonBroadcaster.OnButtonPressed();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        BattleButtonBroadcaster.OnButtonHovered();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        BattleButtonBroadcaster.OnButtonHoverLeave();
        //base.OnPointerExit(eventData);
    }




}
