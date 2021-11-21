using System;
using DG.Tweening;
using UnityEngine;

public class BattleGuiTransitionComponent : MonoBehaviour
{
    public event BattleGuiEventHandler BattleFadeInEvent;
    public event BattleGuiEventHandler BattleFadeOutEvent;
    public delegate void BattleGuiEventHandler(object sender, EventArgs e);

    [SerializeField] private DotweenBroadcasterComponent _fadeInTweenBroadcasterComponent;
    [SerializeField] private DOTweenAnimation _fadeInTween;

    void Start()
    {
        _fadeInTweenBroadcasterComponent.DotweenCompleteEvent += OnBattleFadeInComplete;
        _fadeInTweenBroadcasterComponent.DotweenRewindCompleteEvent += OnBattleFadeOutComplete;
    }
    public void StartFadeIn()
    {
        _fadeInTween.DORestart();
    }

    /// <summary>
    /// Play the fade out at 3x speed so that it goes faster.
    /// </summary>
    public void StartFadeOut()
    {
        DOTween.timeScale = 3.0f;
        _fadeInTween.DOPlayBackwards();
    }

    /// <summary>
    /// This event is called when the fadein is complete.  Probably starts the between turn state.  This is chained from the dotween broadcaster event firing, then this fires.
    /// </summary>
    private void OnBattleFadeInComplete(object obj, EventArgs e)
    {
        BattleFadeInEvent?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// This event is called when the fadeout is complete.  probably switches the scene.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="e"></param>
    private void OnBattleFadeOutComplete(object obj, EventArgs e)
    {
        DOTween.timeScale = 1.0f;
        BattleFadeOutEvent?.Invoke(this, EventArgs.Empty);
    }
}
