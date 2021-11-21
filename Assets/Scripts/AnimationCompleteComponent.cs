using System;
using UnityEngine;

/// <summary>
/// This is used by any animation that is going to be firing a notify, this is used by simple animations with only one notify probably.
/// </summary>
public class AnimationCompleteComponent : MonoBehaviour
{
    public event AnimationCompleteEventHandler AnimationCompleteEvent;
    public delegate void AnimationCompleteEventHandler(object sender, EventArgs e);

    public void OnAnimationCompleteEvent()
    {
        AnimationCompleteEvent?.Invoke(this, EventArgs.Empty);
    }
}
