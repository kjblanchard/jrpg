using System;
using UnityEngine;

public class BattlerAnimationComponent : MonoBehaviour
{
    public Animator BattlerAnimator;

    private const string _attackName = "startAttack";
    private const string _idleName = "startIdle";
    private const string _moveName = "startMoving";
    private const string _moveBackName = "startMoveBack";
    private const string _victoryName = "startVictory";
    private const string _takeDamageName = "startTakeDamage";
    private const string _walkingName = "startWalking";
    private const string _castingName = "startCasting";

    /// <summary>
    /// Changes the current battlers animation.
    /// </summary>
    /// <param name="animToPlays"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void ChangeAnimation(AbilityAnimStep.AnimToStartPlaying animToPlays)
    {
        if (animToPlays == AbilityAnimStep.AnimToStartPlaying.Default)
            return;
        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
        var animTriggerName = animToPlays switch
        {
            AbilityAnimStep.AnimToStartPlaying.Attack => _attackName,
            AbilityAnimStep.AnimToStartPlaying.Move => _moveName,
            AbilityAnimStep.AnimToStartPlaying.MoveBack => _moveBackName,
            AbilityAnimStep.AnimToStartPlaying.Idle => _idleName,
            AbilityAnimStep.AnimToStartPlaying.Victory => _victoryName,
            AbilityAnimStep.AnimToStartPlaying.TakeDamage => _takeDamageName,
            AbilityAnimStep.AnimToStartPlaying.Walking => _walkingName,
            AbilityAnimStep.AnimToStartPlaying.Casting => _castingName,
            _ => throw new ArgumentOutOfRangeException(nameof(animToPlays), animToPlays, "You didn't specify a proper enum")
        };

        BattlerAnimator.SetTrigger(animTriggerName);
    }

    /// <summary>
    /// Allows you to play a sound when performing animations
    /// </summary>
    /// <param name="soundToPlay">The enum for the sfx that should be played</param>
    public void PlaySoundAnimFunc(SoundController.Sfx soundToPlay)
    {
        SoundController.Instance.PlaySfx(soundToPlay);
    }
}
