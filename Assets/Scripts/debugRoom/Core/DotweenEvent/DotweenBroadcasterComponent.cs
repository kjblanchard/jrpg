using System;
using DG.Tweening;
using UnityEngine;

public class DotweenBroadcasterComponent : MonoBehaviour
{
    [SerializeField] public DOTweenAnimation DotweenAnimation;

    public event DotweenCompleteEventHandler DotweenCompleteEvent;
    public event DotweenCompleteEventHandler DotweenRewindCompleteEvent;

    public delegate void DotweenCompleteEventHandler(object sender, EventArgs e);

    public void Start()
    {
        DotweenAnimation.autoPlay = false;
    }

    public void OnDotweenCompleteEvent()
    {
        DotweenCompleteEvent?.Invoke(this, EventArgs.Empty);
    }

    public void OnDotweenRewindEvent()
    {
        DotweenRewindCompleteEvent?.Invoke(this,EventArgs.Empty);

    }

}

